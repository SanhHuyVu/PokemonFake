using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerController : MonoBehaviour, ISavable
{

    private Vector2 input;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if (character.IsMoving == false)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0; // remove diagonal movement

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            Interact();
    }

    void Interact()
    {
        // get the position of the tile player is facing
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        // get the position of the tile said above
        var interactPos = transform.position + facingDir;

        /* create a overlapCircle in interactPos tile with 0.3f to check if there are any object in the interactableLayer
         if there's any in the interactable object in this position the this function will return a collider */
        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer| GameLayers.i.TopLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
            collider.GetComponent<TopLayer>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 0.15f, GameLayers.i.TriggerableLayers);

        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnPlayertrigger(this);
                break;
            }
        }
    }


    // specifying how the posiotion of the player should be saved and restored
    public object CaptureState()
    {
        var saveData = new PlayerSaveData() {
            position = new float[] { transform.position.x, transform.position.y },
            units = GetComponent<UnitParty>().Units.Select(u => u.GetSaveData()).ToList()
        }; 
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        var pos = saveData.position;

        transform.position = new Vector3(pos[0], pos[1]);

        // restore party
        GetComponent<UnitParty>().Units = saveData.units.Select(s => new Unit(s)).ToList();
    }

    public Character Character => character;
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<UnitSaveData> units;
}