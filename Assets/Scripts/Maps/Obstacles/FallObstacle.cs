﻿/****************************************
 * FallObstacle.cs
 * 제작: 김용현
 * 떨어지는 장애물 관련 함수
 * (19.08.07) 초기 위치값 저장(IReset 추가)
 * (19.10.04) 죽었을 때 유저데이터에 죽은 횟수 저장
 * (19.11.03) 초기화 방식을 바꿨습니다. (setActive -> gravity)
 * (19.11.17) 장애물 떨어지고 남아있도록 수정
 * (19.11.30) 장애물 떨어지는 방식 변경(Kinetic 사용)
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.08.05
 * 수정일자: 19.11.30
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallObstacle : MonoBehaviour, IReset
{
    private Vector3 initPos;
    private Quaternion initRot;

    public bool isFalling = true;

    void Start()
    {
        GetInitValue();
    }

    public void GetInitValue()
    {
        initPos = gameObject.transform.position;

        initRot = gameObject.transform.rotation;

        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void SetInitValue()
    {
        // 초기값 위치
        gameObject.transform.SetPositionAndRotation(initPos, initRot);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" && FallTrigger.isWarning)
        {
            // 충돌 위치를 알아내기 위한 코드, 유니티에 collision.contacts 검색
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.Log(contact);

                // (용현) 플레이어에게 달린 카메라 변수. 기존에 하드코딩으로 자식 번호 위치로 카메라 변수를 지정하니까 정상적으로 카메라를 못골랐었음
                GameObject camera = GameManager.Instance.Cam;
                // 상속된 카메라를 상속 해제하는 코드
                camera.transform.parent = null;

                // (용현) 플레이어 비활성화
                collision.gameObject.SetActive(false);

                UserDataIO.User user = UserDataIO.ReadUserData();

                user.deathCount++;

                UserDataIO.WriteUserData(user);

                // 게임 오버
                GameManager.Instance.GameOver();

                break;

                // (용현) 결과창 업데이트
                /* Not Implemented */

            }
        }
    }
}
