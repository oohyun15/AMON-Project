/****************************************
 * Tutorial.cs
 * 제작: 조예진
 * 튜토리얼 기능 추가 위해 Dialog.cs의 subclass 생성
 * 작성일자: 19.10.26.
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : Dialog
{
    [Header("UI")]
    public GameObject TutorialPanel;
    public List<Transform> images;
    public GameObject UIs;

    public Transform[] pos;

    private GameObject minimapPreview;
    private GameObject startBtn;
    public GameObject setting;

    public bool isLobby = false;

    private string prefsKey = "isPlayedTutorial";


    // 튜토리얼 실행 여부 확인
    protected void Start()
    {
        if (isLobby) prefsKey += "_lobby";
        
        for (int i = 0; i < UIs.transform.childCount; i++)
            images.Add(UIs.transform.GetChild(i));

        if (!isLobby)
        {
            GameManager gm = GameManager.Instance;

            minimapPreview = gm.minimapPreview.gameObject;
            startBtn = gm.startButton;
            setting = gm.settings;
        }

        // 아랫줄 주석 풀면 실행 시마다 튜토리얼 기록 지움
        //PlayerPrefs.DeleteKey(prefsKey);

        // 튜토리얼 플레이 여부 확인
        if (PlayerPrefs.GetInt(prefsKey, 0) == 1)
        {
            Debug.Log("튜토리얼 실행 기록 있음");
            return;
        }

        Debug.Log("튜토리얼 실행 기록 없음, 튜토리얼 실행");

        TutorialPanel.SetActive(true);

        InitDialog();
    }

    public override void InitDialog()
    {
        path = rootPath + "tutorial_dialog" + (isLobby ? "_lobby" : "");
        //+ DataManager.Instance.SceneName;

        base.InitDialog();
    }

    // 대사 업데이트 시 강조 이미지 변경
    protected override void UpdateDialog()
    {
        base.UpdateDialog();

        if (index > 0)
        {
            images[index - 1].gameObject.SetActive(false);

            if (index == 2 && !isLobby && false)
            {
                // 튜토리얼 산소통 이미지 길이 설정
                RectTransform oxygen = GameManager.Instance.oxygenSlider.transform.GetChild(0).GetComponent<RectTransform>();
                RectTransform oxrt = UIs.transform.GetChild(3).GetComponent<RectTransform>();
                RectTransform oxbgrt = UIs.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>();

                oxrt.sizeDelta = new Vector2(oxygen.parent.GetComponent<RectTransform>().rect.width, oxrt.sizeDelta.y);
                oxbgrt.sizeDelta = new Vector2(oxygen.GetComponent<RectTransform>().rect.width, oxbgrt.sizeDelta.y);
                oxbgrt.position = oxygen.position;
            }
        }

        images[index].gameObject.SetActive(true);
    }

    // 튜토리얼 끝났을 때 - 튜토리얼 패널 비활성화 후 게임 초기화
    public override void EndDialog()
    {
        if (index > 0)
            images[index - 1].gameObject.SetActive(false);

        TutorialPanel.SetActive(false);

        if (!isLobby)
        {
            if (PlayerPrefs.GetInt(prefsKey, 0) == 0)
            {
                minimapPreview.transform.parent.gameObject.SetActive(true);
                startBtn.SetActive(true);
            }
            else
            {
                setting.SetActive(true);
            }
        }
        

        if (isLobby && PlayerPrefs.GetInt(prefsKey, 0) == 1)
            setting.SetActive(true);

        PlayerPrefs.SetInt(prefsKey, 1);
    }
}
