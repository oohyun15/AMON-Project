/****************************************
 * Reset.cs
 * 제작: 조예진
 * 초기화 스크립트
 * (19.10.13) ㅇㅈ> 디버깅 용 돈/명예 추가 기능
 * (19.10.29) ㅇㅎ> 스테이지 데이터 초기화 추가
 * 작성일자: 19.10.08.
 * 수정일자: 19.10.29.
 ***************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Reset : MonoBehaviour
{

    public GameObject panel;

    public void OnClickResetBtn()
    {
        /*
         * 리셋해야 할 것
         * 돈, 명예, 계급, 피로도, 아이템 데이터, 스테이지 클리어 데이터(아직 저장 안함)
         * 
         * 리셋 안 할 것
         * ?
         */

        UserDataIO.User userData = UserDataIO.ReadUserData();

        // 추가 보상 업데이트
        int addReward = System.Convert.ToInt32(ItemDataManager.Instance.GetRankData()[userData.rank]["add_reward"]);
        userData.addReward = addReward;

        // 데이터 리셋
        userData.money = 0;
        userData.honor = 0;
        userData.stress = 0;
        userData.rank = 0;

        // 아이템 레벨 데이터 리셋
        userData.oxygenlv = 0;
        userData.gloveslv = 0;
        userData.axelv = 0;


        UserDataIO.WriteUserData(userData);

        panel.SetActive(true);
    }

    /* 테스트 버전 용 함수 */

    public void ResetUserData()
    {
        try
        {
            File.Delete(Application.persistentDataPath + "/Data/userData.xml");
        }
        catch (Exception e) { }
    }

    public void ResetItemData()
    {
        UserDataIO.User userData = UserDataIO.ReadUserData();

        // 아이템 레벨 데이터 리셋
        userData.oxygenlv = 0;
        userData.gloveslv = 0;
        userData.axelv = 0;

        UserDataIO.WriteUserData(userData);
    }

    public void ResetTutorialData()
    {
        PlayerPrefs.DeleteKey("isPlayedTutorial");
    }

    public void ResetStageData()
    {
        try
        {
            File.Delete(Application.persistentDataPath + "/Data/stageData.xml");
        }
        catch (Exception e) { }
    }

    public void AddMoney()
    {
        UserDataIO.User user = UserDataIO.ReadUserData();

        user.money += 1000000;

        UserDataIO.WriteUserData(user);
    }

    public void AddHonor(int honor)
    {
        UserDataIO.User user = UserDataIO.ReadUserData();

        user.honor += honor;

        UserDataIO.WriteUserData(user);
    }

    public void MoveScene(string sceneName)
    {
        Lobby.MoveScene(sceneName);
    }
}
