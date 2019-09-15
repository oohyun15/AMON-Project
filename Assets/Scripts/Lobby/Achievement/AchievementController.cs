/****************************************
* AcheivementController.cs
* 제작: 김용현
* 로비 도전과제 패널 컨트롤러
* 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
* (19.09.15) 도전과제 패널 스크롤 형태로 변경
* 작성일자: 19.08.25
* 수정일자: 19.09.15
***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementController : MonoBehaviour
{
    public GameObject achievementList;
    public Achievement[] achievements;           // 도전과제 목록
    public Achievement achievement;
    public Text _name;                           // 도전과제 이름
    public Text content;                         // 내용

    private UserDataIO.User user;                // 유저 데이터
    private readonly string achievementDataPath = "Data/achievement_data";
    private List<Dictionary<string, object>> achievementData;
    private int achievementCount;

    private void Start()
    {
        user = UserDataIO.ReadUserData();

        LoadAchievementData();
    }

    private void LoadAchievementData()
    {
        achievementData = CSVReader.Read(achievementDataPath);

        achievementCount = achievementData.Count;

        // 28을 더한 이유는 도전과제간 간격을 고려해 더한 것
        achievementList.GetComponent<RectTransform>().sizeDelta 
            = new Vector2(0, (28 + achievement.GetComponent<RectTransform>().rect.height) * achievementCount);

        achievements = new Achievement[achievementCount];

        for (int index = 0; index < achievementCount; index++)
        {
            Achievement _achievement = Instantiate(InitAchievement(index));

            _achievement.transform.SetParent(achievementList.transform);
      
            achievements[index] = _achievement;
        }
    }

    private Achievement InitAchievement(int index)
    {
        Achievement _achievement = achievement;

        _achievement._name = achievementData[index]["Name"].ToString();

        _achievement._content = achievementData[index]["Content"].ToString();

        _achievement.achievementName.text = _achievement._name;

        _achievement.name = _achievement._name;

        return _achievement;
    }

    // 패널을 끌 때 패널 초기화를 위해 함수로 구현했음
    public void PanelOff()
    {
        _name.text = "도전 과제를 선택해 주세요.";

        content.text = "";

        achievementList.GetComponent<RectTransform>().anchoredPosition
            = new Vector2(0, 0);

        gameObject.SetActive(false);
    }

    public void AchievementBtn(Achievement _achievement)
    {
        _name.text = _achievement._name;

        content.text = _achievement._content;
    }
}