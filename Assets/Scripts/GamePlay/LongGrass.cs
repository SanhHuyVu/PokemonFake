using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayertrigger(PlayerController player)
    {
        // check for encounters
        if (UnityEngine.Random.Range(1, 101) <= 15)
        {
            player.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle();
            //Debug.Log("OnEncountered");
        }
    }
}
