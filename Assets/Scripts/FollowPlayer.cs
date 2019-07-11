using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform target;

    public float distance;      // 캐릭터와의 거리 차이
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
