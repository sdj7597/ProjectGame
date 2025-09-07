using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{
    public GameObject[] subUI;

    // Start is called before the first frame update

    public void MoveToScene()
    {
        Debug.Log("Game 으로 이동");
        SceneManager.LoadScene("Game");
        GameManager.Instance.timeLimit = 0;
        Time.timeScale = 1;
        
    }
    public void MainScene()
    {
        Debug.Log("main 으로 이동");
        SceneManager.LoadScene("Main");

    }


    public void OnClickOpenUI(int index)
    {
        subUI[index].gameObject.SetActive(true);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
