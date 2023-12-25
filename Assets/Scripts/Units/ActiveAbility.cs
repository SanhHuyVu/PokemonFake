using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveAbility
{
    public AbilityBase Base { get; set; }
    public int HPCost { get; set; }
    public int SPCost { get; set; }
    public int MPCost { get; set; }
    public Sprite Icon { get; set; }

    public ActiveAbility(AbilityBase pAbility)
    {
        Base = pAbility;
        HPCost = pAbility.HPCost;
        MPCost = pAbility.MPCost;
        Icon = pAbility.Icon;
    }

    public ActiveAbility(AAbilitySaveData saveData)
    {
        Base = AAbilitysDB.GetAAbilityByName(saveData.name);

        HPCost = saveData.hpCost;
        MPCost = saveData.mpCost;
        Icon = Base.Icon;
    }

    public AAbilitySaveData GetSaveData()
    {
        var saveData = new AAbilitySaveData() {
            name = Base.Name,
            hpCost = HPCost,
            mpCost = MPCost,
        };
        return saveData;
    }
}

[System.Serializable]
public class AAbilitySaveData
{
    public string name;
    public int hpCost;
    public int mpCost;
}