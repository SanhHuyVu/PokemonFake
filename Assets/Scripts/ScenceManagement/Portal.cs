using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] Transform spawnpoint;
    [SerializeField] DestinationIdentifier destinationPortal;

    PlayerController player;

    public void OnPlayertrigger(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        Debug.Log("entered portal");
        StartCoroutine(SwitchScene()); 
    }

    Fader fader;
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject); // dont destroy the portal to execute all the code first

        GameController.Instance.PauseGame(true);

        yield return fader.FadeIn(0.5f);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnpoint.position);

        yield return fader.FadeOut(0.5f);

        GameController.Instance.PauseGame(false);

        Destroy(gameObject); // destroy the portal aftter excuting all the code
    }

    public Transform SpawnPoint => spawnpoint;
}