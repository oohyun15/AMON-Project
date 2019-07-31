/****************************************
 * Obstacle.cs
 * 제작: 김태윤
 * 장애물 관련 함수
 * (19.07.30) 플레이어 제거되지 않게 수정 및 카메라를 정상적으로 찾게 수정
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.09
 * 수정일자: 19.07.30
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int hp; // 장애물 체력
    
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rgbd = transform.GetComponent<Rigidbody>();
        if(rgbd != null) rgbd.mass = Mathf.Infinity; //mass를 최대로 높여 Amon과 장애물이 부딪혀도 장애물은 움직이지 않게 함
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            foreach (ContactPoint contact in collision.contacts) // 충돌 위치를 알아내기 위한 코드, 유니티에 collision.contacts 검색
            {
                if (contact.point.y > 1.3f) // 스테이지에 배치된 장애물과의 충돌 지점의 y좌표는 0.55임을 이용, amon 위에서 떨어질 떄 충돌하면 amon 오브젝트 파괴
                {
                    Debug.Log(contact.point);

                    // (용현) 플레이어에게 달린 카메라 변수. 기존에 하드코딩으로 자식 번호 위치로 카메라 변수를 지정하니까 정상적으로 카메라를 못골랐었음
                    GameObject camera = collision.gameObject.GetComponent<AmonController>()._camera.gameObject;

                    camera.transform.parent = null; // 상속된 카메라를 상속 해제하는 코드

                    // (용현) 플레이어 비활성화
                    collision.gameObject.SetActive(false);

                    // (용현) 결과창 업데이트
                    /* Not Implemented */
                }
            }
        }
    }
}
