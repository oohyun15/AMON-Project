/****************************************
 * GameManager.cs
 * 제작: 조예진
 * 게임 클리어 조건 확인 스크립트
 * (19.07.30)  게임 시작 버튼을 누르면 UI(조이스틱 및 인터렉션 버튼)를 활성화함
 * (19.07.30)  데이터 로드 및 저장 부분 추가
 * (19.08.01)  게임 초기값을 저장하는 함수 추가
 * (19.08.03)  다시하기 추가, 필드 오브젝트 자동으로 링크
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.26
 * 수정일자: 19.08.03
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

    // 게임 상태
    public enum GameState { Ready, Playing, Over, Clear }

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

    [System.Serializable]
    public class FieldObjects                   // 씬에 존재하는 오브젝트들
    {
        public string name;
        public GameObject[] go;
    }

    private DataManager dm;

    [Header("Game State")]
    [SerializeField]
    public GameState gameState = GameState.Ready;
    private int leftInjured;
    public float timeLimit;                     // 제한 시간, 초 단위

    [Header("Clear Requirements")]              // 구조되지 않고 남아 있는 부상자 인원 기준
    public int maxLeftToMiddleCondition;        // 중간 보상 최대 인원
    public int maxLeftToLowCondition;           // 최하 보상 최대 인원

    [Header("UI")]
    public Text leftTimeText;
    public GameObject startButton;
    public GameObject[] UI;                     // (용현) 0: Joystick, 1: Interaction

    [Header("Field Objects")]
    public AmonController player;
    public GameObject Inventory;                 // (태윤) Player 오브젝트에 상속된 아이템 받는 오브젝트 변수
    public GameObject Cam;
    public GameObject injuredParent;
    // public FieldObjects[] objects;
    public Dictionary<string, List<GameObject>> temp;


    private float time;                         // 남은 시간, 초 단위
    private bool gameOver;

    private IEnumerator timeCheckCoroutine;

    // 스테이지 데이터 변수

    /* PlayerPrefs 저장 키
     * Money - 플레이어 소지 돈
     * Honor - 플레이어 소지 명예
     */

    private void Awake()
    {
        // 스크립트 실행 순서로 인해 Awake에서 선언
        temp = new Dictionary<string, List<GameObject>>();
    }

    private void Start()
    {
        dm = DataManager.Instance;
  
        // 스테이지 데이터 설정
        maxLeftToMiddleCondition = dm.MaxLeftToMiddleCondition;
        maxLeftToLowCondition = dm.MaxLeftToLowCondition;

        dm.ShowPlayerInfo();

        InitGame();
    }

    private void FixedUpdate()
    {
        // Debug: Reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("RESTART");

            InitGame();
        }
    }

    public void InitGame()
    {
        // 실행되고 있는 모든 코루틴 종료
        StopAllCoroutines();

        gameOver = false;

        time = timeLimit;

        SetTimeText(timeLimit);

        // (용현) UI 비활성화
        foreach (GameObject ui in UI) ui.SetActive(false);

        // 필드 오브젝트 초기화 (자동 버전)
        foreach(var pair in temp)
        {
            Debug.Log(pair.Key);

            foreach(var value in pair.Value)
            {
                value.GetComponent<IReset>().SetInitValue(); 
            }
        }

        startButton.SetActive(true);

        /* 필드 오브젝트 초기화 (수동 버전)
        // 플레이어 관련 초기화
        player.SetInitValue();

        // 필드 오브젝트 초기화  IReset으로 컴포넌트 변경
        foreach (FieldObjects fo in objects)
        {
            foreach(GameObject go in fo.go)
            {
                go.GetComponent<IReset>().SetInitValue();
            }
        }
        */

        // 게임 상태 변경: Ready
        gameState = GameState.Ready;

        CheckLeftInjured();
    }

    public void StartGame()
    {
        startButton.SetActive(false);

        // (용현) UI 활성화
        foreach (GameObject ui in UI) ui.SetActive(true);

        timeCheckCoroutine = CheckTime();

        StartCoroutine(timeCheckCoroutine);

        gameState = GameState.Playing;
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

        int money = dm.GetStageReward(DataManager.RewardType.Money, state);
        int honor = dm.GetStageReward(DataManager.RewardType.Honor, state);

        Debug.Log("Reward - Money : " + money + ", Honor : " + honor);

        dm.SaveGameResult(money, honor);

        dm.ShowPlayerInfo();

        gameState = GameState.Clear;

        StopGame();
    }

    // 게임 클리어 실패시
    private void GameOver()
    {
        Debug.Log("game over");

        gameState = GameState.Over;

        StopGame();
    }

    // 플레이어 획득 보상 저장

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