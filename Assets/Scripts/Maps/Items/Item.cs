using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour // Item들의 기본 속성을 가져 인터페이스로 사용될 클래스
{
    public int durability; // 내구도

    protected AmonController player; // 플레이어
    protected ItemController inventory; // ItemController에 의해 아이템 사용이 관리됨

    void Start()
    {
        if(transform.parent != null && player == null)
        {
           player = transform.parent.parent.parent.gameObject.transform.GetComponent<AmonController>();
           inventory = transform.parent.parent.parent.gameObject.transform.GetComponent<ItemController>();
        }
       
    }

    void Update()
    {
        if (transform.parent != null && player == null)
        {
            player = transform.parent.parent.parent.gameObject.transform.GetComponent<AmonController>();
            inventory = transform.parent.parent.parent.gameObject.transform.GetComponent<ItemController>();
        }
    }

    public virtual void ItemActive() // 각 Item의 기능은 이 함수를 상속받아서 사용
    {

    }

    protected void DurabilityManage() // 내구도 관리
    {
        durability -= 1;
        if (durability <= 0) // 내구도 다달면 keyItem을 null 설정
        {
            if (transform.parent.transform.name == "key1") inventory.key1Item = null;
            else if (transform.parent.transform.name == "key2") inventory.key2Item = null;
            else inventory.key3Item = null;

            Destroy(gameObject);

            // (용현) 19.07.30 아이템 사용 후 플레이어 상태 Idle로 변경
            player.state = AmonController.InteractionState.Idle;
        }
    }
}
