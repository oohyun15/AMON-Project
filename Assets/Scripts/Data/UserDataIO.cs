/****************************************
 * UserDataIO.cs
 * 제작: 조예진
 * 유저 데이터 관리
 * (19,08.25) 플레이 횟수 추가
 * (19.09.08) 도전과제 구현을 위한 옵저버 패턴 추가(ISubject)
 *함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.0X.XX
 * 수정일자: 19.08.25
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class UserDataIO : MonoBehaviour, ISubject
{
    // 유저 데이터로 저장할 값 클래스
    public class User
    {
        public int money;                   // 소지 금액
        public int honor;                   // 소지 명예 점수
        public int playCount;               // 플레이 횟수
        public int clearCount;              // 클리어 횟수
        // 저장할 값 늘어날 경우, WriteUserData와 ReadUserData에서 Set/GetAttribute 설정해 주어야 합니다
    }

    private readonly static string userDataFileName = "/userData.xml";
    private User _user;                     // 옵저버 패턴 시 사용되는 user

    // 유저 데이터 저장 시
    public static void WriteUserData(User user)
    {
        string path = GetPath(userDataFileName);

        XmlDocument doc = new XmlDocument();
        XmlElement userElement = doc.CreateElement("user");

        userElement.SetAttribute("money", user.money.ToString());
        userElement.SetAttribute("honor", user.honor.ToString());
        userElement.SetAttribute("playCount", user.playCount.ToString());
        userElement.SetAttribute("clearCount", user.clearCount.ToString());

        doc.AppendChild(userElement);
        doc.Save(path);
    }

    public static User ReadUserData()
    {
        string path = GetPath(userDataFileName);

        User user = new User();

        if (!File.Exists(path))
        {
            user = new User
            {
                money = 0,
                honor = 0,
                playCount = 0,
                clearCount = 0
            };
            WriteUserData(user);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlElement userElement = doc["user"];

            user = new User
            {
                money = userElement.HasAttribute("money") ? System.Convert.ToInt32(userElement.GetAttribute("money")) : 0,
                honor = userElement.HasAttribute("honor") ? System.Convert.ToInt32(userElement.GetAttribute("honor")) : 0,
                playCount = userElement.HasAttribute("playCount") ? System.Convert.ToInt32(userElement.GetAttribute("playCount")) : 0,
                clearCount = userElement.HasAttribute("clearCount") ? System.Convert.ToInt32(userElement.GetAttribute("clearCount")) : 0,
            };
        }

        return user;
    }

    // 유저 소지액 차감, 증가
    public static bool ChangeUserMoney(int change)
    {
        User user = ReadUserData();

        if (user.money + change < 0) return false;

        user.money += change;

        WriteUserData(user);

        return true;
    }

    private static string GetPath(string fileName)
    {
        string path = Application.persistentDataPath + "/Data";       // Assets 경로 안에 저장됨 (테스트)
        DirectoryInfo di = new DirectoryInfo(path);

        // 경로 존재하는지 확인하고 Data 폴더 없으면 생성
        if (!di.Exists)
            di.Create();
        // Application.persistentDataPath;                  // --> 로컬 저장소에 저장

        path += fileName;

        Debug.Log(fileName + " 저장 경로 : " + path);

        return path;
    }

    void ISubject.RegisterObserver(IObserver observer)
    {
        AchievementController.instance.observers.Add(observer);
    }

    void ISubject.RemoveObserver(IObserver observer)
    {
        if (AchievementController.instance.observers.Contains(observer))
            AchievementController.instance.observers.Remove(observer);
    }

    void ISubject.NotifyObservers()
    {
        foreach (var obs in AchievementController.instance.observers)
            obs._update(_user);
    }
}