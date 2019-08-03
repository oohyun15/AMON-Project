/****************************************
 * Item.cs
 * 제작: 김태윤
 * 아이템
 * (19,08.02) Item 내구도 다 썻을 때 Current Item 변경
 * (08.03) player 수정 및 GameManager로 수정, Start(), GetInitValue() 삭제
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.26
 * 수정일자: 19.08.03
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Item들의 기본 속성을 가져 인터페이스로 사용될 클래스
public class Item : MonoBehaviour
{
    public int initDurability;
    public int durability;                              // 내구도

    private GameManager gm;
    private new readonly string name = "Item";
    protected AmonController player;                    // 플레이어

    void Start()
    {
        gm = GameManager.Instance;

        var go = new List<GameObject>();

        // Object에 키가 있으면 추가
        if (gm.temp.ContainsKey(name))
            gm.temp[name].Add(gameObject);

        // 키가 없을 경우 생성
        else
        {
            gm.temp.Add(name, go);

            gm.temp[name].Add(gameObject);
        }

        GetInitValue();
    }


    public virtual void ItemActive() // 각 Item의 기능은 이 함수를 상속받아서 사용
    {

    }

    protected void DurabilityManage() // 내구도 관리
    {
        durability -= 1;
        if (durability <= 0) // 내구도 다달면 keyItem을 null 설정
        {
            gameObject.SetActive(false);

            // (용현) 19.07.30 아이템 사용 후 플레이어 상태 Idle로 변경
            GameManager.Instance.player.state = AmonController.InteractionState.Idle;

            GameManager.Instance.player.currentItem = null;
        }
    }

    public void SetInitValue()
    {
        // 아이템 부모 오브젝트를 비활성화 시켜야함
        gameObject.SetActive(false);

        durability = initDurability;
    }
}
