using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    [SerializeField] Unit unitToGive;

    NPCStates state;
    float idleTimer = 0f;
    int currentpattern = 0;

    Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact(Transform initiator)
    {
       if (state == NPCStates.Idle)
       {
            state = NPCStates.Dialog;
            character.LookTowards(initiator.position);

            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, unitToGive, () =>
            {
                idleTimer = 0f;
                state = NPCStates.Idle;
            }));
       }
    }

    private void Update()
    {
        if (state == NPCStates.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                StartCoroutine(Walk());
            }
        }
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCStates.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentpattern]);

        if (transform.position != oldPos)
            currentpattern = (currentpattern + 1) % movementPattern.Count;

        state = NPCStates.Idle;
    }
}

public enum NPCStates { Idle, Walking, Dialog}
