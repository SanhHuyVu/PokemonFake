using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] LifeStats playerUnitStats;
    [SerializeField] Image body;
    [SerializeField] Image indicator;
    [SerializeField] Sprite arrowBlue;
    [SerializeField] Sprite arrowYellow;

    [SerializeField] GameObject expBar;

    [SerializeField] Sprite unitDefault;
    [SerializeField] Sprite unitActive;
    [SerializeField] Sprite unitActiveYellow;

    Unit _unit;
    public Sprite UnitDefault { get { return unitDefault; } }
    public Sprite UnitActive { get { return unitActive; } }
    public Sprite UnitActiveYellow { get { return unitActiveYellow; } }
    public void SetData(Unit unit)
    {
        _unit = unit;

        nameText.text = unit.Base.Name;
        SetLevel();

        playerUnitStats.SetHP((float)unit.HP / unit.MaxHP, unit.HP, unit.MaxHP);
        playerUnitStats.SetMP((float)unit.MP / unit.MaxMP, unit.MP, unit.MaxMP);
        SetExp();
    }
    public Image Body { get { return body; } }

    public void SetExp()
    {
        float normalizedExp = GetnormalizeExp();

        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public void SetLevel(int amount = 0)
    {
        if (amount == 1)
            levelText.text = "Lvl: <color=green>" + _unit.Level+ " + " + amount+ " Lvl</color>";
        else if (amount > 1)
            levelText.text = "Lvl: <color=green>" + _unit.Level+ " + " + amount+ " Lvls</color>";
        else
            levelText.text = "Lvl: " + _unit.Level;
    }

    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (reset == true) // reset the exp bar
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetnormalizeExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 2f);//.WaitForCompletion();
    }

    float GetnormalizeExp()
    {
        int currLevelExp = _unit.Base.GetExpForlevel(_unit.Level);
        int nextLevelExp = _unit.Base.GetExpForlevel(_unit.Level + 1);

        float normalizedExp = (float)(_unit.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public void UpdateHP()
    {
        if (_unit.HPChanged == true)
        {
            playerUnitStats.SetHP((float)_unit.HP / _unit.MaxHP, _unit.HP, _unit.MaxHP);
            _unit.HPChanged = false;
        }
    }
    public void UpdateMP()
    {
        playerUnitStats.SetMP((float)_unit.MP / _unit.MaxMP, _unit.MP, _unit.MaxMP);
    }

// hp cost preview
    public void ShowHPPreview(int damage)
    {
        playerUnitStats.ShowDamagePreview(damage, _unit.HP, _unit.MaxHP);
    }
    public void HideHPreview()
    {
        playerUnitStats.HideDamagePreview(_unit.HP, _unit.MaxHP);
    }

// mp cost preview
    public void ShowMPPreview(int MPCost)
    {
        playerUnitStats.ShowMPCostPreview(MPCost, _unit.MP, _unit.MaxMP);
    }
    public void HideMPreview()
    {
        playerUnitStats.HideMPCostPreview(_unit.MP, _unit.MaxMP);
    }
    public void hidePreview()
    {
        playerUnitStats.HideDamagePreview(_unit.HP, _unit.MaxHP);
        playerUnitStats.HideMPCostPreview(_unit.MP, _unit.MaxMP);
    }

// self/ally buff indicator
    public void ShowAllyBuffTarget()
    {
        Body.sprite = unitActiveYellow;
        indicator.sprite = arrowYellow;
        indicator.gameObject.SetActive(true);
    }
    public void ShowAllyBuffTargetHover(bool highlightBodyOnly = false)
    {
        Body.sprite = unitActive;
        if (highlightBodyOnly == false)
        {
            indicator.sprite = arrowBlue;
            indicator.gameObject.SetActive(true);
        }
    }
    public void HideSelection()
    {
        Body.sprite = unitDefault;
        indicator.sprite = arrowBlue;
        indicator.gameObject.SetActive(false);
    }
}
