using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteFov : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayertrigger(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        GameController.Instance.OnEnterEliteView(GetComponentInParent<EliteController>());
    }
}
