﻿/****************************************
 * Lobby.cs
 * 제작: 조예진
 * 로비 씬 UI 관리 스크립트
 * (19.10.10) 예진 - 계급 UI 표시 추가
 * (19.10.12) 용현 - MoveScene 함수 static으로 변경
 * (19.10.12) 예진 - 단서창 스크립트 분리
 * 작성일자: 19.08.03.
 * 수정일자: 19.10.12.
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{

    [Header("Top")]
    public Text moneyText;
    public Text rankName;
    public Image rankImg;
    public Slider rankGaze;
    public Text rankPercent;
    public Text stressState;
    public Image stressImg;


    [Header("Rank Up")]
    public GameObject RankUpPanel;
    public Sprite[] rankSprites;
    public Image preRank;
    public Image nowRank;

    [Header("Panel")]
    public GameObject NotifiPanel;
    public GameObject EvidencePanel;
    public GameObject ExitPanel;

    private UserDataIO.User userData;
    List<Dictionary<string, object>> rankData;
    int nextRankHonor;
    private ItemDataManager idm;

    private void Start()
    {
        idm = ItemDataManager.Instance;
        userData = UserDataIO.ReadUserData();
        rankData = idm.GetRankData();

        CheckUserRankUpped();

        SetUIText();
        AudioManager.Instance.PlayAudio("LobbyBgm", 0, 0f, true);
        
        // 피로도 100 이상인데 게임 초기화 되지 않았을 경우/초기화 씬으로 이동하지 않았을 경우 초기화 씬으로 이동
        if (userData.stress >= 100)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ExitPanel.activeInHierarchy) ExitPanel.SetActive(false);

            else ExitPanel.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    private void CheckUserRankUpped()
    {

        if (userData.rank >= rankData.Count - 1)
            return;

        int preRank = userData.rank;

        while (userData.rank < rankData.Count - 1)
        {
            nextRankHonor = System.Convert.ToInt32(rankData[userData.rank + 1]["honor"]);

            if (userData.honor > nextRankHonor)
                userData.rank++;
            else
                break;
        }

        if (preRank != userData.rank)
        {
            UserDataIO.WriteUserData(userData);

            OpenRankupPanel(preRank, rankData);
        }
    }

    private void OpenRankupPanel(int preLv, List<Dictionary<string, object>> rankData)
    {
        preRank.sprite = rankSprites[preLv];
        nowRank.sprite = rankSprites[userData.rank];

        preRank.SetNativeSize();
        preRank.rectTransform.sizeDelta = new Vector2(preRank.rectTransform.rect.width * (26 / preRank.rectTransform.rect.height), 26);

        nowRank.SetNativeSize();
        nowRank.rectTransform.sizeDelta = new Vector2(nowRank.rectTransform.rect.width * (26 / nowRank.rectTransform.rect.height), 26);

        preRank.transform.GetChild(0).GetComponent<Text>().text = rankData[preLv]["name"].ToString();
        nowRank.transform.GetChild(0).GetComponent<Text>().text = rankData[userData.rank]["name"].ToString();
        AudioManager.Instance.PlayAudio("LobbyEffect", 0, 0f, false);

        RankUpPanel.SetActive(true);
        
        SetUIText();
    }


    /* TOP UI */

    public void SetUIText()
    {
        userData = UserDataIO.ReadUserData();

        moneyText.text = userData.money + "";
        
        rankName.text = rankData[userData.rank]["name"].ToString();
        rankImg.sprite = rankSprites[userData.rank];
        rankImg.SetNativeSize();
        Vector2 size = rankImg.rectTransform.sizeDelta;
        rankImg.rectTransform.sizeDelta = new Vector2(size.x * 52 / size.y, 52);

        int i;

        for (i = 0; i < idm.stressNames.Length - 1; i++)
        {
            if (userData.stress < idm.stressValues[i + 1] && userData.stress >= idm.stressValues[i])
            {
                stressState.text = idm.stressNames[i];
                break;
            }
        }

        if (i == idm.stressNames.Length) stressState.text = idm.stressNames[0];

        stressImg.fillAmount = userData.stress / 100f;

        int preRankHonor = 0;

        if (userData.rank != 0)
            preRankHonor = System.Convert.ToInt32(rankData[userData.rank - 1]["honor"]);

        if (userData.rank != rankData.Count - 1)
        {
            rankGaze.value = 1f * (userData.honor - preRankHonor) / nextRankHonor;
            rankPercent.text = (int)(rankGaze.value * 100) + "%";
        }
        else
        {
            rankGaze.value = 1;
            rankPercent.text = "100%";
        }
    }

    /* LOCKER ROOM */

    // 디버깅 용 버튼 
    public void ResetItemData()
    {
        userData.oxygenlv = 0;
        userData.gloveslv = 0;
        userData.axelv = 0;
        userData.shoeslv = 0;

        UserDataIO.WriteUserData(userData);

        GetComponent<ItemUpgrade>().UpdateEquipViewport();
    }


    public IEnumerator Notify (string notification)
    {
        // 알림창 텍스트 설정
        Text text = NotifiPanel.transform.GetChild(0).GetComponent<Text>();
        text.text = notification;

        Image image = NotifiPanel.GetComponent<Image>();

        // 투명도 초기화
        Color initColor = image.color;
        initColor.a = 1f;
        image.color = initColor;

        initColor = text.color;
        initColor.a = 1f;
        text.color = initColor;

        NotifiPanel.SetActive(true);

        float dt = 0.3f;

        while (dt > 0)
        {
            dt -= Time.deltaTime;

            yield return null;
        }

        while (image.color.a > 0)
        {
            // 패널 투명도 변경
            Color newAlpha = image.color;
            newAlpha.a -= 0.03f;

            image.color = newAlpha;

            // 텍스트 투명도 변경
            newAlpha = text.color;
            newAlpha.a -= 0.03f;

            text.color = newAlpha;

            yield return null;
        }

        NotifiPanel.SetActive(false);
    }

    // (임시) 버튼 클릭 시 씬 이동 함수
    // 우선은 버튼에 onClick 함수 설정할 때 스트링으로 연결된 씬 입력하도록 설정
    public static void MoveScene(string sceneName)
    {
        //SceneManager.LoadScene(sceneName);
        LoadingManager.LoadScene(sceneName);
    }

    public void OnClickMoveScene(string sceneName)
    {
        //SceneManager.LoadScene(sceneName);
        LoadingManager.LoadScene(sceneName);
    }
}
