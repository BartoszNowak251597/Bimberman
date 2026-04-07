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
    }

    public async Task LoadLevelAsync()
    {
        PlayerController player = PlayerController.playerInstance;
        if (player == null) return;

        Interaction interaction = player.GetComponent<Interaction>();
        if (interaction != null)
            interaction.ClearAvailable();

        if (!string.IsNullOrEmpty(currentLoadedScene))
        {
            await SceneManager.UnloadSceneAsync(currentLoadedScene);
        }

        await SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        Transform newOrigin = GameObject.Find("PlayerOrigin")?.transform;

        if (newOrigin != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            player.transform.position = newOrigin.position;
            player.transform.rotation = newOrigin.rotation;

            rb.position = newOrigin.position;
            rb.rotation = newOrigin.rotation;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            player.ResetAfterRespawn();

            CameraController cameraController = Camera.main != null
                ? Camera.main.GetComponent<CameraController>()
                : null;

            if (cameraController != null)
            {
                cameraController.lookAtPos = player.transform.position;
            }
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));

        currentLoadedScene = sceneToLoad;
    }

    public void LoadLevel()
    {
        StartCoroutine(LoadLevelCoroutine());
    }
}