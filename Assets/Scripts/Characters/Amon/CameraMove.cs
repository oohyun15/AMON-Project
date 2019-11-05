/****************************************
 * CameraMove.cs
 * 제작: 김태윤
 * FireFigter와 카메라 사이에 벽이 있으면 카메라가 FireFigter쪽으로 움직이도록 하는 함수
 * Raycast를 이용해 충돌포인트로 카메라를 이동시켜 벽에 의해 시야가 안가려지도록 함
 * (08.11) RayCastAll으로 수정하여 Ray가 벽이 아닌 다른 오브젝트와 충돌하여 생기는 버그를 수정함
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.08.07
 * 수정일자: 19.08.11
 ***************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject initPos; 
    // 부상자의 BackPoint처럼 카메라의 원래 위치가 변하지않도록 하는 빈 게임오브젝트를 받아옴, position값이 카메라의 처음 위치와 동일
    public float spd; //Lerp함수 속도 변수
    public float rayLength; // Ray의 길이를 정하는 변수

    private GameObject player;
    private Vector3 subCamVec;

    void Start()
    {
        player = GameManager.Instance.player.gameObject;
    }

    void Update()
    {
        Ray ray = new Ray(player.transform.position + Vector3.up * 1.9f, initPos.transform.position - (player.transform.position + Vector3.up * 1.9f));
        // ray를 player에서 initPos오브젝트 방향으로 발사하여 player와 가장 가까이 있는 벽을 hit으로 받도록 하였음
        RaycastHit[] hits = Physics.RaycastAll(ray, rayLength); // Ray와 충돌하는 모든 오브젝트 정보를 받아옴
        List<RaycastHit> wallList = new List<RaycastHit>();
        for (int i = 0; i < hits.Length; i++) // tag가 Wall인 오브젝트를 wallList에 집어넣는 For문
        {
            RaycastHit hit = hits[i];
            if (hit.collider.tag == "Wall" || hit.collider.tag == "Obstacle")
            {
                if (wallList.Count == 0) // wallList가 비어있으면 바로 집어넣음
                    wallList.Add(hit);
                else // wallList가 비어있지않으면 두 벽 오브젝트의 player와의 거리를 비교하여 더 가까운 오브젝트를 wallList에 집어넣고 다른 건 삭제
                {
                    if (Vector3.Distance(hit.collider.gameObject.transform.position, player.transform.position) <
                          Vector3.Distance(wallList[0].collider.gameObject.transform.position, player.transform.position))
                    {
                        wallList.RemoveAt(0);
                        wallList.Add(hit);
                    }
                    else break;
                }
            }
            else break;
        }

        subCamVec = ((player.transform.position + Vector3.up * 1.9f) - initPos.transform.position).normalized;

        if (wallList.Count != 0) // Ray를 발사하여 충돌 검사하는 부분(0이면 충돌하지않아 wallList가 비어있는 것, 1이면 가장 가까운 벽 오브젝트에 대해 카메라 위치 상호작용) 
             transform.position = Vector3.Lerp(transform.position, wallList[0].point + subCamVec, spd * Time.deltaTime); 
        else
        {
            if(transform.position != initPos.transform.position)
            transform.position = Vector3.Lerp(transform.position, initPos.transform.position, spd * Time.deltaTime);
            //만약 충돌이 일어나지않는다면 카메라를 원래 위치로 되돌림
        }
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);
    }
}
