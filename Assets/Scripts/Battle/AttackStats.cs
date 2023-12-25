using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackStats : MonoBehaviour
{
    [SerializeField] Text PattackText;
    [SerializeField] Text PdefenseText;
    [SerializeField] Text MattackText;
    [SerializeField] Text MdefenseText;
    [SerializeField] UnitInformation unitInformation;

    Unit unit;

    public UnitInformation UnitInformation { get { return unitInformation; } }

    public void SetStats(Unit unit)
    {
        this.unit = unit;
        PattackText.text = "" + unit.PAttack;
        PdefenseText.text = "" + unit.PDefense;
        MattackText.text = "" + unit.MAttack;
        MdefenseText.text = "" + unit.MDefense;
        unitInformation.gameObject.SetActive(true);
    }
    public void SetReset()
    {
        unitInformation.gameObject.SetActive(false);
        PattackText.text = "" ;
        PdefenseText.text = "" ;
        MattackText.text = "" ;
        MdefenseText.text = "" ;
    }
    public void StatsHover()
    {
        unitInformation.DescriptionText.text = $"P Dmg : ({unit.Base.PAttack+unit.Base.PAttackGain*unit.Level}+{unit.StatBoostsFlat[Stat.PAttack]})*{unit.StatBoostsPercentage[Stat.PAttack]+1}"
                                                +$"\nM Dmg: ({unit.Base.MAttack+unit.Base.MAttackGain * unit.Level}+{unit.StatBoostsFlat[Stat.MAttack]})*{unit.StatBoostsPercentage[Stat.MAttack]+1}" 
                                                +$"\nP Amr    : ({unit.Base.PDefense+unit.Base.PDefenseGain * unit.Level}+{unit.StatBoostsFlat[Stat.PDefense]})*{unit.StatBoostsPercentage[Stat.PDefense]+1}" 
                                                +$"\nM Amr   : ({unit.Base.MDefense+unit.Base.MDefenseGain * unit.Level}+{unit.StatBoostsFlat[Stat.MDefense]})*{unit.StatBoostsPercentage[Stat.MDefense]+1}";
    }
    public void StatsHoverExit()
    {
        unitInformation.DescriptionText.text = "";
    }
}
