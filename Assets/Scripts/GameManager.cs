/****************************************
 * GameManager.cs
 * 제작: 조예진
 * 게임 클리어 조건 확인 스크립트
 * (19.07.30)  게임 시작 버튼을 누르면 UI(조이스틱 및 인터렉션 버튼)를 활성화함
 * (19.07.30)  데이터 로드 및 저장 부분 추가
 * (19.08.01)  게임 초기값을 저장하는 함수 추가
 * (19.08.03)  다시하기 추가, 필드 오브젝트 자동으로 링크
 * (19.08.04)  UI(ItemSlots, Minimap), 게임 결과창 추가
 * (19.08.05)  버전 빌드 위해서 Data 관련 코드는 모두 주석처리함
 * (19.08.10)  아이템 슬롯 변수 추가 및 설정 버튼 추가
 * (19.08.11)  LeftInjured 변수 수정. Injured가 Exit 트리거에 나왔을 때 감소하도록 설정
 * (19.08.12)  leftInjured 변수 계산 방식 수정 - 부상자 오브젝트 리스트에서 활성화 여부 확인하도록 함
 * (19.08.20)  아이템 관련 코드 수정
 * (19.09.02)  인터렉션 버튼 아이템 이미지 추가
 * (19.09.15)  옵저버 추가
 * (19.09.22)  인게임 UI 수정
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.26
 * 수정일자: 19.09.22
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 게임 클리어 시 보상 수준
    public enum ClearState { high, mid, low }

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
    public class UISet
    {
        public string name;
        public GameObject UI;
    }

    private DataManager dm;

    [Header("Game State")]
    public GameState gameState = GameState.Ready;
    public int leftInjured;
    public float timeLimit;                     // 제한 시간, 초 단위

    [Header("Clear Requirements")]              // 구조되지 않고 남아 있는 부상자 인원 기준
    public int maxLeftToMiddleCondition;        // 중간 보상 최대 인원
    public int maxLeftToLowCondition;           // 최하 보상 최대 인원

    [Header("UI")]
    public Text leftTimeText;
    public Slider oxygenSlider;                 // (예진) 산소통 테스트
    public GameObject startButton;
    public GameObject settingsButton;
    public GameObject warningPanel;              // (예진) 탈출 버튼
    public UISet[] gameResultPanel;             // (예진) 게임 결과 패널 UI 접근 방식 변경
    public GameObject settings;
    public GameObject[] UI;                     // (용현) 0: Joystick, 1: Interaction, 2: ItemSlots, 3: Minimap
    public Image minimapPreview;
    public Image interactionImage;              // (용현) 인터렉션 아이템 이미지
    public Sprite[] itemImages;                 // (용현) 인터렉션 아이템 이미지 종류
                                                // 0: Axe, 1: Drink, 2: Coin(임시)

    [Header("Field Objects")]
    public AmonController player;
    public GameObject Inventory;                // (태윤) Player 오브젝트에 상속된 아이템 받는 오브젝트 변수
    public GameObject Cam;
    public GameObject injuredParent;
    public Dictionary<string, List<GameObject>> objects;
    public Animator resultCharacterAnimator;    // (예진) 게임 결과창에 렌더되는 캐릭터 오브젝트 애니메이터

    [Header("Observer")]
    public IObserver observer;

    [Header("Sprites")]
    public Sprite[] resultRescuerSprites;       // (예진) 결과 패널에 부상자 구조 여부 스프라이트 받아둠
                                                // 0 --> 구조 실패 / 1 --> 구조 성공


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
        objects = new Dictionary<string, List<GameObject>>();
    }

    private void Start()
    {
        dm = DataManager.Instance;

        // 스테이지 데이터 설정
        maxLeftToMiddleCondition = dm.MaxLeftToMiddleCondition;     // 중간 보상 최대 인원
        maxLeftToLowCondition = dm.MaxLeftToLowCondition;           // 최하 보상 최대 인원

        // 미니맵 프리뷰 스프라이트 불러와서 설정
        minimapPreview.sprite = dm.minimap;

        // 옵저버 추가
        

        // 게임 초기화
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

        // Debug: Settings
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Settings");

            if (settings.activeInHierarchy) SettingButton(false);

            else SettingButton(true);
        }
    }

    public void InitGame()
    {
        // 실행되고 있는 모든 코루틴 종료
        StopAllCoroutines();

        gameOver = false;

        time = timeLimit;

        int defaultTime = 60;

        // (19.09.11. 예진) 산소통 테스트 - debug 씬에 추가 시 if문 삭제
        // (19.09.21.) Debug 씬에 산소통 추가됨
        // 시간에 따라 산소통 크기 다르게 해줌
        // (19.09.26) 산소통 슬라이더 백그라운드 Sliced Sprite 처리하고 위치와 크기 조정하도록 함
        RectTransform rt = oxygenSlider.GetComponent<RectTransform>();
        RectTransform bgrt = oxygenSlider.transform.GetChild(0).GetComponent<RectTransform>();

        float temp = (timeLimit - defaultTime) * 10;

        rt.sizeDelta = new Vector2(rt.sizeDelta.x + temp, rt.sizeDelta.y);
        bgrt.sizeDelta = new Vector2(bgrt.sizeDelta.x + temp, bgrt.sizeDelta.y);
        
        bgrt.anchoredPosition = new Vector3(bgrt.anchoredPosition.x + temp / 2, bgrt.anchoredPosition.y, 0);

        oxygenSlider.maxValue = timeLimit;

        oxygenSlider.value = timeLimit;

        // 장착 아이템 효과 적용
        ApplyEquipItemEffect();

        SetTimeText(time);

        // (19.09.22) 아이템 슬롯 임시 비활성화
        /*
        // (19.08.10) 아이템 이미지가 있을 경우에만 아이템 이미지 활성화.
        // 추후에 아이템이 추가되고 조건들이 많아질 경우 함수로 빼놔야 할 듯
        for (int i = 0; i < UI[2].transform.childCount; i++)
        {
            Transform currentItem = UI[2].transform.GetChild(i);

            // 아이템 이미지가 있는지 확인
            if (currentItem.childCount == 2)
                currentItem.GetChild(0).gameObject.SetActive(true);
        }
        */

        // (용현) UI 비활성화
        foreach (GameObject ui in UI) ui.SetActive(false);
     
        // 필드 오브젝트 초기화 (자동 버전)
        foreach (var pair in objects)
        {
            Debug.Log(pair.Key + " " + pair.Value.Count);

            foreach (var value in pair.Value)
            {
                value.GetComponent<IReset>().SetInitValue();
            }
        }
        
        // 게임 결과창 비활성화
        gameResultPanel[0].UI.SetActive(false);

        // 결과창 캐릭터 렌더링 애니메이션 초기화
        resultCharacterAnimator.SetBool("Victory", false);
        resultCharacterAnimator.SetBool("Fail", false);


        // 세팅 버튼 비활성화
        settingsButton.SetActive(false);

        // 세팅창 비활성화
        settings.SetActive(false);

        startButton.SetActive(true);

        minimapPreview.gameObject.SetActive(true);

        // 게임 상태 변경: Ready
        gameState = GameState.Ready;

        // 초기 부상자 수 확인
        CheckLeftInjured();
    }

    private void ApplyEquipItemEffect()
    {
        int oxygenEffect = GetEquipedItemEffect("oxygen");
        int glovesEffect = GetEquipedItemEffect("gloves");

        time += oxygenEffect;
        player.damage = glovesEffect;
    }

    private int GetEquipedItemEffect(string item)
    {
        //int itemLv = PlayerPrefs.GetInt(item + "lv", 0);

        UserDataIO.User user = UserDataIO.ReadUserData();

        // 아이템 현재 레벨 불러오기
        // int lv = PlayerPrefs.GetInt(item + "lv", 0);
        int itemLv = 0;

        switch (item)
        {
            case "oxygen":
                itemLv = user.oxygenlv;
                break;

            case "gloves":
                itemLv = user.gloveslv;
                break;
        }

        int itemEffect = 0;

        if (itemLv != 0)
        {
            Dictionary<string, object> data = ItemDataManager.Instance.GetEquipItemData();

            object effectData = ((List<Dictionary<string, object>>)data[item])[itemLv - 1]["effect"];

            itemEffect = System.Convert.ToInt32(effectData);
        }

        return itemEffect;
    }

    public void StartGame()
    {
        startButton.SetActive(false);

        minimapPreview.gameObject.SetActive(false);

        // 세팅 버튼 활성화
        settingsButton.SetActive(true);

        // (용현) UI 활성화
        foreach (GameObject ui in UI) ui.SetActive(true);

        // (19.09.22) 아이템 슬롯 임시 비활성화
        UI[2].SetActive(false);

        timeCheckCoroutine = CheckTime();

        List<GameObject> injureds = new List<GameObject>();
        injureds.AddRange(objects["Serious"]);
        //injureds.AddRange(objects["Minor"]);

        // (예진) 부상자 시간 체크 시작
        foreach (GameObject i in injureds)
            i.GetComponent<Injured>().StartTimeCheck();

        StartCoroutine(timeCheckCoroutine);

        gameState = GameState.Playing;
    }

    public void StopGame()
    {
        // 제한 시간 체크 일시 중단
        StopCoroutine(timeCheckCoroutine);

        // 부상자 시간 제한 체크 일시 중단
        List<GameObject> injureds = new List<GameObject>();
        injureds.AddRange(objects["Serious"]);
        //injureds.AddRange(objects["Minor"]);

        foreach (GameObject i in injureds)
        {
            Injured injured = i.GetComponent<Injured>();

            if (!injured.isRescued)
                injured.StopTimeCheck();
        }
    }

    // (예진) 부상자 시간 제한 재개 부분 추가 위해 메소드 추가
    public void ResumeGame()
    {
        // 제한 시간 체크 재개
        StartCoroutine(timeCheckCoroutine);

        // 부상자 시간 체크 재개
        List<GameObject> injureds = new List<GameObject>();
        injureds.AddRange(objects["Serious"]);
        //injureds.AddRange(objects["Minor"]);

        foreach (GameObject i in injureds)
        {
            i.GetComponent<Injured>().StartTimeCheck();
        }
    }

    public int CheckLeftInjured()
    {
        leftInjured = 0;

        List<GameObject> injureds = new List<GameObject>();
        injureds.AddRange(objects["Serious"]);
        //injureds.AddRange(objects["Minor"]);

        foreach (GameObject i in injureds)
            if (i.activeInHierarchy)
                leftInjured++;
            
        return leftInjured;
    }

    // 게임 클리어 여부와 보상 수준 확인
    public void CheckGameClear()
    {
        CheckLeftInjured();

        gameOver = true;

        if (time <= 0) GameOver();

        else if (leftInjured == 0) GameClear(ClearState.high);

        else if (leftInjured <= maxLeftToMiddleCondition) GameClear(ClearState.mid);

        else if (leftInjured <= maxLeftToLowCondition) GameClear(ClearState.low);

        else GameOver();
        
     /*

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

     */
    }

    // (19.10.03 예진) 게임 클리어, 실패 공통 실행 부분 합침
    public void GameEnd()
    {
        int rescuedCount = dm.totalInjuredCount - leftInjured;
        int stress = dm.stressData[rescuedCount];

        // (19.08.25) 플레이 횟수 증가
        UserDataIO.User user = UserDataIO.ReadUserData();
        user.playCount++;
        UserDataIO.WriteUserData(user);

        // (19.10.03 예진) 피로도 업데이트
        if (!UserDataIO.ChangeUserStress(stress))
            Debug.Log("스트레스 100 초과, 게임 오버");
            
        // (19.09.15) 옵저버에게 user데이터 전달
        // user.NotifyObservers();

        // 조이스틱 멈춤
        JoystickController.instance.StopJoystick();

        // 애니메이션 멈춤
        player.AnimationIdle();

        // 세팅 버튼 비활성화
        settingsButton.SetActive(false);

        // (용현) UI 비활성화
        foreach (GameObject ui in UI) ui.SetActive(false);

    }

    // 게임 클리어시
    private void GameClear(ClearState state)
    {
        GameEnd();

        Debug.Log("Game Clear - Reward : " + state.ToString());

        int money = dm.GetStageReward(DataManager.RewardType.Money, state);
        int honor = dm.GetStageReward(DataManager.RewardType.Honor, state);

        Debug.Log("Reward - Money : " + money + ", Honor : " + honor);

        dm.SaveGameResult(money, honor);

        // (19.09.23.) 결과창 애니메이션 설정
        resultCharacterAnimator.SetBool("Victory", true);

        dm.ShowPlayerInfo();

        gameState = GameState.Clear;

        // 게임 결과창 활성화
        ShowGameResult(money, honor);

        StopGame();
    }

    // 게임 클리어 실패 시. (19.08.05) 플레이어가 장애물에 맞아 죽었을 시 접근하기 위해 public으로 변경
    public void GameOver()
    {
        GameEnd();

        if (gameState == GameState.Over)
        {
            Debug.Log("GameOver 중복 호출");

            return;
        }
        Debug.Log("game over");

        gameState = GameState.Over;

        // (19.09.23.) 결과창 애니메이션 설정
        resultCharacterAnimator.SetBool("Fail", true);

        // 게임 결과창 활성화
        ShowGameResult(0, 0);

        StopGame();
    }

    // (예진 19.08.05) 게임 결과 보여주는 UI 창 설정
    // (19.08.10) Find 함수 >> 게임 패널 UI 안의 텍스트, 이미지 미리 연결해 두고 사용하는 식으로 변경
    // (19.08.12) 탈출 못한 부상자도 초록색으로 바뀌는 것 수정
    private void ShowGameResult(int money, int honor)
    {
        GameObject panel = null;

        for (int i = 0; i < gameResultPanel.Length; i++)
        {
            switch (gameResultPanel[i].name)
            {
                // 게임 클리어 상태에 따라 텍스트와 배경 색 변경
                case "Result":
                    if (gameState == GameState.Over)
                    {
                        gameResultPanel[i].UI.transform.GetChild(0).gameObject.SetActive(false);
                        gameResultPanel[i].UI.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    else
                    {
                        gameResultPanel[i].UI.transform.GetChild(0).gameObject.SetActive(true);
                        gameResultPanel[i].UI.transform.GetChild(1).gameObject.SetActive(false);
                    }
                    break;

                // GameStage에 현재 씬 이름(스테이지) 설정
                case "Stage":
                    gameResultPanel[i].UI.GetComponent<Text>().text = dm.GetStage();
                    break;

                // Money, Honor 텍스트 설정
                case "Reward":
                    if (gameState == GameState.Over)
                    {
                        gameResultPanel[i].UI.transform.GetChild(0).gameObject.SetActive(false);
                        gameResultPanel[i].UI.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    else
                    {
                        gameResultPanel[i].UI.transform.GetChild(0).gameObject.SetActive(true);
                        gameResultPanel[i].UI.transform.GetChild(1).gameObject.SetActive(false);
                    }
                    break;

                case "Money":
                    if (gameState == GameState.Clear)
                        gameResultPanel[i].UI.GetComponent<Text>().text = "+" + money;
                    break;
                case "Honor":
                    if (gameState == GameState.Clear)
                        gameResultPanel[i].UI.GetComponent<Text>().text = "+" + honor;
                    break;

                // 구출한 부상자만큼 색깔 설정
                case "Injured":
                    Transform uiTransform = gameResultPanel[i].UI.transform;

                    // 구조된ㄴ 부상자
                    for (int j = uiTransform.childCount - 1; j >= dm.totalInjuredCount; j--)
                    {
                        Debug.Log(dm.totalInjuredCount);
                        uiTransform.GetChild(j).gameObject.SetActive(false);
                    }

                    for (int j = 0; j < dm.totalInjuredCount - leftInjured; j++)
                    {
                        Transform injured = uiTransform.GetChild(j);
                        injured.GetComponent<Image>().sprite
                            = resultRescuerSprites[1];
                        injured.GetChild(0).gameObject.SetActive(true);

                    }
                    // 구조 실패한 부상자
                    for (int j = 0; j < leftInjured; j++)
                    {
                        Debug.Log(leftInjured);
                        Transform injured = uiTransform.GetChild(dm.totalInjuredCount - 1 - j);
                        injured.GetComponent<Image>().sprite
                            = resultRescuerSprites[0];
                        injured.GetChild(0).gameObject.SetActive(false);
                    }       
                    break;
                case "Panel":
                    panel = gameResultPanel[i].UI;
                    break;

                case "Evidence":
                    GameObject ui = gameResultPanel[i].UI;

                    if (leftInjured == 0)
                    {
                        // 단서 획득

                        Dictionary<string, object> eviData = dm.LoadEvidenceData();

                        ui.transform.GetChild(1).GetComponent<Text>().text
                            = eviData["evidenceName"].ToString();

                        ui.SetActive(true);
                    }
                    else
                        ui.SetActive(false);
                    
                    break;
            }
        }

        // 다른 UI 설정 끝나면 패널 열기
        panel.SetActive(true);
    }

    private IEnumerator CheckTime()
    {
        while ((time -= Time.deltaTime) > 0 && !gameOver)
        {
            SetTimeText(time);

            oxygenSlider.value -= Time.deltaTime;
            
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

    // 설정창 온오프
    public void SettingButton(bool OnOff)
    {
        // true: On. false: Off
        if (OnOff)
        {
            // 설정 버튼을 중복해서 누를 수 있으므로 추가
            if (settings.activeInHierarchy) return;

            else
            {
                // 게임 정지
                StopGame();

                // (용현) UI 비활성화
                foreach (GameObject ui in UI) ui.SetActive(false);

                // 설정창 활성화
                settings.SetActive(true);
            }
        }

        else
        {
            // 게임 시작
            ResumeGame();

            // (용현) UI 활성화
            foreach (GameObject ui in UI) ui.SetActive(true);

            // 설정창 비활성화
            settings.SetActive(false);
        }
    }

    // 씬 옮기기
    public void MoveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}