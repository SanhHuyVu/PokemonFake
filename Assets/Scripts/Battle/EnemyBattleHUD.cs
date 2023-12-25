using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattleHUD : MonoBehaviour
{
    [SerializeField] Image hud;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] LifeStats enemyUnitStats;
    [SerializeField] Image selectionImg;
    [SerializeField] Sprite red;
    [SerializeField] Sprite yellow;

    [SerializeField] List<Image> buffs;

    Unit _unit;

    public Image Hud { get { return hud; } }

    public void SetDataEnemies(Unit unit)
    {
        _unit = unit;
        nameText.text = unit.Base.Name;
        levelText.text = "Lvl: " + unit.Level;
        enemyUnitStats.SetHP((float)unit.HP / unit.MaxHP, unit.HP, unit.MaxHP);

        SetStatusIcons();
        _unit.OnStatusChanged += SetStatusIcons;
    }

    public void SetStatusIcons()
    {      
        if (_unit.Statuses == null)
        {
            foreach (var item in buffs)
            {
                item.gameObject.SetActive(false);
            }
        }
        else
        {
            //buffs[0].gameObject.SetActive(true);
            //buffs[0].sprite = _unit.Statuses.Icon;
            for (int i = 0; i < buffs.Count; i++)
            {
                if (i <= _unit.Statuses.Count - 1)
                {
                    buffs[i].gameObject.SetActive(true);
                    buffs[i].sprite = _unit.Statuses[i].Icon;
                }else
                    buffs[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateHP()
    {
        HideHPPreview();
        if (_unit.HPChanged == true)
        {
            enemyUnitStats.SetHP((float)_unit.HP / _unit.MaxHP, _unit.HP, _unit.MaxHP);
            _unit.HPChanged = false;
        }
    }
    public void UpdateMP()
    {
        enemyUnitStats.SetMP((float)_unit.MP / _unit.MaxMP, _unit.MP, _unit.MaxMP);
    }
    public void ShowSelection()
    {
        if(_unit.isDead == false)
            selectionImg.gameObject.SetActive(true);
    }
    public void HideSelection()
    {
        selectionImg.gameObject.SetActive(false);
    }
    public void ShowSelectionHover()
    {
        if (_unit.isDead == false)
            selectionImg.sprite = red;
    }
    public void ShowSelectionHoverExit()
    {
        if (_unit.isDead == false)
            selectionImg.sprite = yellow;
    }
    public void ShowHPPreview(int damage)
    {
        enemyUnitStats.ShowDamagePreview(damage, _unit.HP, _unit.MaxHP);
    }
    public void HideHPPreview()
    {
        enemyUnitStats.HideDamagePreview(_unit.HP, _unit.MaxHP);
    }
}
