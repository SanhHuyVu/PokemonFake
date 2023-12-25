using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Sprite Icon { get; set; }
    // Func allows you to asign function that can return values unlike Action
    public Action<Unit> OnStart { get; set; }
    public Func<Unit, bool> OnBeforeTurn { get; set; }
    public Action<Unit> OnAfterTurn { get; set; } 
}
