/****************************************
 * Observer.cs
 * 제작: 김용현
 * 인게임 내에서 도전과제 달성여부 체크하는 옵저버
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.09.15
 * 수정일자: 19.09.15
 ***************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour, IObserver
{
    public UserDataIO.User user;
    public Dictionary<string, List<GameObject>> objects;

    private readonly string AchievementDataPath = "Data/achievement_data";
    private List<Dictionary<string, object>> achievementData;

    public void UpdateFieldData(Dictionary<string, List<GameObject>> objects)
    {
        this.objects = objects;
    }

    public void UpdateUserData(UserDataIO.User user)
    {
        this.user = user;
    }

    private void SaveResult()
    {
        
    }
}
