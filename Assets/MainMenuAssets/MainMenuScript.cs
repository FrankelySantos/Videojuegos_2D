using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public AudioSource start;
    public AudioSource quit;
  public void PlayGame()
    {
        start.Play();
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitGame()
    {
        quit.Play();
        Application.Quit();
    }
}
