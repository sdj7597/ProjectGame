using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class Mission
{
    // 퀘스트
    public float goalTIme;
    public int[] goalKillCount;
    public int goalObjectCount;
    public GameObject bossObj;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Mission[] mission = new Mission[2];
    public GameObject[] stages;
    public int currentStage;

    public int score = 0;
    public float timeLimit = 0;
    public bool isBossShow = false;

    public bool isStart = false;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    // Start is called before the first frame update
    private void Start()
    {
        Time.timeScale = 1;

        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isStart)
            return;
        timeLimit -= Time.deltaTime;
        if (timeLimit <= 0)
        {
            Debug.Log("게임 오버!");
            UIManager.Instance.GameOver();
        }
    }

    public IEnumerator StartGame()
    {
        yield return null;

        UIManager.Instance.ShowMsgBig("게임을 시작합니다.");

        yield return new WaitForSeconds(1f);

        StartStage(1);

        yield return new WaitForSeconds(.5f);

        PlayerManager.instance.SetUp();
    }

    public void AddScore(int value)
    {
        score += value;
    }

    public void StartStage(int stageIndex)
    {
        isStart = true;

        UIManager.Instance.ShowMsgBig(stageIndex + "스테이지 시작");
        currentStage = stageIndex;
        timeLimit = mission[currentStage - 1].goalTIme;
        switch (stageIndex)
        {
            case 0:
            case 1:
                stages[0].SetActive(true);
                stages[1].SetActive(false);
                break;
            case 2:
                stages[0].SetActive(false);
                stages[1].SetActive(true);
                break;
        }
    }

    public void NextStage()
    {
        switch (currentStage)
        {
            case 0:
            case 1:
                StartStage(2);
                break;
            case 2:
                SceneManager.LoadScene("Ranking");
                break;
        }
    }

    public void DestroyEnemy(int enemyGrade)
    {
        int index = currentStage - 1;

        mission[index].goalKillCount[enemyGrade]--;
        if (mission[index].goalKillCount[enemyGrade] <= 0)
            mission[index].goalKillCount[enemyGrade] = 0;

        CheckMission();
    }

    public void CheckMission()
    {
        int index = currentStage - 1;

        for (int i = 0; i < 4; i++)
        {
            if (mission[index].goalKillCount[0] == 0 &&
                mission[index].goalKillCount[1] == 0 &&
                mission[index].goalKillCount[2] == 0 &&
                mission[index].goalKillCount[3] == 0)
            {
                if (!isBossShow)
                {
                    UIManager.Instance.ShowMsgBig("미션 클리어!");
                    isBossShow = true;
                    Instantiate(mission[index].bossObj);
                }
            }
        }
    }


}
