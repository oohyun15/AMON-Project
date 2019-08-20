/****************************************
 * FieldItem.cs
 * 제작: 김용현
 * 인게임 내에서 획득할 수 있는 아이템의 값을 가짐
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.08.20
 * 수정일자: 19.08.20
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour, IReset
{
    private GameManager gm;
    public int ID_num;                  // 아이템 번호
    public int itemCount;               // 아이템 사용 횟수
    private new readonly string name = "F_Item";

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetInitValue()
    {

    }

    public void SetInitValue()
    {
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }
}
