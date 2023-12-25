using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortBySpeed : IComparer<BattleUnit>
{
    public int Compare(BattleUnit x, BattleUnit y)
    {
        return x.Unit.ActionSP.CompareTo(y.Unit.ActionSP);
    }
}
