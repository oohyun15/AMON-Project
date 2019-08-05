/****************************************
 * Obstacle.cs
 * 제작: 김태윤
 * 장애물 관련 함수
 * (19.07.30) 플레이어 제거되지 않게 수정 및 카메라를 정상적으로 찾게 수정
 * (19.08.03) IRset 인터페이스 추가
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.09
 * 수정일자: 19.08.03
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IReset
{
    public bool initActive;                      // 초기에 필드 내에 장애물이 활성화 됐는지
    public int initHp;
    public int hp;                              // 장애물 체력

    private GameManager gm;
    private new readonly string name = "Obstacle";

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

        GetInitValue();
    }

    public void GetInitValue()
    {
        Rigidbody rgbd = transform.GetComponent<Rigidbody>();

        //mass를 최대로 높여 Amon과 장애물이 부딪혀도 장애물은 움직이지 않게 함
        if (rgbd != null) rgbd.mass = Mathf.Infinity;
    }

    public void SetInitValue()
    {
        hp = initHp;

        // 처음에 활성화 됐었으면 다시 활성화, 그렇지 않으면 비활성화
        if (initActive) gameObject.SetActive(true);

        else gameObject.SetActive(false);
    }
}
