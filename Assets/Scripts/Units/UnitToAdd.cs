using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitToAdd : MonoBehaviour
{
    [SerializeField] Unit unitToAdd;

    public Unit UnitToBeAdded { get { return unitToAdd; } }
}
