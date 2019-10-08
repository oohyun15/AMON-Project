using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        // 데이터 리셋
        userData.money = 0;
        userData.honor = 0;
        userData.stress = 0;

        // 아이템 레벨 데이터 리셋
        userData.oxygenlv = 0;
        userData.gloveslv = 0;
        userData.axelv = 0;

        UserDataIO.WriteUserData(userData);

        panel.SetActive(true);
    }

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

    public void MoveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
