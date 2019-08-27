/****************************************
 * Item.cs
 * 제작: 김태윤
 * 아이템
 * (19,08.02) Item 내구도 다 썻을 때 Current Item 변경
 * (19.08.03) player 수정 및 GameManager로 수정, Start(), GetInitValue() 삭제
 * (19.08.20) Item 번호 추가
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.26
 * 수정일자: 19.08.03
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Item들의 기본 속성을 가져 인터페이스로 사용될 클래스
public class Item : MonoBehaviour, IReset
{
    public int ID_num;                                  // 아이템 번호
    public int initDurability;
    public int durability;                              // 내구도

    private GameManager gm;
    private ItemController controller;
    private new readonly string name = "Item";

    // (예진) 아이템 키 제거를 위해 컨트롤러가 아이템 가져올 때 컨트롤러도 설정하도록 함
    // (19.08.27. 예진) 초기화 때 컨트롤러가 연결되지 않은 것 때문에 오류 발생해서 컨트롤러를 싱글톤 처리함
    // public void SetController(ItemController controller) { this.controller = controller; }

    void Start()
    {
        gm = GameManager.Instance;
        controller = ItemController.Instance;

        var go = new List<GameObject>();

        // Object에 키가 있으면 추가
        if (gm.objects.ContainsKey(name))
            gm.objects[name].Add(gameObject);

        // 키가 없을 경우 생성
        else
        {
            gm.objects.Add(name, go);

            gm.objects[name].Add(gameObject);
        }

       // GetInitValue();
    }

    public virtual void ItemActive() // 각 Item의 기능은 이 함수를 상속받아서 사용
    {
        DurabilityManage();

        controller.UpdateItemCount(this);
    }

    protected void DurabilityManage() // 내구도 관리
    {
        durability -= 1;
        if (durability <= 0) // 내구도 다달면 keyItem을 null 설정
        {
            DestroyItem();
        }
    }

    // (예진) 19.08.05. 아이템 내구도 0 이하가 되었을 때 메소드화 + 슬롯에서 사라지게 하는 것 추가
    public void DestroyItem()
    {
        gameObject.SetActive(false);

        // (용현) 19.07.30 아이템 사용 후 플레이어 상태 Idle로 변경
        gm.player.state = AmonController.InteractionState.Idle;

        gm.player.currentItem = null;

        // (19.08.20) 아이콘 사라지지 않게 함
        // controller.DeleteItemKey(this);
    }

    public void GetInitValue()
    {

    }

    public void SetInitValue()
    {
        // 아이템 부모 오브젝트를 비활성화 시켜야함
        gameObject.SetActive(false);

        durability = initDurability;

        controller.UpdateItemCount(this);
    }
}
