using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    //[SerializeField] List<List<Unit>> enemyUnitList;
    [SerializeField] List<ListWrapper> enemyUnitList;

    public ListWrapper GetRandomEnemyUnitList()
    {
        var enemies = enemyUnitList[Random.Range(0, enemyUnitList.Count)];
        foreach (var unit in enemies.List)
        {
            unit.Init();
        }
        return enemies;
    }
}

[System.Serializable]
public class ListWrapper
{
    public List<Unit> enemyUnits;

    public List<Unit> List
    {
        get { return enemyUnits; }
    }
}
