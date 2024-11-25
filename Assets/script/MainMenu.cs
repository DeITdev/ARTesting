using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void GameMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }
    public void ARGame()
    {
        SceneManager.LoadSceneAsync("AR Game");
    }
    public void ARGame2()
    {
        SceneManager.LoadSceneAsync("AR Game 2");
    }
    public void ARImageTracking()
    {
        SceneManager.LoadSceneAsync("AR Image Tracking");
    }
    public void ARAPIData()
    {
        SceneManager.LoadSceneAsync("AR API Data");
    }
    public void APIDataScada()
    {
        SceneManager.LoadSceneAsync("API Data Scada");
    }
    public void TrainerKit()
    {
        SceneManager.LoadSceneAsync("Trainer Kit");
    }
    public void ObjectView()
    {
        SceneManager.LoadSceneAsync("Object View");
    }

}
