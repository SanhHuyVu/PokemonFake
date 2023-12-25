using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInformation : MonoBehaviour
{

    [SerializeField] GameObject activeAbilityRow;
    [SerializeField] List<Image> activeAbilityImage;
    [SerializeField] List<Image> activeAbilityHolder;

    [SerializeField] Sprite AAHolderHighlight;
    [SerializeField] Sprite AAHolderNormal;

    [SerializeField] GameObject CostsHolder;
    [SerializeField] GameObject HPC;
    [SerializeField] GameObject SPC;
    [SerializeField] GameObject MPC;
    [SerializeField] Text HPCost;
    [SerializeField] Text SPCost;
    [SerializeField] Text MPCost;

    [SerializeField] GameObject passiveAbilityRow;
    [SerializeField] List<Image> passiveAbilityImage;
    [SerializeField] List<Image> passiveAbilityHolder;

    [SerializeField] GameObject description;
    [SerializeField] Text descriptionText;

    public Text DescriptionText { get { return descriptionText; } }

    public void SetActiveAbiblity(List<ActiveAbility> activeAbilities)
    {
        if (activeAbilities.Count == 0)
            activeAbilityRow.SetActive(false);
        else 
            for (int i = 0; i < activeAbilityHolder.Count; i++)
            {
                if (i < activeAbilities.Count)
                {
                    activeAbilityImage[i].sprite = activeAbilities[i].Base.Icon;
                    activeAbilityHolder[i].gameObject.SetActive(true);
                }
                else
                    activeAbilityHolder[i].gameObject.SetActive(false);
            }
        
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
        }else
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
