/****************************************
* AcheivementController.cs
* 제작: 김용현
* 로비 도전과제 패널 컨트롤러
* 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
* 작성일자: 19.08.25
* 수정일자: 19.08.25
***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementController : MonoBehaviour
{
    public GameObject[] achievements;           // 도전과제 목록
    public Text _name;                          // 도전과제 이름
    public Text contents;                       // 내용


    private void Start()
    {
        UserDataIO.User user = UserDataIO.ReadUserData();

        // 도전과제 개수 하드코딩함
        for (int index = 0; index < 5; index++)
        {
            switch (index)
            {
                case 0:
                   if (user.playCount >= 10) achievements[index].GetComponent<Image>().color = Color.green;
                    break;
                case 1:
                    if (user.clearCount >= 10) achievements[index].GetComponent<Image>().color = Color.green;
                    break;
                case 2:
                    if (user.money >= 10000) achievements[index].GetComponent<Image>().color = Color.green;
                    break;
                case 3:
                    if (user.honor >= 20) achievements[index].GetComponent<Image>().color = Color.green;
                    break;
                case 4:
                    /* Not Implemented */
                    break;
            }
        }
    }

    public void AchievementBtn(int num)
    {
        switch (num)
        {
            case 0:
                _name.text = "도전과제 1";
                contents.text = "플레이 횟수 10회";

                break;

            case 1:
                _name.text = "도전과제 2";
                contents.text = "클리어 횟수 10회";
                break;

            case 2:
                _name.text = "도전과제 3";
                contents.text = "돈 10000 모으기";
                break;

            case 3:
                _name.text = "도전과제 4";
                contents.text = "명예 20 달성";
                break;

            case 4:
                _name.text = "도전과제 5";
                contents.text = "도전과제 5은 뭘까요?";
                break;
        }
    }
}