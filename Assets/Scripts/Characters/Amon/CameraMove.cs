/****************************************
 * CameraMove.cs
 * 제작: 김태윤
 * FireFigter와 카메라 사이에 벽이 있으면 카메라가 FireFigter쪽으로 움직이도록 하는 함수
 * Raycast를 이용해 충돌포인트로 카메라를 이동시켜 벽에 의해 시야가 안가려지도록 함
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.08.07
 ***************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject initPos; 
    // 부상자의 BackPoint처럼 카메라의 원래 위치가 변하지않도록 하는 빈 게임오브젝트를 받아옴, position값이 카메라의 처음 위치와 동일
    public float spd; //Lerp함수 속도 변수

    private GameObject player; 
    void Start()
    {
        player = GameManager.Instance.player.gameObject;
    }

    void Update()
    {
        Ray ray = new Ray(player.transform.position + Vector3.up * 1.9f, initPos.transform.position - (player.transform.position + Vector3.up * 1.9f)); 
        // ray를 player에서 initPos오브젝트 방향으로 발사하여 player와 가장 가까이 있는 벽을 hit으로 받도록 하였음
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2)) // Ray를 발사하여 충돌 검사하는 부분
        {
            if (hit.collider.tag == "Wall") // 충돌체가 벽일때만 적용
                 transform.position = Vector3.Lerp(transform.position, hit.point, spd * Time.deltaTime); 
            // hit.point는 ray와 hit의 충돌지점을 말함, 따라서 ray에 벽이 충돌하면 카메라를 충돌지점으로 이동시킨다는 의미
            else return;
        }
        else
        {
            if(transform.position != initPos.transform.position)
            transform.position = Vector3.Lerp(transform.position, initPos.transform.position, spd * Time.deltaTime);
            //만약 충돌이 일어나지않는다면 카메라를 원래 위치로 되돌림
        }

        Debug.DrawRay(ray.origin, ray.direction * 2, Color.red);
    }
    
}
