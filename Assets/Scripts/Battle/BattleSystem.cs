using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public enum BattleState { NoneUnitSelected, PlayerUnitSelected, PlayerAbilitySelected, EnemyTurn, DisplayUnitInfo, BattleOver, Busy }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] List<BattleUnit> playerBattleUnits;

    [SerializeField] List<BattleUnit> enemyBattleUnits;

    [SerializeField] BattleDialogBox dialogBoxContainer;

    [SerializeField] AttackStats attackStats;
    [SerializeField] UnitInformation unitInformation;

    [SerializeField] List<TurnIconIndicator> turnIcons;
    [SerializeField] UnitInfoDisplay unitInfoDisplay;
    [SerializeField] CanvasGroup unitInfoCG;

    public event System.Action<bool> OnBattleOver;

    public FloatingTextManager floatingTextManager;

    BattleState state;
    BattleState stateTemp;

    BattleUnit currentPlayerUnit;
    BattleUnit unitToDisplayInfo;
    int currentAbilityIndex;
    int currentEnemyUnit;

    bool isEliteBattle;
    int escapeAttempts;

    int expGain;

    List<Unit> playerUnits;
    List<Unit> enemyUnits;

    AbilityTarget abilityType;
    // turns 
    List<BattleUnit> unitTurns = new List<BattleUnit>();
    SortBySpeed sortBySpeed = new SortBySpeed();
    
    private void Update()
    {
        if (stateTemp != state)
        {
            stateTemp = state;
            /*ClearLog();
            Debug.Log(state);*/
        }

        if (state == BattleState.PlayerUnitSelected)
            HandleUnitInfoDisplay();

        HandleEscapeKey();
    }
    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
    public void StartBattle(List<Unit> playerUnits, List<Unit> enemyUnits, bool isEBattle = false)
    {
        if (isEBattle == true)
        {
            isEliteBattle = true;
            Debug.Log("Is an elite battle");
        }
        else
        {
            isEliteBattle = false;
            Debug.Log("Is not an elite battle");
        }


        this.playerUnits = playerUnits;
        this.enemyUnits = enemyUnits;
        StartCoroutine(SetUpBattle());

        unitInfoDisplay.HideUnitInfoInstant();

        // init the turn list
        SetUpTurnList(playerUnits, enemyUnits);
    }

    public IEnumerator SetUpBattle()
    {
        for (int i = 0; i < playerBattleUnits.Count; i++)
        {
            if (i < playerUnits.Count)
            {
                playerBattleUnits[i].SetUp(playerUnits[i], true);
            }
            else
                playerBattleUnits[i].AllyHud.gameObject.SetActive(false);
        }
        for (int i = 0; i < enemyBattleUnits.Count; i++)
        {
            if (i < enemyUnits.Count)
            {
                enemyBattleUnits[i].SetUp(enemyUnits[i], false);
                enemyBattleUnits[i].DeFadeCG();
            }
            else
                enemyBattleUnits[i].EnemyHud.gameObject.SetActive(false);
        }

        escapeAttempts = 0;
        expGain = 0;

        //StartCoroutine(dialogBoxContainer.TypeDialog($"You are being attacked!"));
        yield return new WaitForSeconds(1.5f);
        //dialogBoxContainer.enableDialogBoxContainer(false);
    }

    public void SetUpTurnList(List<Unit> playerUnits, List<Unit> enemyUnits)
    {

        // init the turn list
        unitTurns.Clear();
        for (int a = 0; a < playerUnits.Count; a++)
            if (playerUnits[a].isDead == false)
                unitTurns.Add(playerBattleUnits[a]);
        for (int e = 0; e < enemyUnits.Count; e++)
            if (enemyUnits[e].isDead == false)
                unitTurns.Add(enemyBattleUnits[e]);
        unitTurns.Sort(sortBySpeed);
        unitTurns.Reverse();
        StartCoroutine(EndTurn(false, null, true));
    }

    void PerformPlayerAbility(BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        //state = BattleState.PlayerAbilitySelected;
        state = BattleState.PlayerUnitSelected;
        var ability = sourceUnit.Unit.Abilities[currentAbilityIndex];

        StartCoroutine(RunTurn(sourceUnit, targetUnit, ability));
    }

    void PerformEnemyAbility(BattleUnit sourceUnit)
    {
        state = BattleState.EnemyTurn;

        var ability = sourceUnit.Unit.GetRandomAbility();
        var targetType = ability.Base.Target;
        var targetCategory = ability.Base.Category;
        BattleUnit targetUnit = null;
        //  GetRandomUnit(bool) true get playerunit, false get enemyunit
        if (targetCategory == AbilityCategory.Status)
        {
            if (targetType == AbilityTarget.AllAlly || targetType == AbilityTarget.SingleAlly || targetType == AbilityTarget.Self)
                targetUnit = GetRandomUnit(false);
            else if (targetType == AbilityTarget.AllFoe || targetType == AbilityTarget.SingleFoe)
                targetUnit = GetRandomUnit();
        }
        else if (targetCategory == AbilityCategory.Physical || targetCategory == AbilityCategory.Magical)
        {
            targetUnit = GetRandomUnit();
        }

        StartCoroutine(RunTurn(sourceUnit, targetUnit, ability));


        /***** need to implement game over at some point *****/
    }

    void TryToEscape()
    {
        if (isEliteBattle == true)
            return;

        state = BattleState.Busy;

        ++escapeAttempts;
        float enemiesASP = 0;
        float alliesASP = 0 + escapeAttempts*2;

        foreach (var unit in enemyUnits)
        {
            enemiesASP += unit.ActionSP;
        }
        foreach (var unit in playerUnits)
        {
            alliesASP += unit.ActionSP;
        }

        float chance = alliesASP / enemiesASP;
        float rc = Random.Range(0f, 1f);
        if (rc <= chance)
        {
            Debug.Log("Ran away");
            Debug.Log("random chane: " + rc);
            Debug.Log("chance: " + chance);

            BattleOver(true);
        }
        else
        {
            state = BattleState.PlayerUnitSelected;
            StartCoroutine(EndTurn());
        }

        Debug.Log("random chane: " + rc);
        Debug.Log("chance: " + chance);
    }
    IEnumerator RunTurn(BattleUnit sourceUnit, BattleUnit targetUnit, ActiveAbility ability)
    {
        bool canRunTurn = sourceUnit.Unit.OnBeforeTurn();
        if (canRunTurn == false)
        {
            StartCoroutine(EndTurn());
            yield break; // stop the couroutine
        }
        sourceUnit.PlayAttackAnimation();
        //yield return new WaitForSeconds(1.5f);
        yield return null;

        CheckAbilityCost(ability, sourceUnit);

        // apply Status
        if (ability.Base.Category == AbilityCategory.Status)
        {
            RunAbilityEffects(sourceUnit, targetUnit, ability);
        }
        else // perform attack if not status ability
        {
            sourceUnit.PlayAttackAnimation();
            targetUnit.PlayHitAnimation();

            var damageDetails = targetUnit.Unit.TakeDamage(ability, sourceUnit.Unit);
            if (sourceUnit.isPlayerUnit)
                targetUnit.EnemyHud.UpdateHP();
            else
                targetUnit.AllyHud.UpdateHP();

            // show damage detail
            ShowDamageDetail(sourceUnit, targetUnit, damageDetails);
        }

        if (targetUnit.Unit.HP <= 0)
        {
            targetUnit.Unit.isDead = true;
            // exp handle
            if (targetUnit.isPlayerUnit == false)
            {
                ExpCaculate(targetUnit);
            }
            targetUnit.PlayDeadAnimation();
            StartCoroutine(EndTurn(true, targetUnit.Unit));
        }

        if (sourceUnit.isPlayerUnit == true && CheckAllEnemyUnitDead() == true)
            BattleOver(true);
        else if (sourceUnit.isPlayerUnit == true && CheckAllAllyUnitDead() == true)
            BattleOver(false);
        else if (sourceUnit.isPlayerUnit == false && CheckAllEnemyUnitDead() == true)
            BattleOver(true);
        else if (sourceUnit.isPlayerUnit == false && CheckAllAllyUnitDead() == true)
            BattleOver(false);
        else if (targetUnit.Unit.HP > 0)
            StartCoroutine(EndTurn());
    }
    void RunAbilityEffects(BattleUnit sourceUnit, BattleUnit targetUnit, ActiveAbility ability)
    {
        var effect = ability.Base.Effects;
        var targetType = ability.Base.Target;

        // stats boosting
        if (effect.Boosts != null)
        {   //--SELF BUFF
            if (targetType == AbilityTarget.Self)
            {
                sourceUnit.Unit.ApplieBoosts(effect.Boosts);
                PlayBuff(ability, sourceUnit);
            }

            //--SINGLE ENEMY DEBUFF
            else if (targetType == AbilityTarget.SingleFoe)
            {
                targetUnit.Unit.ApplieBoosts(effect.Boosts);
                PlayBuff(ability, targetUnit);
            }

            //--ALL ENEMY DEBUFF
            else if (targetType == AbilityTarget.AllFoe)
            {
                List<BattleUnit> list;
                if (sourceUnit.isPlayerUnit == true)
                    list = enemyBattleUnits;
                else
                    list = playerBattleUnits;
                foreach (var unit in list)
                {
                    if (unit.gameObject.active)
                        if (unit.Unit.isDead == false)
                        {
                            unit.Unit.ApplieBoosts(effect.Boosts);
                            PlayBuff(ability, unit);
                        }
                }
            }

            //--SINGLE ALLY BUFF
            else if (targetType == AbilityTarget.SingleAlly)
            {
                targetUnit.Unit.ApplieBoosts(effect.Boosts);
                PlayBuff(ability, targetUnit);
            }

            //--ALL ALLY BUFF
            else if (targetType == AbilityTarget.AllAlly)
            {
                List<BattleUnit> list;
                if (sourceUnit.isPlayerUnit == true)
                    list = playerBattleUnits;
                else
                    list = enemyBattleUnits;
                foreach (var unit in list)
                {
                    if (unit.gameObject.active)
                        if (unit.Unit.isDead == false)
                        {
                            unit.Unit.ApplieBoosts(effect.Boosts);
                            PlayBuff(ability, unit);
                        }
                }
            }
        }

        // apply status effect
        if (effect.Status != ConditionID.none)
        {
            if (targetType == AbilityTarget.SingleFoe)
                targetUnit.Unit.SetStatus(effect.Status);
            else if (targetType == AbilityTarget.AllFoe)
            {
                List<BattleUnit> list;
                if (sourceUnit.isPlayerUnit == true)
                    list = enemyBattleUnits;
                else
                    list = playerBattleUnits;
                foreach (var unit in list)
                {
                    if (unit.gameObject.active)
                        if (unit.Unit.isDead == false)
                        {
                            unit.Unit.SetStatus(effect.Status);
                        }
                }
            }
        }
    }
    IEnumerator EndTurn(bool isUnitDead = false, Unit unitDied = null, bool justStarted = false)
    {
        UpdateTurnDisplay();
        UnHighLightUnit();

        //state = BattleState.NoneUnitSelected;

        if (justStarted == false)
        {
            yield return new WaitForSeconds(1f); 
            if (isUnitDead == false)
            {
                var firstIndex = unitTurns[0];
                unitTurns.RemoveAt(0);
                unitTurns.Add(firstIndex);
            }
            else
            {
                var firstIndex = unitTurns[0];
                unitTurns.RemoveAt(0);
                unitTurns.Add(firstIndex);
                for (int i = 0; i < unitTurns.Count; i++)
                {
                    if (unitTurns[i].Unit == unitDied)
                    {
                        unitTurns.RemoveAt(i);
                    }
                }
            }
        }
        UpdateTurnDisplay();

        // start enemy turn if unit is enemy/ally turn if unit is ally
        if (unitTurns[0].isPlayerUnit == true)
        {
            state = BattleState.PlayerUnitSelected;
            if (CheckAllEnemyUnitDead() == false)
            {
                currentPlayerUnit = unitTurns[0];
                TriggerStatus(currentPlayerUnit);
                attackStats.SetStats(currentPlayerUnit.Unit);
                unitInformation.SetActiveAbiblity(currentPlayerUnit.Unit.Abilities);
                HideAllyHighLight();
                currentPlayerUnit.AllyHud.Body.sprite = currentPlayerUnit.AllyHud.UnitActive;

            }
        }

        if (unitTurns[0].isPlayerUnit == false)
        {
            if (unitTurns[0].Unit != unitDied || unitTurns[0].Unit.HP > 0 || unitTurns[0].Unit.isDead == false)
            {
                if (CheckAllAllyUnitDead() == false)
                {
                    yield return new WaitForSeconds(0.5f);
                    if (TriggerStatus(unitTurns[0]) == false)
                    {
                        yield return new WaitForSeconds(0.8f);
                        PerformEnemyAbility(unitTurns[0]);
                    }
                    else
                    {
                        if (CheckAllAllyUnitDead() == false && CheckAllEnemyUnitDead() == true)
                            BattleOver(true);
                        else if (CheckAllAllyUnitDead() == true && CheckAllEnemyUnitDead() == false)
                            BattleOver(false);
                    }
                }
            }
        }
        
    }
    bool TriggerStatus(BattleUnit targetUnit)
    {
        // trigger status effects after turn
        targetUnit.Unit.OnAfterTurn();
        if (targetUnit.Unit.HPChanged == true)
        {
            if (targetUnit.isPlayerUnit)
                targetUnit.AllyHud.UpdateHP();
            else
                targetUnit.EnemyHud.UpdateHP();
            targetUnit.PlayHitAnimation();
            ShowDamageDetail(targetUnit, targetUnit);
        }
        if (targetUnit.Unit.HP <= 0)
        {
            targetUnit.Unit.isDead = true;
            targetUnit.PlayDeadAnimation();
            ExpCaculate(targetUnit);
            StartCoroutine(EndTurn(true, targetUnit.Unit));
            return true;
        }
        else
            return false;

        if (targetUnit.isPlayerUnit == true && CheckAllEnemyUnitDead() == true)
            BattleOver(true);
        else if (targetUnit.isPlayerUnit == true && CheckAllAllyUnitDead() == true)
            BattleOver(false);
        else if (targetUnit.isPlayerUnit == false && CheckAllEnemyUnitDead() == true)
            BattleOver(true);
        else if (targetUnit.isPlayerUnit == false && CheckAllAllyUnitDead() == true)
            BattleOver(false);
    }
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerUnits.ForEach(u => u.OnBattleOver());
        if (won)
        {
            Debug.Log("battle won");
            StartCoroutine(HandleExpGain());
            Debug.Log(playerUnits.Count+" ally(s) unit gained "+expGain/playerUnits.Count+" exp each from "+expGain+" exp");
        }
        else
            Debug.Log("battle lost");
        OnBattleOver(won);
    }
    void CheckAbilityCost(ActiveAbility ability, BattleUnit unit)
    {
        if (ability.Base.HPCost > 0)
        {
            unit.Unit.HP -= ability.Base.HPCost;

            if (unit.Unit.HP <= 0)
                unit.Unit.HP = 0;
        }
        if (ability.Base.MPCost > 0)
        {
            unit.Unit.MP -= ability.Base.MPCost;

            if (unit.Unit.MP <= 0)
                unit.Unit.MP = 0;
        }
        if (unit.isPlayerUnit)
        {
            unit.AllyHud.UpdateHP();
            unit.AllyHud.UpdateMP();
        }
        else
        {
            unit.EnemyHud.UpdateHP();
            //unit.EnemyHud.UpdateMP();
        }
    }
    void PlayBuff(ActiveAbility ability, BattleUnit target)
    {
        if (ability.Base.BType == BuffType.Buff)
            StartCoroutine(target.BuffIndicator.PlayBuffAplly(true));
        else
            StartCoroutine(target.BuffIndicator.PlayBuffAplly(false));
    }
    void UpdateTurnDisplay()
    {
        for (int i = 0; i < turnIcons.Count; i++)
        {
            if (i < unitTurns.Count)
            {
                if (unitTurns[i].isPlayerUnit == true)
                {
                    turnIcons[i].gameObject.SetActive(true);
                    turnIcons[i].SetFoeOrAlly(unitTurns[i].Unit.Base.BackSprite, true);
                }
                else
                {
                    turnIcons[i].gameObject.SetActive(true);
                    turnIcons[i].SetFoeOrAlly(unitTurns[i].Unit.Base.FrontSprite, false);
                }
            }
            else
                turnIcons[i].gameObject.SetActive(false);
        }
    }
    void UnHighLightUnit()
    {
        for (int i = 0; i < playerBattleUnits.Count; i++)
        {
            if (playerBattleUnits[i].gameObject.active)
            {
                playerBattleUnits[i].AllyHud.HideSelection();
                attackStats.SetReset();
            }
        }
        unitInformation.ResetDescription();
        unitInformation.DeHighlightAA();
        unitInformation.ResetCosts();
    }

    BattleUnit GetRandomUnit(bool getAllyUnit = true) // true get ally, false get enemy
    {
        List<int> unitList = new List<int>();
        List<Unit> list;
        if (getAllyUnit == true)
            list = playerUnits;
        else
            list = enemyUnits;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].isDead == false)
                unitList.Add(i);
        }
        int r = Random.Range(0, unitList.Count);

        if (getAllyUnit == true)
            return playerBattleUnits[unitList[r]];
        else
            return enemyBattleUnits[unitList[r]];

        unitList.Clear();
        list = null;
    }

    bool CheckAllEnemyUnitDead()
    {
        List<int> aliveEnemyList = new List<int>();
        for (int i = 0; i < enemyUnits.Count; i++)
            if (enemyUnits[i].isDead == false)
                aliveEnemyList.Add(i);

        if (aliveEnemyList.Count > 0)
            return false;
        else
            return true;
        aliveEnemyList.Clear();
    }
    bool CheckAllAllyUnitDead()
    {
        List<int> aliveAllyList = new List<int>();
        for (int i = 0; i < playerUnits.Count; i++)
            if (playerUnits[i].isDead == false)
                aliveAllyList.Add(i);

        if (aliveAllyList.Count > 0)
            return false;
        else
            return true;
    }

    void ExpCaculate(BattleUnit unitDied)
    {
        // exp gain from killed foes
        int expYield = unitDied.Unit.Base.ExpYield;
        int enemyLvl = unitDied.Unit.Level;
        float eliteBonus = (isEliteBattle) ? 1.5f : 1f;

        int expFromKill = Mathf.FloorToInt((expYield * enemyLvl * eliteBonus) / 7);
        expGain = expGain + expFromKill;
        Debug.Log("expFromKill " + expFromKill);
    }

    IEnumerator HandleExpGain()
    {
        // gives exp to player units
        foreach (var unit in playerUnits)
        {
            int amountLvl = 0;
            unit.Exp += (expGain / playerUnits.Count);
            Debug.Log(unit.Base.name + " " + unit.Exp);

            foreach (var battleUnit in playerBattleUnits)
            {
                if (battleUnit.Unit == unit)
                {
                    yield return battleUnit.AllyHud.SetExpSmooth();
                    while (unit.CheckForLevelUp())
                    {
                        // if level up the reset the level
                        ++amountLvl;
                        battleUnit.AllyHud.SetLevel(amountLvl);

                        // level up skill
                        var abilityToLvlUp= unit.GetLearnableAbility();
                        if (abilityToLvlUp != null)
                        {
                            if (unit.Abilities.Count < UnitBase.MaxNumOfAbilities)
                            {
                                unit.LevelUpAbility(abilityToLvlUp);
                                Debug.Log(unit.Base.Name + " levelUp" + abilityToLvlUp.Base.Name);
                            }
                        }
                        battleUnit.AllyHud.UpdateHP();
                        battleUnit.AllyHud.UpdateMP();

                        yield return battleUnit.AllyHud.SetExpSmooth(true);
                    }
                }
            }
        }
    }

    void ShowDamageDetail(BattleUnit sourceUnit, BattleUnit targetUnit, DamageDetails damageDetails = null, string status = "") 
    {
        if (sourceUnit.isPlayerUnit == true)
        {
            if (damageDetails != null)
            {
                if (damageDetails.Critical == 1)
                    FloatingDamageDetail(targetUnit.Unit, null, targetUnit.EnemyHud, false);
                else if (damageDetails.Critical > 1)
                    FloatingDamageDetail(targetUnit.Unit, null, targetUnit.EnemyHud, true);
            }
            else
                FloatingDamageDetail(sourceUnit.Unit, sourceUnit.AllyHud, null, false, status);
        }
        else
        {
            if (damageDetails != null)
            {
                if (damageDetails.Critical == 1)
                    FloatingDamageDetail(targetUnit.Unit, targetUnit.AllyHud, null, false);
                else if (damageDetails.Critical > 1)
                    FloatingDamageDetail(targetUnit.Unit, targetUnit.AllyHud, null, true, status);
            }
            else
                FloatingDamageDetail(sourceUnit.Unit, null, sourceUnit.EnemyHud, false);
        }
    }
    void FloatingDamageDetail(Unit unit, BattleHud allyHud = null, EnemyBattleHUD enemyHud = null, bool isCrited = false, string status = "")
    {
        if (allyHud == null)
        {
            var p = enemyHud.gameObject.transform.position;
            if (isCrited)
                ShowText("-" + unit.damageTaken + " CRIT", 23, Color.red,
                         new Vector3(p.x+0.6f, p.y+0.3f, p.z), Vector3.up * 0.25f, 0.5f);
            else
                // show the damage taken
                ShowText("-" + unit.damageTaken + status, 23, Color.red,
                             new Vector3(p.x+0.3f, p.y+0.3f, p.z), Vector3.up * 0.25f, 0.5f);
        }
        else if (enemyHud == null)
        {
            var p = allyHud.gameObject.transform.position;
            if (isCrited)
            {
                // show the damage taken
                ShowText("-" + unit.damageTaken + " CRIT", 23, Color.red,
                         new Vector3(p.x+0.5f, p.y + 0.8f, p.z), Vector3.up * 0.25f, 0.5f);
            }
            else
            {
                // show the damage taken
                ShowText("-" + unit.damageTaken + status, 23, Color.red,
                         new Vector3(p.x + 0.25f, p.y + 0.8f, p.z), Vector3.up * 0.25f, 0.5f);
            }
        }
        else
        {

        }
    }

    // display text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }
    public void HandleUpdate()
    {

    }


    // PLAYER UNIT MOUSE INTERACTION
    public void UnitHover(int unit)
    {
        if (state == BattleState.PlayerAbilitySelected)
        {
            if (playerBattleUnits[unit] == currentPlayerUnit && abilityType == AbilityTarget.Self)
                playerBattleUnits[unit].AllyHud.ShowAllyBuffTargetHover();
            else if (abilityType == AbilityTarget.AllAlly)
            {
                foreach (var bUnit in playerBattleUnits)
                {
                    bUnit.AllyHud.ShowAllyBuffTargetHover();
                }
            }
            else if (abilityType == AbilityTarget.SingleAlly)
            {
                playerBattleUnits[unit].AllyHud.ShowAllyBuffTargetHover();
            }
        }
    }
    public void UnitExit()
    {
        if (state == BattleState.PlayerAbilitySelected)
        {
            if (abilityType == AbilityTarget.AllAlly || abilityType == AbilityTarget.SingleAlly)
            {
                ShowAllyHighLight();
                foreach (var bUnit in playerBattleUnits)
                {
                    bUnit.AllyHud.ShowAllyBuffTarget();
                }
            }
            else if (abilityType == AbilityTarget.Self)
            {
                currentPlayerUnit.AllyHud.ShowAllyBuffTarget();
            }

        }
    }
    public void UnitClicked(int unit)
    {
        if (state == BattleState.PlayerUnitSelected)
        {
            ShowSelection();
            unitToDisplayInfo = playerBattleUnits[unit];
            StartCoroutine(unitInfoDisplay.SetUp(playerUnits[unit]));
            state = BattleState.DisplayUnitInfo;
        }
        else if (state == BattleState.PlayerAbilitySelected)
        {
            var ability = currentPlayerUnit.Unit.Abilities[currentAbilityIndex];
            if (ability.MPCost > currentPlayerUnit.Unit.MP || ability.HPCost > currentPlayerUnit.Unit.HP)
            {
            }
            else
            {
                /*unitInformation.ResetDescription();
                unitInformation.DeHighlightAA();
                unitInformation.ResetCosts();*/
                if (playerBattleUnits[unit] == currentPlayerUnit && abilityType == AbilityTarget.Self)
                    PerformPlayerAbility(currentPlayerUnit, currentPlayerUnit);
                else if (abilityType == AbilityTarget.SingleAlly || abilityType == AbilityTarget.AllAlly)
                    PerformPlayerAbility(currentPlayerUnit, playerBattleUnits[unit]);
                state = BattleState.PlayerUnitSelected;
            }
        }

    }


    // ACTIVE ABILITY MOUSE INTERACTION
    public void ActiveAbilityHover(int ability)
    {
        if (state == BattleState.PlayerUnitSelected)
        {
            unitInformation.SetActiveDescription(currentPlayerUnit.Unit.Abilities[ability]);
            unitInformation.HighlightAA(ability);
            unitInformation.SetCosts(currentPlayerUnit.Unit.Abilities[ability]);
            int HPcost = currentPlayerUnit.Unit.Abilities[ability].Base.HPCost;
            int MPcost = currentPlayerUnit.Unit.Abilities[ability].Base.MPCost;
            if (HPcost > 0)
            {
                currentPlayerUnit.AllyHud.ShowHPPreview(HPcost);
            }
            if (MPcost > 0)
            {
                currentPlayerUnit.AllyHud.ShowMPPreview(MPcost);
            }

        }
        else if (state == BattleState.DisplayUnitInfo)
        {
            unitInfoDisplay.SetActiveDescription(unitToDisplayInfo.Unit.Abilities[ability]);
            unitInfoDisplay.HighlightAA(ability);
            unitInfoDisplay.SetCosts(unitToDisplayInfo.Unit.Abilities[ability]);
        }
    }
    public void ActiveAbilityHoverExit()
    {

        if (state == BattleState.PlayerUnitSelected)
        {
            unitInformation.ResetDescription();
            unitInformation.DeHighlightAA();
            unitInformation.ResetCosts();
            currentPlayerUnit.AllyHud.hidePreview();
        }
        else if (state == BattleState.DisplayUnitInfo)
        {
            unitInfoDisplay.ResetDescription();
            unitInfoDisplay.DeHighlightAA();
            unitInfoDisplay.ResetCosts();
        }
    }
    public void ActiveAbilityClicked(int ability)
    {
        if (state == BattleState.PlayerUnitSelected)
        {
            currentAbilityIndex = ability;
            state = BattleState.PlayerAbilitySelected;

            unitInformation.SetActiveDescription(currentPlayerUnit.Unit.Abilities[ability]);
            unitInformation.HighlightAA(ability);
            unitInformation.SetCosts(currentPlayerUnit.Unit.Abilities[ability]);
        }
        else if (state == BattleState.PlayerAbilitySelected)
        {
            if (ability != currentAbilityIndex)
            {
                currentAbilityIndex = ability;

                ActiveAbilityHoverExit();

                unitInformation.DeHighlightAA();
                unitInformation.SetActiveDescription(currentPlayerUnit.Unit.Abilities[ability]);
                unitInformation.HighlightAA(ability);
                unitInformation.SetCosts(currentPlayerUnit.Unit.Abilities[ability]);
                ShowSelection();
            }
        }
        ShowSelection();
    }
    void ShowSelection()
    {
        if (state == BattleState.PlayerAbilitySelected)
        {
            var targetType = currentPlayerUnit.Unit.Abilities[currentAbilityIndex].Base.Target;
            abilityType = targetType;
            if (abilityType == AbilityTarget.AllFoe || abilityType == AbilityTarget.SingleFoe)
            {
                ShowEnemyHighLight();
            }
            else if (abilityType == AbilityTarget.SingleAlly || abilityType == AbilityTarget.AllAlly)
            {
                ShowAllyHighLight();
                HideEnemyHighLight();
            }
            else if (abilityType == AbilityTarget.Self)
            {
                currentPlayerUnit.AllyHud.ShowAllyBuffTarget();
                HideEnemyHighLight();
            }
        }
        else if (state == BattleState.PlayerUnitSelected)
        {
            HideEnemyHighLight();
            HideAllyHighLight();
            if (state == BattleState.PlayerUnitSelected)
                currentPlayerUnit.AllyHud.ShowAllyBuffTargetHover(true);
        }
    }
    void ShowEnemyHighLight(bool show = true)
    {
        for (int i = 0; i < enemyBattleUnits.Count; i++)
        {
            if (enemyBattleUnits[i].EnemyHud.gameObject.active)
                enemyBattleUnits[i].EnemyHud.ShowSelection();
        }
    }
    void HideEnemyHighLight()
    {
        for (int i = 0; i < enemyBattleUnits.Count; i++)
        {
            if (enemyBattleUnits[i].EnemyHud.gameObject.active)
                enemyBattleUnits[i].EnemyHud.HideSelection();
        }
    }
    void ShowAllyHighLight()
    {
        for (int i = 0; i < playerBattleUnits.Count; i++)
        {
            if (playerBattleUnits[i].AllyHud.gameObject.active)
                playerBattleUnits[i].AllyHud.ShowAllyBuffTarget();
        }
    }
    void HideAllyHighLight()
    {
        for (int i = 0; i < playerBattleUnits.Count; i++)
        {
            if (playerBattleUnits[i].AllyHud.gameObject.active)
                playerBattleUnits[i].AllyHud.HideSelection();
        }
    }


    // ENEMIES INTERACTION
    public void EnemyHover(int enemy)
    {
        if (state == BattleState.PlayerAbilitySelected)
        {
            var ability = currentPlayerUnit.Unit.Abilities[currentAbilityIndex];
            if (enemyUnits[enemy].isDead == false)
            {// 1
                if (ability.Base.Category == AbilityCategory.Magical || ability.Base.Category == AbilityCategory.Physical)
                {// 2
                    if (abilityType == AbilityTarget.SingleFoe)
                    {
                        int damage = enemyUnits[enemy].PreviewDamage(ability, currentPlayerUnit.Unit);
                        enemyBattleUnits[enemy].EnemyHud.ShowSelectionHover();
                        enemyBattleUnits[enemy].EnemyHud.ShowHPPreview(damage);
                    }
                    else if (abilityType == AbilityTarget.AllFoe)
                    {
                        for (int e =0; e < enemyUnits.Count; e++)
                        {
                            if (enemyBattleUnits[e].Unit.isDead == false)
                            {
                                int damage = enemyUnits[e].PreviewDamage(ability, currentPlayerUnit.Unit);
                                enemyBattleUnits[e].EnemyHud.ShowSelectionHover();
                                enemyBattleUnits[e].EnemyHud.ShowHPPreview(damage);
                            }
                        }
                    }
                }// 2
                else if (ability.Base.Category == AbilityCategory.Status)
                {// 3
                    if (abilityType == AbilityTarget.SingleFoe)
                        enemyBattleUnits[enemy].EnemyHud.ShowSelectionHover();
                    else if (abilityType == AbilityTarget.AllFoe)
                    {
                        for (int e = 0; e < enemyUnits.Count; e++)
                        {
                            if (enemyBattleUnits[e].Unit.isDead == false)
                                enemyBattleUnits[e].EnemyHud.ShowSelectionHover();
                        }
                    }
                }// 3
            }// 1
        }
    }
    public void EnemyHoverExit(int enemy)
    {
        if (state == BattleState.PlayerAbilitySelected)
        {
            if (abilityType == AbilityTarget.SingleFoe)
            {
                enemyBattleUnits[enemy].EnemyHud.ShowSelectionHoverExit();
                enemyBattleUnits[enemy].EnemyHud.HideHPPreview();
            }
            else if (abilityType == AbilityTarget.AllFoe)
            {
                for (int e = 0; e < enemyUnits.Count; e++)
                {
                    if (enemyBattleUnits[e].Unit.isDead == false) {
                        enemyBattleUnits[e].EnemyHud.ShowSelectionHoverExit();
                        enemyBattleUnits[e].EnemyHud.HideHPPreview();
                    }
                }
            }
        }
    }
    public void EnemyClicked(int enemy)
    {
        if (state == BattleState.PlayerAbilitySelected)
        {
            var ability = currentPlayerUnit.Unit.Abilities[currentAbilityIndex];
            if (ability.MPCost > currentPlayerUnit.Unit.MP || ability.HPCost > currentPlayerUnit.Unit.HP)
            {
            }
            else
            {
                if (abilityType == AbilityTarget.SingleFoe || abilityType == AbilityTarget.AllFoe)
                {
                    currentEnemyUnit = enemy;
                    if (abilityType == AbilityTarget.SingleFoe)
                    {
                        enemyBattleUnits[currentEnemyUnit].EnemyHud.ShowSelectionHoverExit();
                        HideEnemyHighLight();
                    }
                    else if (abilityType == AbilityTarget.AllFoe)
                    {
                        for (int e = 0; e < enemyUnits.Count; e++)
                            if (enemyBattleUnits[e].Unit.isDead == false)
                            {
                                enemyBattleUnits[e].EnemyHud.ShowSelectionHoverExit();
                                HideEnemyHighLight();
                            }
                    }

                    if (enemyUnits[currentEnemyUnit].isDead == false)
                        PerformPlayerAbility(currentPlayerUnit, enemyBattleUnits[currentEnemyUnit]);
                }
            }
            
        }
        else if (state == BattleState.PlayerUnitSelected)
        {
            unitToDisplayInfo = enemyBattleUnits[enemy];
            StartCoroutine(unitInfoDisplay.SetUp(enemyUnits[enemy]));
            state = BattleState.DisplayUnitInfo;
        }
    }


    // RUN CLICKED
    public void RunClicked()
    {
        if (state == BattleState.PlayerUnitSelected)
            TryToEscape();
    }

    // BACKGROUND MOUSE INTERACTION
    public void backgroundClicked()
    {
        if (state == BattleState.PlayerAbilitySelected)
        {
            state = BattleState.PlayerUnitSelected;
            unitInformation.ResetDescription();
            unitInformation.DeHighlightAA();
            unitInformation.ResetCosts();
            currentPlayerUnit.AllyHud.hidePreview();
        }

        ShowSelection();
    }


    // KEY BOARD INPUT
    void HandleUnitInfoDisplay()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown("1"))
        {
            if (playerUnits.Count > 0)
            {
                StartCoroutine(unitInfoDisplay.SetUp(playerUnits[0]));
                state = BattleState.DisplayUnitInfo;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown("2"))
        {
            if (playerUnits.Count > 1)
            {
                StartCoroutine(unitInfoDisplay.SetUp(playerUnits[1]));
                state = BattleState.DisplayUnitInfo;
            }
        }
        else if ((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown("3")))
        {
            if (playerUnits.Count > 2)
            {
                StartCoroutine(unitInfoDisplay.SetUp(playerUnits[2]));
                state = BattleState.DisplayUnitInfo;
            }
        }
    }
    void HandleEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(1))
        {
            if (state == BattleState.DisplayUnitInfo)
            {
                StartCoroutine(unitInfoDisplay.HideUnitInfo());
                state = BattleState.PlayerUnitSelected;
            }
            else if (state == BattleState.PlayerAbilitySelected)
            {
                backgroundClicked();
            }
        }
    }

}
