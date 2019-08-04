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

    private UserDataIO.User userData;

    private void Start()
    {
        userData = UserDataIO.ReadUserData();

        SetUIText();
    }

    private void SetUIText()
    {
        moneyText.text = "소지 금액 : " + userData.money;
        honorText.text = "명예 점수 : " + userData.honor;
    }

    // (임시) 버튼 클릭 시 씬 이동 함수
    // 우선은 버튼에 onClick 함수 설정할 때 스트링으로 연결된 씬 입력하도록 설정
    public void MoveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
