using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeStats : MonoBehaviour
{
    [SerializeField] Image health;
    [SerializeField] Image stamina;
    [SerializeField] Image mana;

    [SerializeField] Text healthText;
    [SerializeField] Text staminaText;
    [SerializeField] Text manaText;

    [SerializeField] Image healthPreview;
    [SerializeField] Image staminaPreview;
    [SerializeField] Image manaPreview;
    [SerializeField] Color HPPreviewColor;
    [SerializeField] Color MPPreviewColor;


    Color HPColor = new Color32(229, 103, 94, 255);
    Color SPColor = new Color32(94, 229, 134, 255);
    Color MPColor = new Color32(94, 101, 229, 255);

    public void SetHP(float hpNormalized, int hp, int MaxHp)
    {
        health.gameObject.transform.localScale = new Vector3(hpNormalized, 1f);
        healthText.text = $"{hp}/{MaxHp}";
        health.color = HPColor;
    }
    //mp
    public void SetMP(float mpNormalized, int mp, float MaxMp)
    {
        mana.gameObject.transform.localScale = new Vector3(mpNormalized, 1f);
        manaText.text = $"{mp}/{MaxMp}";
    }

    // preview damage/hpcost
    public void ShowDamagePreview(int damage, int currentHP, int maxHP)
    {
        health.color = HPPreviewColor;
        healthPreview.gameObject.SetActive(true);
        int newhp;
        if (currentHP - damage >= 0)
            newhp = currentHP - damage;
        else
            newhp = 0;
        healthPreview.gameObject.transform.localScale = new Vector3((float)newhp / maxHP, 1f);
        healthText.text = $"{newhp} (-{damage})/{maxHP}";
    }
    public void HideDamagePreview(int currentHP, int maxHP)
    {
        health.color = HPColor;
        health.gameObject.transform.localScale = new Vector3((float)currentHP / maxHP, 1f);
        healthPreview.gameObject.SetActive(false);
        healthText.text = $"{currentHP}/{maxHP}";
    }
    public void HideSPCostPreview(int currentSP, int maxSP)
    {
        stamina.color = SPColor;
        stamina.gameObject.transform.localScale = new Vector3((float)currentSP / maxSP, 1f);
        staminaPreview.gameObject.SetActive(false);
        staminaText.text = $"{currentSP}/{maxSP}";
    }
    // preview mp cost
    public void ShowMPCostPreview(int MPcost, int currentMP, int maxMP)
    {
        mana.color = MPPreviewColor;
        manaPreview.gameObject.SetActive(true);
        int newmp;
        if (currentMP - MPcost >= 0)
            newmp = currentMP - MPcost;
        else
            newmp = 0;
        manaPreview.gameObject.transform.localScale = new Vector3((float)newmp / maxMP, 1f);
        manaText.text = $"{newmp} (-{MPcost})/{maxMP}";
    }
    public void HideMPCostPreview(int currentMP, int maxMP)
    {
        mana.color = MPColor;
        mana.gameObject.transform.localScale = new Vector3((float)currentMP / maxMP, 1f);
        manaPreview.gameObject.SetActive(false);
        manaText.text = $"{currentMP}/{maxMP}";
    }
}
