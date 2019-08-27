/****************************************
 * ItemController.cs
 * 제작: 김태윤
 * 아이템 컨트롤러
 * (19,08.02) Item 클래스간 상호작용 수정 (내구도 다 달았을 때 부분)
 * (19.08.03) player 변수 삭제 및 GameManager로 수정, Array이용해서 깔끔하게 정리
 * (19.08.04) 버튼 사용을 위해 ItemSwap 함수 public으로 변환
 * (19.08.20) 아이템 UI에서 개수 추가
 * (19.08.27) 싱글톤으로 만듦
 *함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.26
 * 수정일자: 19.08.20
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    // (19.08.27. 예진) 초기화 때 컨트롤러가 연결되지 않은 것 때문에 오류 발생해서 싱글톤 처리함
    private static ItemController instance;
    public static ItemController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(ItemController)) as ItemController;

                if (instance == null)
                {
                    Debug.LogError("There's no active ItemController object");
                }
            }

            return instance;

        }
    }

    private GameObject itemInvt;            // 아이템들이 포함되어있는 게임오브젝트를 받아옴
    private GameObject itemSlot;            // 아이템 슬롯 UI
    public GameObject[] keys;               // 각 번호키 오브젝트 배열 번호키에 할당된 아이템을 받아오는 배열
    public Item[] keyItems;
    public Item[] inventoryData;
    public Text[] itemCountText;            // UI에 표시할 현재 아이템 개수
    public int TIN;                         // Total Item Number; 필드 내 전체 아이템 종류

    void Start()
    {
        // 필드 내 전체 아이템 종류
        TIN = GameManager.Instance.objects["Item"].Count;

        // (08.03) public 해제하고 GameManager를 통해서 변수에 할당
        itemInvt = GameManager.Instance.Inventory; 
        itemSlot = GameManager.Instance.UI[2];
        keys = new GameObject[TIN];
        keyItems = new Item[TIN];
        inventoryData = new Item[TIN]; // 나중에 UI를 위해서 InventoryData array를 만들어놓았다.
        itemCountText = new Text[TIN];


        for (int i = 0; i < TIN; i++) // 시작 시 각 번호키에 장착된 아이템 할당
        {
            keys[i] = itemInvt.transform.GetChild(i).gameObject;

            keyItems[i] = keys[i].transform.GetChild(0).gameObject.GetComponent<Item>();

            inventoryData[i] = keyItems[i];

            // (19.08.20) 텍스트 연결
            itemCountText[i] = itemSlot.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Text>();
        }
    }

    void Update() // 각 숫자키를 누를때마다 현재 들고있는 아이템을 변경, 아이템이 없으면 null설정
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ItemSwap(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ItemSwap(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ItemSwap(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) ItemSwap(3);
    }

    // 버튼 사용을 위해 public으로 변환
    public void ItemSwap(int itemNum)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].SetActive(false);
        }

        // (용현) 현재 아이템도 일단 없는 상태로 해놓음
        GameManager.Instance.player.currentItem = null;

        // (용현) 플레이어 상태를 Idle로 변경
        GameManager.Instance.player.state = AmonController.InteractionState.Idle;

        // itemNum에 맞는 숫자키만 활성화, 만약 그 숫자에 상속된 아이템이 없을 경우 keyItem에 null, 아니면 아이템 정보를 넘김
        // (용현) 내구도 있을 때만 하도록 수정 -> Item 클래스에서 내구도 다 달았을 때 ItemController 링크된거 끊는 걸 없앰(주석처리함)
        // Axe

        if (itemNum == 3 || keyItems[itemNum] == null) GameManager.Instance.player.currentItem = null; // 4번키, 즉 배열의 3번째는 맨손으로 설정

        else
        {
            if (keyItems[itemNum].durability > 0)
            {
                keys[itemNum].SetActive(true);

                keyItems[itemNum].gameObject.SetActive(true);

                // (용현)맨손이 아닐경우, 플레이어의 인터렉션 상태를 Item으로 변경
                if (keyItems[itemNum] == null) GameManager.Instance.player.state = AmonController.InteractionState.Idle;

                else GameManager.Instance.player.state = AmonController.InteractionState.Item;

                GameManager.Instance.player.currentItem = keyItems[itemNum];
            }

            // (19.08.20) 내구도가 없을 시 아이템을 변경하지 않음
            else return;
        }

        // (용현) 장애물 바로 앞에서 아이템 변경 시 인터렉션 상태 Obstacle로 변경
        if (GameManager.Instance.player.obstacle) GameManager.Instance.player.state = AmonController.InteractionState.Obstacle;
    }

    // 이거 왜 있는지 모르겠음
    public IEnumerator AddItem(Item _item)
    {
        for (int i = 0; i < 3; i++)
        {
            if (inventoryData[i] != null) yield return null; // 인벤토리에 아이템 있으면 그 아이템 창에는 아이템을 집어넣지않음
            else // 위치, 회전, 상속을 부모 오브젝트에 설정함
            {
                _item.gameObject.transform.position = keys[i].gameObject.transform.position;
                _item.gameObject.transform.rotation = keys[i].gameObject.transform.rotation;
                _item.gameObject.transform.parent = keys[i].transform;

                keyItems[i] = _item;
                GameManager.Instance.player.currentItem = _item;
                inventoryData[i] = keyItems[i];

                yield break;
            }
        }
        yield return new WaitForSeconds(0.1f); // space키의 딜레이를 위해서 설정
    }

    // (예진) 19.08.05. 아이템 내구도 다 닳았을 때 아이템 키에서 제거하고 슬롯에서 보이지 않게 함
    public void DeleteItemKey(Item item)
    {
        int itemNum = -1;

        for (int i = 0; i < TIN; i++)
        {
            if (keyItems[i] != null && keyItems[i].Equals(item))
            {
                itemNum = i;
                break;
            }
        }

        if (itemNum > -1)
        {
            itemSlot.transform.GetChild(itemNum).GetChild(0).gameObject.SetActive(false);

            // 연결 해제할 필요 없음.
            // keyItems[itemNum] = null;
        }
        else
            Debug.LogError("아이템 비활성화 오류");
    }

    public void UpdateItemCount(Item item)
    {
        int itemNum = -1;

        for (int i = 0; i < TIN; i++)
        {
            if (keyItems[i] != null && keyItems[i].Equals(item))
            {
                itemNum = i;
                break;
            }
        }

        if (itemNum > -1) itemCountText[itemNum].text = keyItems[itemNum].durability.ToString();
        
        else Debug.LogError("아이템 비활성화 오류");
    }
}
