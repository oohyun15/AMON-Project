/****************************************
 * UserDataIO.cs
 * 제작: 조예진
 * 유저 데이터 관리
 * (19,08.25) 플레이 횟수 추가
 * (19.09.15) User 클래스에 옵저버 패턴 추가
 * (19.10.04) 도전과제 추가
 * (19.10.10) 랭크 추가
 * (19.10.12) Stage 클래스 추가
 *함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.0X.XX
 * 수정일자: 19.10.12
 ***************************************/

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
        public int money;                   // 소지 금액
        public int honor;                   // 소지 명예 점수
        public int stress;                  // 피로도
        public int rank;

        /* 도전과제 카운트 변수 */
        public int playCount;               // 플레이 횟수
        public int clearCount;              // 클리어 횟수
        public int rescuedCount;             // 구출 횟수
        public int destroyCount;            // 장애물 파괴 횟수
        public int resetCount;              // 게임 초기화 횟수
        public int deathCount;              // 장애물에 맞아서 죽은 횟수

        
        /* 장착 아이템 레벨 */
        public int oxygenlv;                // 산소통 레벨
        public int gloveslv;                // 장갑 레벨
        public int axelv;

        public int[] achievementList;       // 도전과제 목록
        // 저장할 값 늘어날 경우, WriteUserData와 ReadUserData에서 Set/GetAttribute 설정해 주어야 합니다
    }

    public class Stage
    {
        public int[] isPlayed;
        public int[] rescueNum;
    }

    private readonly static string userDataFileName = "/userData.xml";
    private readonly static string stageDataFileName = "/stageData.xml";
    private readonly static string achievementDataPath = "Data/achievements_data";
    private readonly static string stageDataPath = "Data/stage_data";
    private static List<Dictionary<string, object>> achievementData, stageData;
    public static int achievementCount = 0;
    public static int stageCount = 0;

    // 유저 데이터 저장 시
    public static void WriteUserData(User user)
    {
        string path = GetPath(userDataFileName);

        XmlDocument doc = new XmlDocument();
        XmlElement userElement = doc.CreateElement("user");

        userElement.SetAttribute("money", user.money.ToString());
        userElement.SetAttribute("honor", user.honor.ToString());
        userElement.SetAttribute("stress", user.stress.ToString());
        userElement.SetAttribute("rank", user.rank.ToString());

        /* 도전과제 카운트 변수 */
        userElement.SetAttribute("playCount", user.playCount.ToString());
        userElement.SetAttribute("clearCount", user.clearCount.ToString());
        userElement.SetAttribute("rescuedCount", user.rescuedCount.ToString());
        userElement.SetAttribute("destroyCount", user.destroyCount.ToString());
        userElement.SetAttribute("resetCount", user.resetCount.ToString());
        userElement.SetAttribute("deathCount", user.deathCount.ToString());

        /* 장착 아이템 레벨 */
        userElement.SetAttribute("oxygenlv", user.oxygenlv.ToString());
        userElement.SetAttribute("gloveslv", user.gloveslv.ToString());
        userElement.SetAttribute("axelv", user.axelv.ToString());

        /* 도전과제 달성 여부 */
        for(int index = 0; index < achievementCount; index++)
        {
            string number = "achievementList" + index.ToString();

            userElement.SetAttribute(number, user.achievementList[index].ToString());
        }

        doc.AppendChild(userElement);
        doc.Save(path);
    }

    public static User ReadUserData()
    {
        string path = GetPath(userDataFileName);

        User user;

        if (achievementCount == 0) achievementCount = FindCount(achievementData, achievementDataPath);

        if (!File.Exists(path))
        {
            user = new User
            {
                money = 0,
                honor = 0,
                stress = 0,
                rank = 0,

                /* 도전과제 카운트 변수 */
                playCount = 0,
                clearCount = 0,
                rescuedCount = 0,
                destroyCount = 0,
                resetCount = 0,
                deathCount = 0,

                /* 장착 아이템 레벨 */
                oxygenlv = 0,
                gloveslv = 0,
                axelv = 0,

                /* 도전과제 달성 여부 */
                achievementList = new int[achievementCount]
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
                stress = userElement.HasAttribute("stress") ? System.Convert.ToInt32(userElement.GetAttribute("stress")) : 0,
                rank = userElement.HasAttribute("rank") ? System.Convert.ToInt32(userElement.GetAttribute("rank")) : 0,

                /* 도전과제 카운트 변수 */
                playCount = userElement.HasAttribute("playCount") ? System.Convert.ToInt32(userElement.GetAttribute("playCount")) : 0,
                clearCount = userElement.HasAttribute("clearCount") ? System.Convert.ToInt32(userElement.GetAttribute("clearCount")) : 0,
                rescuedCount = userElement.HasAttribute("rescuedCount") ? System.Convert.ToInt32(userElement.GetAttribute("rescuedCount")) : 0,
                destroyCount = userElement.HasAttribute("destroyCount") ? System.Convert.ToInt32(userElement.GetAttribute("destroyCount")) : 0,
                resetCount = userElement.HasAttribute("resetCount") ? System.Convert.ToInt32(userElement.GetAttribute("resetCount")) : 0,
                deathCount = userElement.HasAttribute("deathCount") ? System.Convert.ToInt32(userElement.GetAttribute("deathCount")) : 0,

                /* 장착 아이템 레벨 */
                oxygenlv = userElement.HasAttribute("oxygenlv") ? System.Convert.ToInt32(userElement.GetAttribute("oxygenlv")) : 0,
                gloveslv = userElement.HasAttribute("gloveslv") ? System.Convert.ToInt32(userElement.GetAttribute("gloveslv")) : 0,
                axelv = userElement.HasAttribute("axelv") ? System.Convert.ToInt32(userElement.GetAttribute("axelv")) : 0,

                /* 도전과제 달성 여부 */
                achievementList = new int[achievementCount]
            };

            // 도전과제 달성 여부 확인
            for (int index =0; index < achievementCount; index++)
            {
                string number = "achievementList" + index.ToString();

                user.achievementList[index] = userElement.HasAttribute(number) ? System.Convert.ToInt32(userElement.GetAttribute(number)) : 0;
            }
        }

        // user.observer = GameManager.Instance.observer;

        return user;
    }

    public static void WriteStageData(Stage stage)
    {
        string path = GetPath(stageDataFileName);

        XmlDocument doc = new XmlDocument();
        XmlElement userElement = doc.CreateElement("stage");

        for (int index = 0; index < stageCount; index++)
        {
            string number = "Stage" + index.ToString() + "RescueNumber";
            string name = "Stage" + index.ToString() + "isPlayed";

            userElement.SetAttribute(number, stage.rescueNum[index].ToString());
            userElement.SetAttribute(name, stage.isPlayed[index].ToString());
        }

        doc.AppendChild(userElement);
        doc.Save(path);
    }

    public static Stage ReadStageData()
    {
        string path = GetPath(stageDataFileName);

        Stage stage;

        if (stageCount == 0) stageCount = FindCount(stageData, stageDataPath);

        if (!File.Exists(path))
        {
            stage = new Stage
            {
                isPlayed = new int[stageCount],
                rescueNum = new int[stageCount]
            };

            WriteStageData(stage);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlElement userElement = doc["stage"];

            stage = new Stage
            {
                isPlayed = new int[stageCount],
                rescueNum = new int[stageCount]
            };

            // 도전과제 달성 여부 확인
            for (int index = 0; index < stageCount; index++)
            {
                string number = "Stage" + index.ToString() + "RescueNumber";
                string name = "Stage" + index.ToString() + "isPlayed";

                stage.rescueNum[index] = userElement.HasAttribute(number) ? System.Convert.ToInt32(userElement.GetAttribute(number)) : 0;
                stage.isPlayed[index] = userElement.HasAttribute(name) ? System.Convert.ToInt32(userElement.GetAttribute(name)) : 0;
            }
        }
        return stage;
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

    // 유저 피로도 차감, 증가 - false return 시 게임 오버
    public static bool ChangeUserStress(int change)
    {
        User user = ReadUserData();

        if (user.stress + change < 0)
        {
            user.stress = 0;

            WriteUserData(user);

            return true;
        }

        else if (user.stress + change > 100)
        {
            user.stress = 100;

            WriteUserData(user);

            return false;
        }

        user.stress += change;

        WriteUserData(user);

        Debug.Log("현재 피로도 : " + ReadUserData().stress);
        
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

    private static int FindCount(List<Dictionary<string, object>> data, string path)
    {
        data = CSVReader.Read(path);

        return data.Count;
    }
}