﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public GameObject itemInvt; // 아이템들이 포함되어있는 게임오브젝트를 받아옴
    private AmonController player;

    // private bool itemIsActive;

    public GameObject key1; // 각 숫자 키와 그 키에 상속되어있는 아이템들을 받아오는 변수들
    public Item key1Item;
    public GameObject key2;
    public Item key2Item;
    public GameObject key3;
    public Item key3Item;

    public Item[] inventoryData;

    void Start()
    {
        player = gameObject.transform.GetComponent<AmonController>(); // player 캐릭터를 받아옴

        key1 = itemInvt.transform.GetChild(0).gameObject; // key 오브젝트와 keyItem 정보를 받아옴
        key1Item = key1.transform.GetChild(0).gameObject.GetComponent<Item>();

        key2 = itemInvt.transform.GetChild(1).gameObject;
        key2Item = key2.transform.GetChild(0).gameObject.GetComponent<Item>();

        key3 = itemInvt.transform.GetChild(2).gameObject;
        key3Item = key3.transform.GetChild(0).gameObject.GetComponent<Item>();

        inventoryData = new Item[3]; // 나중에 UI를 위해서 InventoryData array를 만들어놓았다.

        inventoryData[0] = key1Item;
        inventoryData[1] = key2Item;
        inventoryData[2] = key3Item;
    }

    void Update() // 각 숫자키를 누를때마다 현재 들고있는 아이템을 변경, 아이템이 없으면 null설정
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ItemSwap(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ItemSwap(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ItemSwap(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) ItemSwap(4);
    }

    void ItemSwap(int itemNum)
    {
        key1.SetActive(false);
        key2.SetActive(false);
        key3.SetActive(false); // 아이템 변경 전 모든 key를 비활성화

        // itemNum에 맞는 숫자키만 활성화, 만약 그 숫자에 상속된 아이템이 없을 경우 keyItem에 null, 아니면 아이템 정보를 넘김
        if (itemNum == 1)
        {
            key1.SetActive(true);
            if (key1Item == null) player.currentItem = null;
            else key1Item.gameObject.SetActive(true);

            player.currentItem = key1Item;
        }

        else if (itemNum == 2)
        {
            key2.SetActive(true); 

            if (key2Item == null) player.currentItem = null;
            else key2Item.gameObject.SetActive(true);

            player.currentItem = key2Item;
        }

        else if (itemNum == 3)
        {
            key3.SetActive(true);
            if (key3Item == null) player.currentItem = null;
            else key3Item.gameObject.SetActive(true);

            player.currentItem = key3Item;
        }
        else if (itemNum == 4) player.currentItem = null; // 4번키는 맨손을 의미함
    }

    public IEnumerator AddItem(Item _item)
    {
        for(int i = 0; i < 3; i++)
        {
            if (inventoryData[i] != null) yield return null; // 인벤토리에 아이템 있으면 그 아이템 창에는 아이템을 집어넣지않음
            else
            {
                if(i == 0) // 아이템을 주우면 위치, 각도를 key1에 맞추고 key1에 상속시킴
                {
                    _item.gameObject.transform.position = key1.gameObject.transform.position;
                    _item.gameObject.transform.rotation = key1.gameObject.transform.rotation;
                    _item.gameObject.transform.parent = key1.transform;

                    //keyItem과 currentItem, inventoryData를 _item값으로 초기화
                    key1Item = _item;
                    player.currentItem = _item;
                    inventoryData[i] = key1Item;

                   yield break; // 아이템을 주우는 행위를 했다면 코루틴에서 탈출
                }
                else if(i == 1)
                {
                    _item.gameObject.transform.position = key2.gameObject.transform.position;
                    _item.gameObject.transform.rotation = key2.gameObject.transform.rotation;
                    _item.gameObject.transform.parent = key2.transform;

                    key2Item = _item;
                    player.currentItem = _item;
                    inventoryData[i] = key2Item;

                    yield break;
                }
                else
                {
                    _item.gameObject.transform.position = key3.gameObject.transform.position;
                    _item.gameObject.transform.rotation = key3.gameObject.transform.rotation;
                    _item.gameObject.transform.parent = key3.transform;

                    key3Item = _item;
                    player.currentItem = _item;
                    inventoryData[i] = key3Item;

                    yield break;
                }
            }
        }

        yield return new WaitForSeconds(0.1f); // space키의 딜레이를 위해서 설정
    }
}