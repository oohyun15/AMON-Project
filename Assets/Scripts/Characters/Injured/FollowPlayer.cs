/****************************************
 * FollowPlayer.cs
 * 제작: 조예진
 * 설정된 target을 따라다니도록 하는 코드
 * MinorInjured 오브젝트가 구조되었을 때 플레이어를 따라다니도록 함
 * 작성일자: 19.07.11
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform target;

    public float distance;      // 캐릭터와의 최소 거리 차이
    public float speed;         // 부상자 최대 속도

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Update()
    {
        if (Vector3.SqrMagnitude(target.position - transform.position) > distance)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
            transform.rotation = target.rotation;
        }
    }
}