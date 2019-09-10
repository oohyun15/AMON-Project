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
    [Header("UI")]
    public Text moneyText;
    public Text honorText;
    public GameObject NotifiPanel;


    private UserDataIO.User userData;

    private void Start()
    {
        userData = UserDataIO.ReadUserData();

        SetUIText();
    }

    public void SetUIText()
    {
        userData = UserDataIO.ReadUserData();

        moneyText.text = userData.money + "";
        honorText.text = userData.honor + "";
    }

    // 점점 투명해지는 알림창
    /*
    public void Notify (string notification)
    {
        StartCoroutine(OpenNotifyPanel(notification));
    }*/

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

    // 테스트 버튼 
    public void ResetItemData()
    {
        PlayerPrefs.DeleteKey("oxygenlv");
        PlayerPrefs.DeleteKey("gloveslv");

        GetComponent<ItemUpgrade>().UpdateEquipViewport();
    }

    // (임시) 버튼 클릭 시 씬 이동 함수
    // 우선은 버튼에 onClick 함수 설정할 때 스트링으로 연결된 씬 입력하도록 설정
    public void MoveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
