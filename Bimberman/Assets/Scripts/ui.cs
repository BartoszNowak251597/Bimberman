using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ui : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button tryAgainBtn;
    public Button looserBtn;

    public LevelLoader loader;

    void Awake()
    {

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tryAgainBtn.onClick.AddListener(TryAgain);
        looserBtn.onClick.AddListener(Looser);
    }

    

    void TryAgain()
    {
        //SceneManager.LoadScene("Base");
        loader.LoadLevel();
    }
    void Looser()
    {
        Application.Quit();
    }
}
