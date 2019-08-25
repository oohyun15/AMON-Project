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
    List<Dictionary<string, object>> itemDataList;

    public GameManager.UISet[] content;

    private Lobby lobby;

    private void Start()
    {
        lobby = GetComponent<Lobby>();

        itemDataList = ItemDataManager.Instance.GetEquipItemData();

        // 업그레이드 스크롤 뷰 업데이트
        SetContent();
    }

    public void UpgradeItem(string item)
    {
        // 아이템 현재 레벨 불러오기
        int lv = PlayerPrefs.GetInt(item + "lv", 0);

        // 아이템 다음 레벨 구입 가격 불러오기
        if (lv < itemDataList.Count)
        {
            int price = System.Convert.ToInt32(itemDataList[lv]["price"]);

            // 돈 부족할 경우 업그레이드 되지 않음 
            if (!UserDataIO.ChangeUserMoney(-price))
            {
                StartCoroutine(lobby.Notify("업그레이드 실패\n소지 금액 부족"));
            }
            // 돈 충분할 경우 업그레이드 완료
            else
            {
                PlayerPrefs.SetInt(item + "lv", lv + 1);

                StartCoroutine(lobby.Notify("업그레이드 성공\n현재 " + itemDataList[0]["name"] + " 아이템 레벨은 " + (lv + 1)));

                SetContent();
            }
        }
        // 최대 강화 상태일 경우
        else
        {
            StartCoroutine(lobby.Notify("아이템이 최대 강화 상태입니다\n현재 " + itemDataList[0]["name"] + " 아이템 레벨은 " + lv));
        }
    }

    public void SetContent()
    {
        UserDataIO.User userData = UserDataIO.ReadUserData();

        int lv = PlayerPrefs.GetInt("oxygen" + "lv", 0);
        int honor = System.Convert.ToInt32(itemDataList[lv]["honor"]);

        content[3].UI.SetActive(false);

        // 최대 레벨일 경우 강화 버튼 사용 불가능
        if (lv >= itemDataList.Count)
        {
            content[2].UI.GetComponent<Button>().interactable = false;
        }
        // 명예 점수 부족할 시 업그레이드 잠금
        else if (userData.honor < honor)
        {
            content[3].UI.SetActive(true);
            content[3].UI.transform.GetChild(0).GetComponent<Text>().text
                = "명예점수 " + honor + "점 필요";
        }

        // 아이템 설명 텍스트 설정
        string nowLv;

        if (lv == 0)
            nowLv = "";
        else
            nowLv = "\n현재 레벨 : " + lv + " 효과 : " + itemDataList[lv - 1]["effect"];

        string nextLv;

        if (lv >= itemDataList.Count)
            nextLv = "\n최대 레벨입니다";
        else
            nextLv = "\n다음 레벨 : " + (lv + 1) + " 효과 : " + itemDataList[lv]["effect"];

        content[1].UI.GetComponent<Text>().text =
            itemDataList[0]["name"] +
            nowLv +
            nextLv;
    }
}
