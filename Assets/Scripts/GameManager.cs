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
 * (19.10.13)  Stage 클래스 관련 부상자수 저장 추가
 * (19.10.19)  파티클 리스트 추가
 * (19.11.09)  게임오버씬 이름 오류 수정
 * (19.11.11)  도전과제 1번 수정
 * (19.11.16)  인터렉션 버튼 수정
 * (19.12.07)  인게임 우측 상단에 도전과제 패널 추가
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.26
 * 수정일자: 19.11.11
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IObserver
{
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
                    Debug.Log("There's no active GameManager object");
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
    public int stageNum;                        // 스테이지 번호, (Ex. Stage 1-1 => stageNum = 0, Stage 2-2 => stageNum = 4 (= 1*3 + 1)) 
    public int leftInjured;
    public float timeLimit;                     // 제한 시간, 초 단위
    public float hurtTime;                      // 캔버스에 빨갛게 올라와요!

    [Header("UI")]
    public Text leftTimeText;
    public Slider oxygenSlider;                 // (예진) 산소통 테스트
    public GameObject startButton;
    public GameObject settingsButton;
    public GameObject warningPanel;             // (예진) 탈출 버튼
    public GameObject evidencePanel;
    public Transform evidence;
    public ResultAnimationController resultPanel;
    public Button resultOkButton;               // 게임 결과창 확인 버튼
    public GameObject settings;
    public GameObject[] UI;                     // (용현) 0: Joystick, 1: Interaction, 2: ItemSlots, 3: Minimap
    public Image minimapPreview;
    public Image interactionImage;              // (용현) 인터렉션 아이템 이미지
    public GameObject LeftInjuredNumImages;     // (예진) 부상자 몇명 남았는지 보여줄 오브젝트
    public GameObject achievementPanel;         // (용현) 도전과제 우측 상단 패널
    public Image achievementImage;
    public Text achievementText;

    [Header("Particle")]
    public GameObject[] FX_Ingame;              // 0: FX_Save, 1: FX_Damaged, 2: FX_Hurt, 3: FX_HitDust

    [Header("Field Objects")]
    public AmonController player;
    public GameObject Cam;
    public GameObject injuredParent;
    public Dictionary<string, List<GameObject>> objects;
    public Animator resultCharacterAnimator;    // (예진) 게임 결과창에 렌더되는 캐릭터 오브젝트 애니메이터

    [Header("Sprites")]
    public Sprite[] resultRescuerSprites;       // (예진) 결과 패널에 부상자 구조 여부 스프라이트 받아둠    
                                                // 0 --> 구조 실패 / 1 --> 구조 성공
    public Sprite[] oxygenSprites;              // 0 --> 평상시 / 1 --> 긴급
    public Sprite[] achievementImages;          // (용현) 도전과제 이미지
    public Sprite[] itemImages;                 // (용현) 인터렉션 아이템 이미지 종류
                                                // 0: Rescue, 1: Axe, 2: Kick

    public float time;                          // 남은 시간, 초 단위

    /* private variable */
    private bool gameOver;
    private IEnumerator timeCheckCoroutine;
    private List<int> achievementQueue;
    private bool lock_spin = false;             // (용현) 스핀락 변수
    private float velocity;                     // (용현) 패널 열고 닫을 때 쓸 속도 변수. 건드리지 마셈

    private void Awake()
    {
        // 스크립트 실행 순서로 인해 Awake에서 선언
        objects = new Dictionary<string, List<GameObject>>();
    }

    private void Start()
    {
        dm = DataManager.Instance;

        // 미니맵 프리뷰 스프라이트 불러와서 설정
        minimapPreview.sprite = dm.minimap;

        string name = dm.SceneName;
        stageNum = (name[5] - 49) * 3 + (name[7] - 49);

        InitGame();
    }

    private void FixedUpdate()
    {
        // Debug: Reset
        if (Input.GetKeyDown(KeyCode.R))
            InitGame();

        // Debug: Settings
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settings.activeInHierarchy) SettingButton(false);

            else SettingButton(true);
        }
    }

    public void InitGame()
    {
        StopAllCoroutines();

        gameOver = false;

        time = timeLimit;

        ApplyEquipItemEffect();

        /* 산소통 크기 설정*/
        RectTransform rt = oxygenSlider.GetComponent<RectTransform>();
        RectTransform bgrt = oxygenSlider.transform.GetChild(0).GetComponent<RectTransform>();
        RectTransform fillrt = oxygenSlider.transform.GetChild(1).GetComponent<RectTransform>();

        RectTransform bgFramert = oxygenSlider.transform.GetChild(2).GetComponent<RectTransform>();
        RectTransform textrt = oxygenSlider.transform.GetChild(3).GetComponent<RectTransform>();

        float temp = (time - timeLimit) * 10;
        timeLimit = time;

        rt.sizeDelta = new Vector2(rt.sizeDelta.x + temp, rt.sizeDelta.y);
        bgrt.sizeDelta = new Vector2(bgrt.sizeDelta.x + temp, bgrt.sizeDelta.y);
        bgFramert.sizeDelta = new Vector2(bgFramert.sizeDelta.x + temp, bgFramert.sizeDelta.y);

        oxygenSlider.maxValue = time;
        oxygenSlider.value = time;

        fillrt.localPosition = new Vector2(fillrt.rect.width / 2 - temp / 2, fillrt.localPosition.y);
        textrt.localPosition = new Vector2(fillrt.rect.width / 2 - temp / 2, fillrt.localPosition.y);
        /* 산소통 크기 설정 끝*/

        // 도전과제 대기큐 생성
        achievementQueue = new List<int>();

        // (용현) UI 비활성화
        foreach (GameObject ui in UI) ui.SetActive(false);

        // FX_Hurt 비활성화
        FX_Ingame[2].SetActive(false);

        // 필드 오브젝트 초기화 (자동 버전)
        foreach (var pair in objects)
            foreach (var value in pair.Value)
                value.GetComponent<IReset>().SetInitValue();

        // (19.11.18) 남은 부상자 이미지 UI total 값만큼만 활성화하기
        for (int i = 0; i < dm.total; i++)
            LeftInjuredNumImages.transform.GetChild(i).gameObject.SetActive(true);

        // 결과창 초기화
        resultPanel.gameObject.SetActive(false);
        resultCharacterAnimator.SetBool("Victory", false);
        resultCharacterAnimator.SetBool("Fail", false);
        resultCharacterAnimator.gameObject.SetActive(false);

        settingsButton.SetActive(false);
        settings.SetActive(false);

        // 미니맵 미리보기 초기화
        minimapPreview.SetNativeSize();
        RectTransform previewRect = minimapPreview.GetComponent<RectTransform>();
        previewRect.sizeDelta = new Vector2(previewRect.rect.width * 300 / previewRect.rect.height, 300);

        RectTransform frameRect = minimapPreview.transform.GetChild(0).GetComponent<RectTransform>();
        frameRect.sizeDelta = new Vector2(previewRect.rect.width + 15, previewRect.rect.height + 20);

        // (19.10.26 예진) 튜토리얼 실행 중에 미니맵 프리뷰 + 시작 버튼 보이지 않도록 함 
        if (PlayerPrefs.GetInt("isPlayedTutorial", 0) == 1)
        {
            minimapPreview.transform.parent.gameObject.SetActive(true);
            startButton.SetActive(true);
        }

        // BGM 틀기
        if (stageNum >= 0 && stageNum < 3) AudioManager.Instance.PlayAudio("GameManagerBgm", 0, 0f, true);
        else if (stageNum >= 3 && stageNum < 6) AudioManager.Instance.PlayAudio("GameManagerBgm", 1, 0f, true);
        else if (stageNum >= 6 && stageNum < 9) AudioManager.Instance.PlayAudio("GameManagerBgm", 2, 0f, true);
        else if (stageNum >= 9 && stageNum < 12) AudioManager.Instance.PlayAudio("GameManagerBgm", 3, 0f, true);
        else AudioManager.Instance.PlayAudio("GameManagerBgm", 4, 0f, true);

        // 초기 부상자 수 확인
        CheckLeftInjured();
    }

    private void ApplyEquipItemEffect()
    {
        UserDataIO.User user = UserDataIO.ReadUserData();

        int oxygenEffect = GetEquipedItemEffect("oxygen", user);
        int glovesEffect = GetEquipedItemEffect("gloves", user);
        int shoesEffect = GetEquipedItemEffect("shoes", user);
        int axeEffect = GetEquipedItemEffect("axe", user);

        time = oxygenEffect;
        player.AttackSpd = 1.5f * glovesEffect / 100;
        player.initMoveSpeed = 5 * shoesEffect / 100;
        player.axeDamage = axeEffect;
    }

    private int GetEquipedItemEffect(string item, UserDataIO.User user)
    {
        int itemLv = 0;

        switch (item)
        {
            case "oxygen":
                itemLv = user.oxygenlv;
                break;

            case "gloves":
                itemLv = user.gloveslv;
                break;

            case "axe":
                itemLv = user.axelv;
                break;

            case "shoes":
                itemLv = user.shoeslv;
                break;
        }

        Dictionary<string, object> data = ItemDataManager.Instance.GetEquipItemData();

        object effectData = ((List<Dictionary<string, object>>)data[item])[itemLv]["effect"];

        return System.Convert.ToInt32(effectData);
    }

    public void StartGame()
    {
        // UI 활성화 설정
        startButton.SetActive(false);
        minimapPreview.transform.parent.gameObject.SetActive(false);
        settingsButton.SetActive(true);
        foreach (GameObject ui in UI) ui.SetActive(true);

        // 시간 시작
        timeCheckCoroutine = CheckTime();
        StartCoroutine(timeCheckCoroutine);

        // 부상자 리스트 설정
        List<GameObject> injureds = new List<GameObject>();
        injureds.AddRange(objects["Serious"]);
    }

    public void StopGame()
    {
        StopCoroutine(timeCheckCoroutine);
    }

    public void ResumeGame()
    {
        StartCoroutine(timeCheckCoroutine);
    }

    public int CheckLeftInjured()
    {
        leftInjured = 0;
        List<GameObject> injureds = new List<GameObject>();
        injureds.AddRange(objects["Serious"]);

        foreach (GameObject i in injureds)
            if (i.activeInHierarchy)
                leftInjured++;

        int count = 0;

        foreach (Image i in LeftInjuredNumImages.GetComponentsInChildren<Image>())
        {
            if (count < dm.total - leftInjured)
                i.sprite = resultRescuerSprites[1];
            else break;

            count++;
        }

        return leftInjured;
    }

    public void CheckGameClear()
    {
        gameOver = true;

        CheckLeftInjured();

        if (time <= 0) GameOver();
        else GameClear();
    }

    // (19.10.03 예진) 게임 클리어, 실패 공통 실행 부분 합침
    public void GameEnd()
    {
        int rescuedCount = dm.total - leftInjured;

        AudioManager.Instance.StopAllAudio();
        AudioManager.Instance.PlayAudio("GameManagerEffect", 0, 0.5f, false);

        // (19.10.13) 이번 스테이지에서 구출한 인원 저장
        UserDataIO.Stage stage = UserDataIO.ReadStageData();
        stage.rescueNum[stageNum] = rescuedCount;
        stage.isPlayed[stageNum] = 1;
        UserDataIO.WriteStageData(stage);

        // (19.08.25) 플레이 횟수 증가
        UserDataIO.User user = UserDataIO.ReadUserData();
        user.playCount++;
        UserDataIO.WriteUserData(user);

        CheckAchievements(user, stage);

        // (19.10.03 예진) 피로도 업데이트
        int stress = dm.GetStageReward(DataManager.RewardType.stress, leftInjured);

        // (19.11.09 예진) 피로도 100 이상일 때 게임 결과창의 확인 버튼 누르면 초기화 씬으로 이동
        if (!UserDataIO.ChangeUserStress(stress))
            SetButtonListener(resultOkButton, "GameOver");

        /* 대화창 표시 */
        for (int i = ItemDataManager.Instance.stressValues.Length - 2; i > 0; i--)
            if (UserDataIO.ReadUserData().stress >= ItemDataManager.Instance.stressValues[i]
                && user.stress < ItemDataManager.Instance.stressValues[i])
            {
                StoryDialog.instance.SetFile("stress" + i);
                break;
            }

        if (dm.IsLastStage())
        {
            StoryDialog.instance.SetFile("clear" + (dm.SceneName[5] - 48));

            // 게임 모든 스테이지 클리어 시 엔딩 씬으로
            if (dm.SceneName[5] - 48 == 5)
                SetButtonListener(resultOkButton, "GameOver");
            // 게임의 큰 스테이지 하나를 클리어 시 로비 씬으로
            else
                SetButtonListener(resultOkButton, "Lobby");
        }

        // 애니메이션 멈춤
        player.AnimationIdle();

        // 조이스틱 멈춤
        JoystickController.instance.StopJoystick();

        // UI 활성화 여부 설정
        settingsButton.SetActive(false);
        foreach (GameObject ui in UI) ui.SetActive(false);
        resultCharacterAnimator.gameObject.SetActive(true);
    }

    private void GameClear()
    {
        GameEnd();

        int money = dm.GetStageReward(DataManager.RewardType.money, leftInjured);
        int honor = dm.GetStageReward(DataManager.RewardType.honor, leftInjured);

        dm.SaveGameResult(money, honor);

        // (19.09.23.) 결과창 애니메이션 설정
        if (leftInjured == dm.total)
        {
            resultCharacterAnimator.SetBool("Fail", true);
            AudioManager.Instance.PlayAudio("GameManagerEffect", 2, 0f, false);
        }
        else
        {
            resultCharacterAnimator.SetBool("Victory", true);
            AudioManager.Instance.PlayAudio("GameManagerEffect", 3, 0f, false);
        }

        // 게임 결과창 활성화
        ShowGameResult(money, honor);

        StopGame();
    }

    public void GameOver()
    {
        GameEnd();

        // (19.09.23.) 결과창 애니메이션 설정
        resultCharacterAnimator.SetBool("Fail", true);

        AudioManager.Instance.PlayAudio("GameManagerEffect", 2, 0f, false);

        // 게임 결과창 활성화
        ShowGameResult(0, 0);

        StopGame();
    }

    // (예진 19.08.05) 게임 결과 보여주는 UI 창 설정
    private void ShowGameResult(int money, int honor)
    {
        resultPanel.StartResultAnimation(dm.total, leftInjured, money, honor, UserDataIO.ReadUserData().stress);
    }

    private IEnumerator CheckTime()
    {
        bool isChangedSprite = time < 20 ? true : false;

        while ((time -= Time.deltaTime) > 0 && !gameOver)
        {
            SetTimeText(time);

            oxygenSlider.value -= Time.deltaTime;

            // (19.10.19) FX_Hurt 추가
            if (time < hurtTime &&
                !FX_Ingame[2].activeInHierarchy) FX_Ingame[2].SetActive(true);

            if (time < 20 && !isChangedSprite)
            {
                oxygenSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = oxygenSprites[1];

                isChangedSprite = true;
                AudioManager.Instance.PlayAudio("TimeOut", 0, 0f, true);
            }

            yield return null;
        }

        SetTimeText(time);

        CheckGameClear();
    }

    private void SetTimeText(float setTime)
    {

        if (setTime < 0) setTime = 0;
        leftTimeText.text = ((int)setTime).ToString();
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
                StopGame();

                // (용현) UI 비활성화
                foreach (GameObject ui in UI) ui.SetActive(false);

                settings.SetActive(true);
            }
        }
        else
        {
            ResumeGame();

            // (용현) UI 활성화
            foreach (GameObject ui in UI) ui.SetActive(true);

            settings.SetActive(false);
        }
    }

    public void MoveScene(string sceneName)
    {
        LoadingManager.LoadScene(sceneName);
    }

    // 도전과제 성공 여부 조건
    public void CheckAchievements(UserDataIO.User user, UserDataIO.Stage stage)
    {
        for (int index = 0; index < UserDataIO.achievementCount; index++)
        {
            if (user.achievementList[index] == 1) continue;

            var temp = CSVReader.Read(AchievementController.achievementDataPath);

            var temp2 = CSVReader.Read(UserDataIO.stageDataPath);

            int condition = System.Convert.ToInt32(temp[index]["Condition"]);


            switch (index)
            {
                // 1 스테이지 전체 구조 시 달성
                case 0:

                    bool checkBit = true;

                    for (int i = 0; i < 3; i++)
                    {
                        int num = System.Convert.ToInt32(temp2[4 * i]["save"]);

                        if (stage.rescueNum[i] != num)
                        {
                            checkBit = false;

                            break;
                        }
                    }

                    if (checkBit) UpdateAchievement(0);
                    break;

                case 1:
                    bool isAcheived = true;

                    for (int i = 0; i < stage.isGotEvidence.Length; i++)
                    {
                        if (stage.isGotEvidence[i] == 0)
                        {
                            isAcheived = false;
                            break;
                        }
                    }

                    if (isAcheived) UpdateAchievement(1);
                    break;

                case 2:
                    if (user.rescuedCount >= condition) UpdateAchievement(2);
                    break;

                case 3:
                    if (user.destroyCount >= condition) UpdateAchievement(3);
                    break;

                case 4:
                    if (user.playCount >= condition) UpdateAchievement(4);
                    break;

                case 5:
                    if (user.deathCount >= condition) UpdateAchievement(5);
                    break;
            }
        }
    }

    public void UpdateAchievement(int index)
    {
        // (19.10.04) 도전과제 성공 여부 업데이트
        UserDataIO.User user = UserDataIO.ReadUserData();

        user.achievementList[index] = 1;

        UserDataIO.WriteUserData(user);

        ShowAchievementPanel(index);
    }

    public void ShowAchievementPanel(int index)
    {
        // Enqueue
        achievementQueue.Add(index);
        AudioManager.Instance.PlayAudio("LobbyEffect", 3, 0f, false);
        // Spin Lock
        StartCoroutine(SpinLock());
    }

    public IEnumerator PanelTimer(float time, GameObject panel)
    {
        StartCoroutine(OpenPanel(panel));

        yield return new WaitForSeconds(time);

        StartCoroutine(ClosePanel(panel));
    }

    public IEnumerator OpenPanel(GameObject panel)
    {
        var panelPos = panel.GetComponent<RectTransform>();

        float TargetPositionY = panelPos.anchoredPosition.y - panelPos.sizeDelta.y;

        while (panelPos.anchoredPosition.y - TargetPositionY > 1f)
        {
            Vector2 vec = new Vector2(
                panelPos.anchoredPosition.x,
                Mathf.SmoothDamp(panelPos.anchoredPosition.y, TargetPositionY - 0.5f, ref velocity, 0.25f)
                );

            panelPos.anchoredPosition = vec;

            yield return null;
        }
        yield break;
    }

    public IEnumerator ClosePanel(GameObject panel)
    {
        var panelPos = panel.GetComponent<RectTransform>();

        float TargetPositionY = panelPos.anchoredPosition.y + panelPos.sizeDelta.y;

        while (TargetPositionY - panelPos.anchoredPosition.y > 1f)
        {
            Vector2 vec = new Vector2(
                panelPos.anchoredPosition.x,
                Mathf.SmoothDamp(panelPos.anchoredPosition.y, TargetPositionY - 0.5f, ref velocity, 0.25f)
                );

            panelPos.anchoredPosition = vec;

            yield return null;
        }

        // Dequeue
        achievementQueue.RemoveAt(0);

        // Unlock
        lock_spin = false;

        yield break;
    }

    public IEnumerator SpinLock()
    {
        // Spin Lock
        while (lock_spin) yield return null;

        lock_spin = true;

        // Set Achievement
        var achievementData = CSVReader.Read(AchievementController.achievementDataPath);

        achievementText.text = achievementData[achievementQueue[0]]["Name"].ToString();

        achievementImage.sprite = achievementImages[achievementQueue[0]];

        StartCoroutine(PanelTimer(3f, achievementPanel));

        yield break;
    }

    private void SetButtonListener(Button btn, string scenename)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => LoadingManager.LoadScene(scenename));
    }
}