/****************************************
 * Lobby.cs
 * 제작: 조예진
 * 로비 씬 UI 관리 스크립트
 * 작성일자: 19.08.03.
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
    public Text honorText;

    [Header("Evidence")]
    public Image eviImage;
    public Text eviName;
    public Text eviInfo;
    public Transform eviContents;
    public Sprite[] eviSprites;

    [Header("Panel")]
    public GameObject NotifiPanel;
    public GameObject EvidencePanel;

    private UserDataIO.User userData;
    private ItemDataManager idm;

    private void Start()
    {
        idm = ItemDataManager.Instance;
        userData = UserDataIO.ReadUserData();

        SetUIText();

        InitEvidenceScroll();
        InitEviInfo();

        // 피로도 100 이상인데 게임 초기화 되지 않았을 경우/초기화 씬으로 이동하지 않았을 경우 초기화 씬으로 이동
        if (userData.stress >= 100)
        {
            SceneManager.LoadScene("GameOver_test");
        }
    }


    /* TOP UI */

    public void SetUIText()
    {
        userData = UserDataIO.ReadUserData();

        moneyText.text = userData.money + "";
        honorText.text = userData.honor + "";
    }

    /* LOCKER ROOM */

    // 디버깅 용 버튼 
    public void ResetItemData()
    {
        PlayerPrefs.DeleteKey("oxygenlv");
        PlayerPrefs.DeleteKey("gloveslv");

        userData.oxygenlv = 0;
        userData.gloveslv = 0;
        userData.axelv = 0;

        UserDataIO.WriteUserData(userData);

        GetComponent<ItemUpgrade>().UpdateEquipViewport();
    }

    /* EVIDENCE PANEL */

    public void InitEvidenceScroll()
    {
        List<Dictionary<string, object>> eviList = idm.GetEvidenceData();
        Evidence[] eviArr = eviContents.GetComponentsInChildren<Evidence>();

        for (int i = 0; i < eviArr.Length; i++)
        {
            Dictionary<string, object> eviData = eviList[i];
            Evidence evi = eviArr[i];
            evi.SetEvidence(eviData, eviSprites[i]);
            evi.btn.onClick.AddListener(() => OnClickEvidence(evi.Name, evi.Explain, evi.Img));
        }

        EvidencePanel.SetActive(false);
    }

    public void InitEviInfo()
    {
        eviImage.sprite = null;
        eviName.text = "";
        eviInfo.text = "단서를 선택해 주세요";
    }

    public void OnClickEvidence(string name, string explain, Sprite sprite)
    {
        eviImage.sprite = sprite;
        eviName.text = name;
        eviInfo.text = explain;
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
    public void MoveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
