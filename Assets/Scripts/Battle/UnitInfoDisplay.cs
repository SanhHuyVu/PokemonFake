using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnitInfoDisplay : MonoBehaviour
{
    [SerializeField] Text unitName;
    [SerializeField] Text unitLvl;
    [SerializeField] Image unitImg;
    [SerializeField] LifeStats HPStats;
    [SerializeField] UnitInformation info;
    [SerializeField] CanvasGroup unitInfoCG;

    [SerializeField] AttackStats attackStats;
    [SerializeField] Text descriptionText;

    [SerializeField] GameObject CostsHolder;
    [SerializeField] GameObject HPC;
    [SerializeField] GameObject SPC;
    [SerializeField] GameObject MPC;
    [SerializeField] Text HPCost;
    [SerializeField] Text SPCost;
    [SerializeField] Text MPCost;

    [SerializeField] Sprite AAHolderHighlight;
    [SerializeField] Sprite AAHolderNormal;

    [SerializeField] List<Image> activeAbilityHolder;

    public Text DescriptionText { get { return descriptionText; } }
    public IEnumerator SetUp(Unit unit)
    {
        yield return new WaitForSeconds(0.21f);
        unitName.text = unit.Base.Name;
        unitLvl.text = "" + unit.Level;
        unitImg.sprite = unit.Base.FrontSprite;

        HPStats.SetHP((float)unit.HP / unit.MaxHP, unit.HP, unit.MaxHP);
        HPStats.SetMP((float)unit.MP / unit.MaxMP, unit.MP, unit.MaxMP);

        info.SetActiveAbiblity(unit.Abilities);
        attackStats.SetStats(unit);

        unitInfoCG.gameObject.SetActive(true);
        unitInfoCG.DOFade(1f, 0.2f);
    }

    public IEnumerator HideUnitInfo()
    {
        unitInfoCG.DOFade(0f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        unitInfoCG.gameObject.SetActive(false);
    }
    public void HideUnitInfoInstant()
    {
        unitInfoCG.DOFade(0f, 0f);
        unitInfoCG.gameObject.SetActive(false);
    }

    public void HighlightAA(int ability)
    {
        activeAbilityHolder[ability].sprite = AAHolderHighlight;
    }
    public void DeHighlightAA()
    {
        for (int i = 0; i < activeAbilityHolder.Count; i++)
        {
            activeAbilityHolder[i].sprite = AAHolderNormal;
        }
    }
    public void SetActiveDescription(ActiveAbility ability)
    {
        descriptionText.text = ability.Base.Description;
    }
    public void ResetDescription()
    {
        descriptionText.text = "";
    }
    public void SetCosts(ActiveAbility ability)
    {
        CostsHolder.SetActive(true);
        if (ability.HPCost > 0)
        {
            HPC.SetActive(true);
            HPCost.text = "" + ability.HPCost;
        }
        else
            HPC.SetActive(false);

        if (ability.SPCost > 0)
        {
            SPC.SetActive(true);
            SPCost.text = "" + ability.SPCost;
        }
        else
            SPC.SetActive(false);

        if (ability.MPCost > 0)
        {
            MPC.SetActive(true);
            MPCost.text = "" + ability.MPCost;
        }
        else
            MPC.SetActive(false);
    }
    public void ResetCosts()
    {
        CostsHolder.SetActive(default);
        HPCost.text = "";
        SPCost.text = "";
        MPCost.text = "";
    }
}
