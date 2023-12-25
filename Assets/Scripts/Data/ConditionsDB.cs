using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public Sprite poisonSprite;
    public Sprite burnSprite;
    public Sprite freezeSprite;
    public Sprite paralyzeSprite;
    public Sprite sleepSprite;

    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
            condition.Icon = Resources.Load<Sprite>("Status_icon/" + conditionId);
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>() 
    {
        {
            ConditionID.poison,
            new Condition()
            {
                Name = "Poison",
                OnAfterTurn = (Unit unit) =>
                {
                    unit.UpdateHP(50);
                }
            }
        },
        {
            ConditionID.burn,
            new Condition()
            {
                Name = "Burn",
                OnAfterTurn = (Unit unit) =>
                {
                    unit.UpdateHP(15);
                }
            }
        },
        {
            ConditionID.freeze,
            new Condition()
            {
                Name = "Freeze",
                OnBeforeTurn = (Unit unit) =>
                {
                    if (Random.Range(1,5) == 1) // 1 out of 4 chance the unit will be unfrozen
                    {
                        unit.CureStatus(ConditionID.freeze);
                        return true;
                    }

                    return false;
                }
            }
        },
        {
            ConditionID.paralyze,
            new Condition()
            {
                Name = "Paralyze",
                OnStart = (Unit unit) =>
                {
                    // paralyze for 2 turns
                    unit.StatusTime = 2;
                },
                OnBeforeTurn = (Unit unit) =>
                {
                    if (unit.StatusTime <= 0)
                    {
                        unit.CureStatus(ConditionID.paralyze);
                        return true;
                    }
                    unit.StatusTime--;
                    return false;
                }
            }
        },
        {
            ConditionID.sleep,
            new Condition()
            {
                Name = "Sleep",
                OnStart = (Unit unit) =>
                {
                    // sleep for 1 - 3 turns
                    unit.StatusTime = Random.Range(1,4);
                },
                OnBeforeTurn = (Unit unit) =>
                {
                    if (unit.StatusTime <= 0)
                    {
                        unit.CureStatus(ConditionID.sleep);
                        return true;
	                }
                    unit.StatusTime--;
                    return false;
                }
            }
        }
    };
}

public enum ConditionID
{
    none, poison, burn, sleep, paralyze, freeze, stun
}
