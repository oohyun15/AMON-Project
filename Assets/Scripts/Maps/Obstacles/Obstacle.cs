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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            // 충돌 위치를 알아내기 위한 코드, 유니티에 collision.contacts 검색
            foreach (ContactPoint contact in collision.contacts) 
            {
                // 스테이지에 배치된 장애물과의 충돌 지점의 y좌표는 0.55임을 이용, amon 위에서 떨어질 떄 충돌하면 amon 오브젝트 파괴
                if (contact.point.y > 1.3f) 
                {
                    Debug.Log(contact.point);

                    // (용현) 플레이어에게 달린 카메라 변수. 기존에 하드코딩으로 자식 번호 위치로 카메라 변수를 지정하니까 정상적으로 카메라를 못골랐었음
                    GameObject camera = GameManager.Instance.Cam;
                    // 상속된 카메라를 상속 해제하는 코드
                    camera.transform.parent = null; 

                    // (용현) 플레이어 비활성화
                    collision.gameObject.SetActive(false);

                    // (용현) 결과창 업데이트
                    /* Not Implemented */
                }
            }
        }
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
