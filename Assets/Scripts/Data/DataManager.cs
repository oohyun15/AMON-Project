/****************************************
 * DataManager.cs
 * 제작: 조예진
 * 게임 데이터 로드 및 저장 스크립트
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * (19.08.02) 유저 데이터 xml에서 불러오고 저장하는 부분 추가
 * (19.08.03) 미니맵 스프라이트 불러오는 부분 추가
 * (19.08.25) 클리어 횟수 추가
 * (19.10.02) SaveGameResult 시 유저데이터 읽어옴
 * 작성일자: 19.07.30
 * 수정일자: 19.10.02
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

    private readonly string stageDataPath = "Data/stage_data";       // 스테이지 데이터 csv 파일 경로
    private List<Dictionary<string, object>> stageData;

    public List<int> stressData = new List<int> { 25, 15, 10, 5, 0, -20 };

    private string sceneName;
    public string SceneName { get { return sceneName; } }
    private int dataIndex;
    public int DataIndex
    {
        get { return dataIndex; }
    }

    // Stage Data

    public Sprite minimap;

    public int totalInjuredCount = 5;

    private int maxLeftToLowCondition;
    public int MaxLeftToLowCondition { get { return maxLeftToLowCondition; } }

    private int maxLeftToMiddleCondition;
    public int MaxLeftToMiddleCondition { get { return maxLeftToMiddleCondition; } }

    private UserDataIO.User user;

    private void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;

        LoadStageData();
    }

    // 스테이지 데이터 로드
    private void LoadStageData()
    {
        dataIndex = -1;

        Debug.Log("Scene name : " + sceneName);

        stageData = CSVReader.Read(stageDataPath);

        for (int i = 0; i < stageData.Count; i++)
        {
            if (stageData[i]["sceneName"].ToString() == sceneName)
            {
                dataIndex = i;

                break;
            }
        }

        if (dataIndex == -1)
        {
            Debug.Log("스테이지 데이터 로드 실패 - (임시)minimap 데이터로 설정");
            dataIndex = 0;
            sceneName = "minimap";
        }
        else Debug.Log("스테이지 데이터 로드 완료");

        // 미니맵 프리뷰 스프라이트 불러오기 - 안될 경우 이미지의 텍스쳐 타입 Sprite인지 확인
        minimap = Resources.Load<Sprite>("Minimap/" + sceneName);

        totalInjuredCount = System.Convert.ToInt32(stageData[dataIndex]["total"]);
        maxLeftToLowCondition = totalInjuredCount - System.Convert.ToInt32(stageData[dataIndex]["low"]);
        maxLeftToMiddleCondition = totalInjuredCount - System.Convert.ToInt32(stageData[dataIndex]["mid"]);
    }

    public void SaveGameResult(int money, int honor)
    {
        user = UserDataIO.ReadUserData();

        // (19. 08. 02) xml 이용한 저장
        user.honor += honor;
        user.money += money + user.addReward;

        // (19.08.25) 클리어 횟수 증가
        user.clearCount++;

        UserDataIO.WriteUserData(user);
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

    public string GetStage() { return sceneName; }
}
