using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Transform spawnpoint;
    [SerializeField] DestinationIdentifier destinationPortal;

    PlayerController player;

    public void OnPlayertrigger(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        Debug.Log("entered portal");
        StartCoroutine(Teleport()); 
    }

    Fader fader;
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator Teleport()
    {
        GameController.Instance.PauseGame(true);

        yield return fader.FadeIn(0.5f);


        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnpoint.position);

        yield return fader.FadeOut(0.5f);

        GameController.Instance.PauseGame(false);
    }

    public Transform SpawnPoint => spawnpoint;
}

public enum DestinationIdentifier { A, B, C, D, E, F, G}