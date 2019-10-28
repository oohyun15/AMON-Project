/****************************************
* AcheivementController.cs
* 제작: 김용현
* 로비 도전과제 패널 컨트롤러
* 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
* (19.09.15) 도전과제 패널 스크롤 형태로 변경
* (19.10.29) 도전과제 이미지 추가
* 작성일자: 19.08.25
* 수정일자: 19.10.29
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
    public Text info;
    public Sprite[] achievementImage;            // 도전과제 이미지

    private UserDataIO.User user;                // 유저 데이터
    public readonly static string achievementDataPath = "Data/achievements_data";
    private List<Dictionary<string, object>> achievementData;
    private int achievementCount;
    private Color initColor;

    private void Start()
    {
        user = UserDataIO.ReadUserData();

        initColor = achievement._icon.color;

        LoadAchievementData();
    }

    private void LoadAchievementData()
    {
        achievementData = CSVReader.Read(achievementDataPath);

        achievementCount = achievementData.Count;

        // 28을 더한 이유는 도전과제간 간격을 고려해 더한 것
        achievementList.GetComponent<RectTransform>().sizeDelta
            = new Vector2(0, (28 + achievement.GetComponent<RectTransform>().rect.height) * (achievementCount + 1));

        achievements = new Achievement[achievementCount];

        for (int index = 0; index < achievementCount; index++)
        {
            Achievement _achievement = Instantiate(InitAchievement(index));

            if (user.achievementList[index] == 1) _achievement._icon.color = Color.red;

            _achievement.transform.SetParent(achievementList.transform);

            achievements[index] = _achievement;
        }
    }

    private Achievement InitAchievement(int index)
    {
        Achievement _achievement = achievement;

        _achievement.index = index;

        _achievement._name = achievementData[index]["Name"].ToString();

        _achievement._content = achievementData[index]["Content"].ToString();

        _achievement._info = achievementData[index]["Condition"].ToString();

        _achievement.achievementName.text = _achievement._name;

        _achievement.name = _achievement._name;

        _achievement._icon.sprite = achievementImage[index];

        return _achievement;
    }

    // 패널을 끌 때 패널 초기화를 위해 함수로 구현했음
    public void PanelOff()
    {
        InitText();

        achievementList.GetComponent<RectTransform>().anchoredPosition
            = new Vector2(0, 0);

        gameObject.SetActive(false);
    }

    public void AchievementBtn(Achievement _achievement)
    {
        int index = _achievement.index;

        _name.text = _achievement._name;

        content.text = _achievement._content;

        switch (index)
        {
            case 2:
                info.text = user.rescuedCount + " / " + _achievement._info;
                break;

            case 3:
                info.text = user.destroyCount + " / " + _achievement._info;
                break;

            case 4:
                info.text = user.playCount + " / " + _achievement._info;
                break;

            case 5:
                info.text = user.deathCount + " / " + _achievement._info;
                break;

            default:
                info.text = "";
                break;
        }
    }

    public void ResetAchievement()
    {
        for (int index = 0; index < achievementCount; index++)
        {
            if (user.achievementList[index] == 1)
            {
                user.achievementList[index] = 0;

                // 하드코딩
                achievementList.transform.GetChild(index + 1).GetComponent<Achievement>()._icon.color = initColor;
            }
        }

        user.playCount = 0;
        user.clearCount = 0;
        user.rescuedCount = 0;
        user.destroyCount = 0;
        user.resetCount = 0;
        user.deathCount = 0;

        UserDataIO.WriteUserData(user);

        InitText();
    }

    private void InitText()
    {
        _name.text = "도전 과제를 선택해주세요.";

        content.text = "";

        info.text = "";
    }
}