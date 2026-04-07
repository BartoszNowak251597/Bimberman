using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    public string globalScene = "GlobalScene";
    public string baseScene = "Base_POV";

    public async void StartGame()
    {
        await SceneManager.LoadSceneAsync(globalScene, LoadSceneMode.Additive);
        await SceneManager.LoadSceneAsync(this.baseScene, LoadSceneMode.Additive);

        Scene baseScene = SceneManager.GetSceneByName(this.baseScene);
        if (baseScene.IsValid())
        {
            SceneManager.SetActiveScene(baseScene);
        }

        Scene menuScene = gameObject.scene;
        await SceneManager.UnloadSceneAsync(menuScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}