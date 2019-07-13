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
                    GameObject camera = collision.transform.GetChild(1).gameObject; 
                    if(camera.transform.GetComponent<Camera>() != null) camera.transform.parent = null; // 상속된 카메라를 상속 해제하는 코드
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}
