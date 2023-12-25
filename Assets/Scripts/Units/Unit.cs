using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Unit
{
    [SerializeField] UnitBase _base;
    [SerializeField] int level;

    public UnitBase Base { get { return _base; } }
    public int Level { get { return level; } }

    public int Exp { get; set; }
    public int HP { get; set; }
    public int MP { get; set; }
    public bool isDead { get; set; }
    // damage taken
    public int damageTaken { get; set; }
    
    public bool HPChanged { get; set; }
    public List<ActiveAbility> Abilities { get; set; }
    /* Dictionary is like a list but withs keys, in this case Stat is key and int is value*/
    public Dictionary<Stat, float> Stats { get; private set; } 
    public Dictionary<Stat, float> StatBoostsPercentage { get; private set; } 
    public Dictionary<Stat, float> StatBoostsFlat { get; private set; } 
    public List<BuffID> CurrentBuffs { get; set; }
    public List<Condition> Statuses { get; private set; }
    public int StatusTime { get; set; }

    public event System.Action OnStatusChanged;
    public void Init()
    {
        CaculateStats();

        HP = MaxHP;
        MP = MaxMP;

        Exp = Base.GetExpForlevel(Level);

        isDead = false;

        Abilities = new List<ActiveAbility>();    
        foreach (var ability in Base.ActiveABilities)
        {
            if (Level >= ability.LevelToLearn)
            {
                Abilities.Add(new ActiveAbility(ability.Base));
            }
            if (Abilities.Count >= UnitBase.MaxNumOfAbilities)
                break;
        }

        ResetStatBoosts();
        Statuses = new List<Condition>();
    }

    public Unit(UnitSaveData saveData)
    {
        _base = UnitDB.GetUnitByName(saveData.name);

        HP = saveData.hp;
        MP = saveData.mp;
        level = saveData.level;
        Exp = saveData.exp;

        Abilities = saveData.Aabilities.Select(s => new ActiveAbility(s)).ToList();

        CaculateStats();

        ResetStatBoosts();
        Statuses = new List<Condition>();
    }

    public UnitSaveData GetSaveData()
    {
        var saveData = new UnitSaveData()
        {
            name = Base.name,
            hp = HP,
            mp = MP,
            level = Level,
            exp = Exp,
            Aabilities = Abilities.Select(a => a.GetSaveData()).ToList()
        };

        return saveData;
    }

    void CaculateStats(bool levelUp = false)
    {
        Stats = new Dictionary<Stat, float>();
        Stats.Add(Stat.PAttack, Mathf.FloorToInt(Base.PAttack + (Base.PAttackGain * Level)));
        Stats.Add(Stat.PDefense, Mathf.FloorToInt(Base.PDefense + (Base.PDefenseGain * Level)));
        Stats.Add(Stat.MAttack, Mathf.FloorToInt(Base.MAttack + (Base.MAttackGain * Level)));
        Stats.Add(Stat.MDefense, Mathf.FloorToInt(Base.MDefense + (Base.MDefenseGain * Level)));
        Stats.Add(Stat.Speed, Mathf.FloorToInt(Base.ActionSpeed + (Base.ActionSpeedGain * Level)));
        Stats.Add(Stat.CritRate, Base.CritRate);
        Stats.Add(Stat.CritDamage, Base.CritDmg);

        // non boostable stats
        MaxHP = Mathf.FloorToInt(Base.MaxHP + (Base.MaxHPGain * Level));
        MaxMP = Mathf.FloorToInt(Base.MaxMP + (Base.MaxMPGain * Level));
        if (levelUp == true)
        {
            int hpChange = MaxHP - HP;
            int mpChange = MaxMP - MP;
            HP += hpChange;
            MP += mpChange;
        }
    }

    void ResetStatBoosts()
    {
        StatBoostsPercentage = new Dictionary<Stat, float>()
        {
            {Stat.PAttack, 0 },
            {Stat.PDefense, 0 },
            {Stat.MAttack, 0 },
            {Stat.MDefense, 0 },
            {Stat.Speed, 0 },
            {Stat.CritRate, 0 },
            {Stat.CritDamage, 0 },
            {Stat.Accuracy, 0 },
            {Stat.Evasion, 0 }
        };
        StatBoostsFlat = new Dictionary<Stat, float>()
        {
            {Stat.PAttack, 0 },
            {Stat.PDefense, 0 },
            {Stat.MAttack, 0 },
            {Stat.MDefense, 0 },
            {Stat.Speed, 0 },
            {Stat.CritRate, 0 },
            {Stat.CritDamage, 0 },
            {Stat.Accuracy, 0 },
            {Stat.Evasion, 0 }
        };
        CurrentBuffs = new List<BuffID>();
    }

    float GetStats(Stat stat)
    {
        float statVal = Stats[stat];

        statVal = statVal + StatBoostsFlat[stat];
        if (stat == Stat.Accuracy || stat == Stat.Evasion)
            statVal = statVal + statVal;
        else
            statVal = statVal + statVal * StatBoostsPercentage[stat];

        if (statVal < 0)
            statVal = 0;

        return statVal;
    }

    public void ApplieBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var value = statBoost.boostValue;
            bool newBuff = true;

            foreach (var ID in CurrentBuffs)
            {
                if (ID == statBoost.buffID)
                {
                    newBuff = false;
                }
            }

            if ( newBuff == true)
            {
                CurrentBuffs.Add(statBoost.buffID);
                if (statBoost.mulOrAdd == BoostValueCategory.PercentAmount)
                {
                    StatBoostsPercentage[stat] = StatBoostsPercentage[stat] + value;
                }
                else if (statBoost.mulOrAdd == BoostValueCategory.FlatAmount)
                {
                    StatBoostsFlat[stat] = StatBoostsFlat[stat] + value;
                }
            }

            //Debug.Log($"{stat} inscrease by {StatBoostsFlat[stat]} flat amount and {StatBoostsPercentage[stat]}%");
        }
    }

    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForlevel(level + 1))
        {
            ++level;
            CaculateStats(true);
            return true;
        }

        return false;
    }

    public ActiveAbilities GetLearnableAbility()
    {
        return Base.ActiveABilities.Where(x => Level == x.LevelToLearn).FirstOrDefault();
    }

    public void LevelUpAbility(ActiveAbilities abilityToLevel)
    {
        if (Abilities.Count > UnitBase.MaxNumOfAbilities)
            return;

        /*for (int i = 0; i < Abilities.Count; i++)
        {
            if (Abilities[i].Base == abilityToLevel.Base)
                return;
        }*/

        Abilities.Add(new ActiveAbility(abilityToLevel.Base));
    }

    // set up default stats
    public int PAttack { get { return (int)GetStats(Stat.PAttack); } }
    public int PDefense { get { return (int)GetStats(Stat.PDefense); } }
    public int MAttack { get { return (int)GetStats(Stat.MAttack); } }
    public int MDefense { get { return (int)GetStats(Stat.MDefense); } }
    public int ActionSP { get { return (int)GetStats(Stat.Speed); } }
    public int CritRate { get { return (int)GetStats(Stat.CritRate); } }
    public float CritDmg { get { return GetStats(Stat.CritDamage); } }
    
    // non boostable stats
    public int MaxHP { get; private set; }
    public int MaxSP { get; private set; }
    public int MaxMP { get; private set; }

    public DamageDetails TakeDamage(ActiveAbility ability, Unit attacker)
    {
        float criticalDmg = 1f;
        bool isCreated = false;
        if (Random.value * 100f <= /*attacker.critRate*/ 50f)
        {
            criticalDmg = attacker.CritDmg;
            isCreated = true;
        }

        var damageDetails = new DamageDetails()
        {
            IsCreated = isCreated,
            Critical = criticalDmg,
            IsDead = false
        };

        //  attack
        float attack;
        if (ability.Base.Category == AbilityCategory.Physical)
            attack = attacker.PAttack;
        else if (ability.Base.Category == AbilityCategory.Magical)
            attack = attacker.MAttack;
        else
            attack = 0;

        //  defense
        float defense;
        if (ability.Base.Category == AbilityCategory.Physical)
            defense = (PDefense + 100);
        else if (ability.Base.Category == AbilityCategory.Magical)
            defense = (MDefense + 70);
        else
            defense = 0;

        float b = (ability.Base.Power * ((float) attack) * criticalDmg) / defense;
        int damage = Mathf.FloorToInt(b);
        //Debug.Log($"{ability.Base.Power}*({(float)a})*{criticalDmg})/{def}={damage}");
        if (damage <= 1)
            damage = 1;

        UpdateHP(damage);

        return damageDetails;
        isDead = damageDetails.IsDead;
    }
    public int PreviewDamage(ActiveAbility ability, Unit attacker)
    {
        //  attack
        float attack;
        if (ability.Base.Category == AbilityCategory.Physical)
            attack = attacker.PAttack;
        else if (ability.Base.Category == AbilityCategory.Magical)
            attack = attacker.MAttack;
        else
            attack = 0;

        //  defense
        float defense;
        if (ability.Base.Category == AbilityCategory.Physical)
            defense = (PDefense + 100);
        else if (ability.Base.Category == AbilityCategory.Magical)
            defense = (MDefense + 70);
        else
            defense = 0;


        float b = (ability.Base.Power * ((float)attack)) / defense;
        int damage = Mathf.FloorToInt(b);
        //Debug.Log($"{ability.Base.Power}*({(float)a})*{criticalDmg})/{def}={damage}");
        if (damage <= 1)
            damage = 1;
        damageTaken = damage;

        return damage;
    }
    public void UpdateHP(int damage, bool isLethal = true)
    {
        damageTaken = damage;
        if (isLethal == true)
            HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        else
            HP = Mathf.Clamp(HP - damage, 1, MaxHP);
        HPChanged = true;
    }
    public void SetStatus(ConditionID conditionID)
    {
        bool isExisting = false;
        var status = ConditionsDB.Conditions[conditionID];
        // check if status exists
        foreach (var item in Statuses)
        {
            if (item.Id == status.Id)
                isExisting = true;
        }
        // if not then applie status
        if (isExisting == false)
        {
            status?.OnStart?.Invoke(this);// "this" is the current unit
            Statuses.Add(status);
        }

        OnStatusChanged?.Invoke();
    }
    public void CureStatus(ConditionID Id)
    {
        for (int i = 0; i < Statuses.Count; i++)
        {
            if (Statuses[i].Id == Id)
                Statuses.Remove(Statuses[i]);
        }
        OnStatusChanged?.Invoke();
    }
    public ActiveAbility GetRandomAbility()
    {
        // make list with abilities that have enough requiered resources
        var abilities = Abilities.Where(x => (x.MPCost <= MP || x.HPCost <= HP)).ToList();
        int r = Random.Range(0, abilities.Count);
        return abilities[r];
    }
    public bool OnBeforeTurn()
    {
        bool canRunTurn = true;
        if (Statuses != null)
        {
            for (int i = 0; i < Statuses.Count; i++)
            {
                if (Statuses[i]?.OnBeforeTurn != null)
                {
                    if (Statuses[i].OnBeforeTurn(this) == false)
                        canRunTurn = false;
                }
            }
        }

        return canRunTurn;
    }
    public void OnAfterTurn()
    {
        //Status?.OnAfterTurn?.Invoke(this);
        if (Statuses != null)
        {
            foreach (var status in Statuses)
            {
                status?.OnAfterTurn?.Invoke(this);
            }
        }
    }
    public void OnBattleOver()
    {
        ResetStatBoosts();
    }
}

public class DamageDetails
{
    public bool IsDead { get; set; }
    public float Critical { get; set; }
    public bool IsCreated { get; set; }

}
[System.Serializable]
public class UnitSaveData
{
    public string name;
    public int hp;
    public int mp;
    public int level;
    public int exp;
    public List<AAbilitySaveData> Aabilities;
}