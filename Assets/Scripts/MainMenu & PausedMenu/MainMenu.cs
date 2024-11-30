using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void play() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
    }

    public void quit() 
    {
        Application.Quit();
        Debug.Log("Player Has quit the game");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
