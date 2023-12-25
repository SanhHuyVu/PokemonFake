using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitParty : MonoBehaviour
{
    [SerializeField] List<Unit> unitParty;

    private void Start()
    {
        foreach (var unit in unitParty)
        {
            unit.Init();
        }
    }

    public List<Unit> UnitsParty(){ return unitParty; }

    public List<Unit> Units { 
        get { return unitParty; }
        set { unitParty = value; }
    }

    public bool AddUnitToParty(Unit unit)
    {
        if (unitParty.Count <3)
        {
            unitParty.Add(unit);
            unit.Init();
            return true;
        }
        return false;
    }
}
