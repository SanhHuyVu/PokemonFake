using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StallDoor : MonoBehaviour, Interactable
{
    [SerializeField] Sprite closed;
    [SerializeField] Sprite open;

    bool close = true;
    SpriteRenderer spriteRenderer;

    int TopLayer;
    int Interactable;

    private void Start()
    {
        TopLayer = LayerMask.NameToLayer("TopLayer");
        Interactable = LayerMask.NameToLayer("Interactable");
    }

    public void Interact(Transform initiator)
    {
        if (close == true)
        {
            close = false;
            gameObject.GetComponent<SpriteRenderer>().sprite = open;
            gameObject.layer = TopLayer;
        }
        else
        {
            close = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = closed;
            gameObject.layer = Interactable;
        }
    }
}
