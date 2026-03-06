using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MenuStartGame() {
        SceneManager.LoadScene(1); // this is the SampleScene aka the actual game world
    }

    public void MenuExitGame() {
        Application.Quit();
    }
}
