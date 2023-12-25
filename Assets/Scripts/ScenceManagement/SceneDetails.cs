using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;

    List<SavableEntity> savableEntities;
    public bool IsLoaded { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log($"Entered {gameObject.name}");

            LoadScene();
            GameController.Instance.SetCurrentScene(this);

            // load all connected scenes
            foreach (var scene in connectedScenes)
            {
                scene.LoadScene();
            }

            // unload scenes no longer connected
            var preScene = GameController.Instance.PrevScene;
            if (GameController.Instance.PrevScene != null)
            {
                var previouslyLoadedScenes = GameController.Instance.PrevScene.connectedScenes;
                foreach (var scene in previouslyLoadedScenes)
                {
                    // if the scene is not the currently loaded scene and if it not in the connected scene
                    // of the currently loaded scene then we unload them
                    if (!connectedScenes.Contains(scene) && scene != this)
                        scene.UnloadScene();
                }

                if (!connectedScenes.Contains(preScene))
                    preScene.UnloadScene();
            }
        }
    }

    public void LoadScene()
    {
        if (IsLoaded == false)
        {
            var oparation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;

            // only restore the entities after the async function complete
            oparation.completed += (AsyncOperation op) =>
            {
                savableEntities = GetSavableEntities();
                SavingSystem.i.RestoreEntityStates(savableEntities);
            };
        }
    }
    public void UnloadScene()
    {
        if (IsLoaded == true)
        {
            SavingSystem.i.CaptureEntityStates(savableEntities);

            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }

    List<SavableEntity> GetSavableEntities()
    {
        var currScene = SceneManager.GetSceneByName(gameObject.name);
        var savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currScene).ToList();
        return savableEntities;
    }
}
