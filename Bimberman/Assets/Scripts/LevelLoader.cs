using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static string currentLoadedScene;
    public string sceneToLoad;

    private async Task OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            await SceneManager.UnloadSceneAsync(currentLoadedScene);

            await SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

            PlayerOrigin newOrigin = FindAnyObjectByType<PlayerOrigin>();

            if (newOrigin != null)
            {
                PlayerController.playerInstance.transform.position = newOrigin.transform.position;
                PlayerController.playerInstance.GetComponent<Rigidbody>().MovePosition(newOrigin.transform.position);
                PlayerController.playerInstance.transform.rotation = newOrigin.transform.rotation;
            }
        }
    }
}