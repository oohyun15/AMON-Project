﻿/****************************************
 * GameManager.cs
 * 제작: 조예진
 * 게임 클리어 조건 확인 스크립트
 * (19.07.30)  게임 시작 버튼을 누르면 UI(조이스틱 및 인터렉션 버튼)를 활성화함
 * (19.07.30)  데이터 로드 및 저장 부분 추가
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.26
 * 수정일자: 19.07.30
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 게임 클리어 시 보상 수준
    public enum ClearState
    {
        high,
        mid,
        low
    }

    // 게임 매니저 싱글톤
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (instance == null)
                {
                    Debug.LogError("There's no active GameManager object");
                }
            }

            return instance;
        }
    }

    public GameObject injuredParent;
    public AmonController player;

    [Header("Game State")]
    [SerializeField]
    private int leftInjured;
    public float timeLimit;                     // 제한 시간, 초 단위

    [Header("Clear Requirements")]              // 구조되지 않고 남아 있는 부상자 인원 기준
    public int maxLeftToMiddleCondition;        // 중간 보상 최대 인원
    public int maxLeftToLowCondition;           // 최하 보상 최대 인원

    [Header("UI")]
    public Text leftTimeText;
    public GameObject[] UI;                     // (용현) 0: Joystick, 1: Interaction

    private float time;                         // 남은 시간, 초 단위
    private bool gameOver;

    private IEnumerator timeCheckCoroutine;


    // 스테이지 데이터 변수
    private string stageDataPath = "Data/stage_data";       // 스테이지 데이터 csv 파일 경로
    private List<Dictionary<string, object>> stageData;

    private string sceneName;
    private int dataIndex;

    /* PlayerPrefs 저장 키
     * Money - 플레이어 소지 돈
     * Honor - 플레이어 소지 명예
     */

    private void Start()
    {
        LoadStageData();

        ShowPlayerInfo();

        gameOver = false;

        leftInjured = injuredParent.transform.childCount;

        time = timeLimit;

        SetTimeText(timeLimit);

        // (용현) UI 비활성화
        foreach (GameObject ui in UI) ui.SetActive(false);
    }

    // 스테이지 데이터 로드
    private void LoadStageData()
    {
        dataIndex = -1;

        sceneName = SceneManager.GetActiveScene().name;

        stageData = CSVReader.Read(stageDataPath);

        for (int i = 0; i < stageData.Count; i++)
        {
            if (stageData[i]["sceneName"].ToString() == sceneName)
            {
                dataIndex = i;

                break;
            }
        }

        if (dataIndex == -1) Debug.LogError("스테이지 데이터 로드 실패");
        else Debug.Log("스테이지 데이터 로드 완료");

        maxLeftToLowCondition = System.Convert.ToInt32(stageData[dataIndex]["maxLeftTolow"]);
        maxLeftToMiddleCondition = System.Convert.ToInt32(stageData[dataIndex]["maxLeftTomid"]);
    }

    private void ShowPlayerInfo()
    {
        Debug.Log("Player Info. Money = " + PlayerPrefs.GetInt("Money", 0) +
            ", Honor = " + PlayerPrefs.GetInt("Honor", 0));
    }

    public void StartGame(GameObject startButton)
    {
        startButton.SetActive(false);

        // (용현) UI 활성화
        foreach (GameObject ui in UI) ui.SetActive(true);
        
        timeCheckCoroutine = CheckTime();

        StartCoroutine(timeCheckCoroutine);
    }

    public void StopGame()
    {
        StopCoroutine(timeCheckCoroutine);
    }

    private void CheckLeftInjured()
    {
        leftInjured = injuredParent.transform.childCount;
    }

    // 게임 클리어 여부와 보상 수준 확인
    public void CheckGameClear()
    {
        CheckLeftInjured();

        // 플레이어 탈출 여부 확인
        bool isPlayerEscaped = player.IsEscaped;

        // 완전 게임 클리어 (최고 보상)
        if (leftInjured == 0 && isPlayerEscaped)
        {
            gameOver = true;

            GameClear(ClearState.high);
        }
        // 시간이 다 된 경우 - 중, 하위 게임 클리어 혹은 게임 오버
        else if (time <= 0) 
        {
            gameOver = true;

            if (isPlayerEscaped)
            {
                // 게임 클리어 시 (플레이어 빠져 나옴)
                if (leftInjured <= maxLeftToMiddleCondition)
                    GameClear(ClearState.mid);

                else if (leftInjured <= maxLeftToLowCondition)
                    GameClear(ClearState.low);

                // 최소 인원 구하지 못한 경우 게임 오버
                else GameOver();
            }
            else
            {
                // 게임 오버 시 (플레이어 빠져 나오지 못함)
                GameOver();
            }
        }
    }

    // 게임 클리어시
    private void GameClear(ClearState state)
    {
        Debug.Log("Game Clear - Reward : " + state.ToString());

        int money = 0, honor = 0;

        money = System.Convert.ToInt32(stageData[dataIndex][state.ToString() + "RewardMoney"]);
        honor = System.Convert.ToInt32(stageData[dataIndex][state.ToString() + "RewardHonor"]);

        Debug.Log("Reward - Money : " + money + ", Honor : " + honor);

        SaveGameResult(money, honor);

        StopGame();
    }

    // 게임 클리어 실패시
    private void GameOver()
    {
        Debug.Log("game over");

        StopGame();
    }

    // 플레이어 획득 보상 저장
    private void SaveGameResult(int money, int honor)
    {
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) + money);
        PlayerPrefs.SetInt("Honor", PlayerPrefs.GetInt("Honor", 0) + honor);

        ShowPlayerInfo();
    }

    private IEnumerator CheckTime()
    {
        while ((time -= Time.deltaTime) > 0 && !gameOver)
        {
            SetTimeText(time);

            yield return null;
        }

        SetTimeText(time);

        CheckGameClear();
    }

    private void SetTimeText(float setTime)
    {
        if (setTime < 0) setTime = 0;

        leftTimeText.text = "남은 시간 : " + setTime.ToString("00.00");
    }
}
