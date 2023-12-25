using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Abilitie", menuName = "Units/Create new ability")]
public class AbilityBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] int power;
    [SerializeField] int mpCost;
    [SerializeField] int hpCost;
    [SerializeField] AbilityCategory category;
    [SerializeField] BuffType bType;
    [SerializeField] AbilityEffects effects;
    [SerializeField] AbilityTarget target;
    [SerializeField] Sprite icon;

    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public int Power { get { return power; } }
    public int MPCost { get { return mpCost; } }
    public int HPCost { get { return hpCost; } }
    public AbilityCategory Category { get { return category; } }
    public BuffType BType { get { return bType; } }
    public AbilityEffects Effects { get { return effects; } }
    public AbilityTarget Target { get { return target; } }
    public Sprite Icon { get { return icon; } }


}

[System.Serializable]
public class AbilityEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    public List<StatBoost> Boosts { get { return boosts; } }
    public ConditionID Status { get { return status; } }
}
[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public float boostValue;
    public BoostValueCategory mulOrAdd;
    public BuffID buffID;
}

public enum BoostValueCategory
{
    PercentAmount,
    FlatAmount
}

public enum AbilityCategory
{
    Physical,
    Magical,
    Status,
    None
}

public enum BuffType
{
    Buff,
    Debuff
}

public enum AbilityTarget
{
    SingleFoe,
    SingleAlly,
    AllFoe,
    AllAlly,
    Self
}

public enum BuffID
{
    // dmg up
    DmgUp_Self_Flat,
    DmgUp_Self_Percent,
    DmgUp_AllAlly_Flat,
    DmgUp_AllAlly_Percent,
    DmgUp_SingleAlly_Flat,
    DmgUp_SingleAlly_Percent,
    // dmg down
    DmgDown_AllFoe_Flat,
    DmgDown_AllFoe_Percent,
    DmgDown_SingleFoe_Flat,
    DmgDown_SingleFoe_Percent,
}
