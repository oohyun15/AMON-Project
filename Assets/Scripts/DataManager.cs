/****************************************
 * DataManager.cs
 * 제작: 조예진
 * 게임 데이터 로드 및 저장 스크립트
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.30
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public enum RewardType
    {
        Money,
        Honor
    }

    private static DataManager instance = null;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(DataManager)) as DataManager;

                if (instance == null)
                {
                    Debug.LogError("There's no active DataManager object");
                }
            }

            return instance;
        }
    }

    private string stageDataPath = "Data/stage_data";       // 스테이지 데이터 csv 파일 경로
    private List<Dictionary<string, object>> stageData;

    private string sceneName;
    private int dataIndex;

    // Stage Data
    private int maxLeftToLowCondition;
    public int MaxLeftToLowCondition { get { return maxLeftToLowCondition; } }

    private int maxLeftToMiddleCondition;
    public int MaxLeftToMiddleCondition { get { return maxLeftToMiddleCondition; } }

    private void Awake()
    {
        LoadStageData();
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

    public void SaveGameResult(int money, int honor)
    {
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) + money);
        PlayerPrefs.SetInt("Honor", PlayerPrefs.GetInt("Honor", 0) + honor);
    }


    public int GetStageReward(RewardType rewardType, GameManager.ClearState state)
    {
        int reward;

        reward = System.Convert.ToInt32(stageData[dataIndex][state.ToString() + "Reward" + rewardType.ToString()]);

        return reward;
    }

    public void ShowPlayerInfo()
    {
        Debug.Log("Player Info. Money = " + PlayerPrefs.GetInt("Money", 0) +
            ", Honor = " + PlayerPrefs.GetInt("Honor", 0));
    }

}
