using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class UserDataIO : MonoBehaviour
{
    // 유저 데이터로 저장할 값 클래스
    public class User
    {
        public int money;       // 소지 금액
        public int honor;       // 소지 명예 점수
        // 저장할 값 늘어날 경우, WriteUserData와 ReadUserData에서 Set/GetAttribute 설정해 주어야 합니다
    }

    private readonly static string userDataFileName = "/userData.xml";

    // 유저 데이터 저장 시
    public static void WriteUserData(User user)
    {
        string path = GetPath(userDataFileName);

        XmlDocument doc = new XmlDocument();
        XmlElement userElement = doc.CreateElement("user");

        userElement.SetAttribute("money", user.money.ToString());
        userElement.SetAttribute("honor", user.honor.ToString());

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
                honor = 0
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
                money = System.Convert.ToInt32(userElement.GetAttribute("money")),
                honor = System.Convert.ToInt32(userElement.GetAttribute("honor"))
            };
        }

        return user;
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
}
