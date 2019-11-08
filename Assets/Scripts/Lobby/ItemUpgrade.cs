/****************************************
 * ItemUpgrade.cs
 * 제작: 조예진
 * 아이템 업그레이드 기능 스크립트
 * 테스트를 위해 우선 산소통만 업그레이드 할 수 있도록 설정
 * 테스트를 위해 아이템 데이터는 PlayerPrefs를 통해 저장됨
 * -> 아이템 업그레이드 데이터를 기존 UserData와 함께 저장할지? 혹은 새 파일 생성?
 * 작성일자: 19.08.25.
 ***************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUpgrade : MonoBehaviour
{
    Dictionary<string, object> itemDataList;

    public GameManager.UISet[] content;

    private Lobby lobby;

    private void Start()
    {
        lobby = GetComponent<Lobby>();

        itemDataList = ItemDataManager.Instance.GetEquipItemData();

        // 업그레이드 스크롤 뷰 업데이트
        UpdateEquipViewport();
    }

    public void UpgradeItem(string item)
    {
        UserDataIO.User user = UserDataIO.ReadUserData();

        // 아이템 현재 레벨 불러오기
        // int lv = PlayerPrefs.GetInt(item + "lv", 0);
        int lv = 0;

        switch (item)
        {
            case "oxygen":
                lv = user.oxygenlv;
                break;

            case "gloves":
                lv = user.gloveslv;
                break;

            case "axe":
                lv = user.axelv;
                break;

            case "shoes":
                lv = user.shoeslv;
                break;                  
        }

        int maxLv = ((List<Dictionary<string, object>>)itemDataList[item]).Count;


        // 아이템 다음 레벨 구입 가격 불러오기
        if (lv < maxLv)
        {
            int price = System.Convert.ToInt32(GetDataValue(item, lv + 1, "price"));

            Debug.Log(price);

            // 돈 부족할 경우 업그레이드 되지 않음 
            if (!UserDataIO.ChangeUserMoney(-price))
            {
                StartCoroutine(lobby.Notify("업그레이드 실패\n소지 금액 부족"));
            }
            // 돈 충분할 경우 업그레이드 완료
            else
            {
                user.money -= price;

                // PlayerPrefs.SetInt(item + "lv", lv + 1);
                switch (item)
                {
                    case "oxygen":
                        user.oxygenlv++;
                        break;

                    case "gloves":
                        user.gloveslv++;
                        break;

                    case "axe":
                        user.axelv++;
                        break;

                    case "shoes":
                        user.shoeslv ++;
                        break;
                }

                UserDataIO.WriteUserData(user);

                StartCoroutine(lobby.Notify("업그레이드 성공\n현재 " + GetDataValue(item, 1, "name") + " 아이템 레벨은 " + (lv + 1)));

                UpdateEquipViewport();

                lobby.SetUIText();
            }
        }
        // 최대 강화 상태일 경우
        else
        {
            StartCoroutine(lobby.Notify(
                "아이템이 최대 강화 상태입니다\n현재 " + GetDataValue(item, 1, "name")+ " 아이템 레벨은 " + lv)
            );
        }
    }

    public void SetContent(GameManager.UISet content)
    {
        UserDataIO.User userData = UserDataIO.ReadUserData();

        string item = content.name;
        int lv = 0;

        switch (content.name)
        {
            case "oxygen":
                lv = userData.oxygenlv;
                break;

            case "gloves":
                lv = userData.gloveslv;
                break;

            case "axe":
                lv = userData.axelv;
                break;
            case "shoes":
                lv = userData.shoeslv;
                break;
        }

        int maxLv = ((List<Dictionary<string, object>>)itemDataList[item]).Count;

        int rank = 0;

        if (lv != maxLv)
            rank = System.Convert.ToInt32(GetDataValue(item, lv + 1, "rank"));

        content.UI.transform.GetChild(3).gameObject.SetActive(false);

        // 최대 레벨일 경우 강화 버튼 사용 불가능
        if (lv >= maxLv)
        {
            content.UI.transform.GetChild(2).GetComponent<Button>().interactable = false;
        }
        // 계급 조건보다 낮을 시 업그레이드 잠금
        else if (userData.rank < rank)
        {
            content.UI.transform.GetChild(3).gameObject.SetActive(true);
            content.UI.transform.GetChild(3).GetChild(0).GetComponent<Text>().text
                = ItemDataManager.Instance.rankData[rank]["name"].ToString() + " 이상 구입 가능";
        }

        // 아이템 설명 텍스트 설정
        string nowLv;

        if (lv == 0)
            nowLv = "";
        else
            nowLv = "\n현재 레벨 : " + lv + " 효과 : " + GetDataValue(item, lv, "effect");

        string nextLv;

        if (lv == maxLv)
            nextLv = "\n최대 레벨입니다";
        else
            nextLv = "\n다음 레벨 : " + (lv + 1) + " 효과 : " + GetDataValue(item, lv + 1, "effect");



        content.UI.transform.GetChild(1).GetComponent<Text>().text =
            GetDataValue(item, 1, "name") +
            nowLv +
            nextLv;
    }

    private object GetDataValue(string item, int lv, string key)
    {
        return ((List<Dictionary<string, object>>)itemDataList[item])[lv - 1][key];
    }

    public void UpdateEquipViewport()
    {
        foreach (GameManager.UISet go in content)
            SetContent(go);
    }
}
