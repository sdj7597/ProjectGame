using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Text scoreText, timeLimitText, missionInfoText;

    public GameObject finalUI, gameOverUI;

    public GameObject msgPrefab, msgBigPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Update()
    {
        #region 업데이트 UI
        GameManager game = GameManager.Instance;

        if (!game.isStart)
            return;

        TimeSpan timeSpan = TimeSpan.FromSeconds(game.timeLimit);
        timeLimitText.text = "남은 시간 : " + string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

        scoreText.text = game.score.ToString();

        int index = game.currentStage - 1;

        missionInfoText.text = "";
        missionInfoText.text += "1등급 :" + game.mission[index].goalKillCount[1];
        missionInfoText.text += "\n2등급 :" + game.mission[index].goalKillCount[2];
        missionInfoText.text += "\n3등급 :" + game.mission[index].goalKillCount[3];
        missionInfoText.text += "\n오브젝트 :" + game.mission[index].goalObjectCount;
        #endregion

    }

    //공용 메세지 박스 출력
    public void ShowMsg(string msg)
    {
        GameObject MsgObj = Instantiate(msgPrefab);
        MsgObj.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = msg;
        Destroy(MsgObj, 1.5f);
    }

    public void ShowMsgBig(string msg)
    {
        GameObject MsgObjBig = Instantiate(msgBigPrefab);
        MsgObjBig.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = msg;
        Destroy(MsgObjBig, 1.5f);
    }

    public void ShowFinalUI()
    {
        finalUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }
}