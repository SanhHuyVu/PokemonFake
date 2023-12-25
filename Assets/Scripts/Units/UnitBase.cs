using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Units/Create new unit")]
public class UnitBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] UnitType type1;
    [SerializeField] UnitType type2;

    // base stats
    [Header("Base stats")]
    [SerializeField] int maxHP;
    [SerializeField] int maxMP;
    [SerializeField] int pAttack;
    [SerializeField] int pDefense;
    [SerializeField] int mAttack;
    [SerializeField] int mDefense;

    [SerializeField] int critRate;
    [SerializeField] float critDmg;

    // stats gain per lvl
    [Header("Stats gain per lvl")]
    [SerializeField] int maxHPGain;
    [SerializeField] int maxMPGain;
    [SerializeField] int pAttackGain;
    [SerializeField] int pDefenseGain;
    [SerializeField] int mAttackGain;
    [SerializeField] int mDefenseGain;

    [SerializeField] int actionSpeed;
    [SerializeField] int actionSpeedGain;

    [SerializeField] int expYield;

    [SerializeField] int currentSkillPoint;

    [SerializeField] List<ActiveAbilities> activeAbilities;

    public static int MaxNumOfAbilities { get; set; } = 4;

    public int GetExpForlevel(int level)
    {
        return 4 * (level * level * level) / 5;
    }

    public string Name { get { return name; } }
    public Sprite BackSprite { get { return backSprite; } }
    public Sprite FrontSprite { get { return frontSprite; } }
    public int MaxHP { get { return maxHP; } }
    public int MaxMP { get { return maxMP; } }
    public int PAttack { get { return pAttack; } }
    public int PDefense {  get { return pDefense; } }
    public int MAttack { get { return mAttack; } }
    public int MDefense {  get { return mDefense; } }
    public int MaxHPGain { get { return maxHPGain; } }
    public int MaxMPGain { get { return maxMPGain; } }
    public int PAttackGain { get { return pAttackGain; } }
    public int PDefenseGain { get { return pDefenseGain; } }
    public int MAttackGain { get { return mAttackGain; } }
    public int MDefenseGain { get { return mDefenseGain; } }
    public int CurrentSkillPoint { get { return currentSkillPoint; } }
    public int CritRate { get { return critRate; } }
    public float CritDmg { get { return critDmg; } }
    public int ActionSpeed { get { return actionSpeed; } }
    public int ActionSpeedGain { get { return actionSpeedGain; } }
    public List<ActiveAbilities> ActiveABilities { get { return activeAbilities; } }
    public int ExpYield => expYield;
}

[System.Serializable]
public class ActiveAbilities
{
    [SerializeField] AbilityBase abilityBase;
    [SerializeField] int lvlToLearn;

    public AbilityBase Base { get { return abilityBase; } }

    public int LevelToLearn { get { return lvlToLearn; } }

}

public enum Stat
{
    PAttack,
    PDefense,
    MAttack,
    MDefense,
    Speed,
    CritRate,
    CritDamage,

    // these 2 are not actual stats, only used to boost ability accuracy
    Accuracy,
    Evasion
}

public enum UnitType
{
    None,
    Normal,
    Player,
    Special,
    Summon,
    Flying,
    WildAnimal
}
