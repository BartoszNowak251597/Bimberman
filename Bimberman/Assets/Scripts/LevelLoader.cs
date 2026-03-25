using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static string currentLoadedScene;
    public string sceneToLoad;
    public bool preserveAfterLoad;

    private IEnumerator LoadLevelCoroutine()
    {
        transform.parent = null;
        SceneManager.MoveGameObjectToScene(this.gameObject, PlayerController.playerInstance.gameObject.scene);

        Debug.Log(currentLoadedScene);

        var loadLevelTask = LoadLevelAsync();

        while (!loadLevelTask.IsCompleted)
        {
            yield return null;
        }

        if (!preserveAfterLoad)
        {
            Destroy(this.gameObject);
        }

        yield break;
    }

    public async Task LoadLevelAsync()
    {
        PlayerController.playerInstance.GetComponent<Interaction>().ClearAvailable();

        await SceneManager.UnloadSceneAsync(currentLoadedScene);

        await SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        Transform newOrigin = GameObject.Find("PlayerOrigin")?.transform;

        if (newOrigin != null)
        {
            PlayerController.playerInstance.transform.position = newOrigin.position;
            PlayerController.playerInstance.GetComponent<Rigidbody>().MovePosition(newOrigin.position);
            PlayerController.playerInstance.transform.rotation = newOrigin.rotation;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));

        currentLoadedScene = sceneToLoad;
    }

    public void LoadLevel()
    {
        StartCoroutine(LoadLevelCoroutine());
    }
}