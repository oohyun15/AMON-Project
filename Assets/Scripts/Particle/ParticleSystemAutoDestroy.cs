/****************************************
 * ParticleSystemAutoDestroy.cs
 * 제작: 백수영
 * 파티클 수명이 다됐을 시 씬에서 삭제
 * 작성일자: 19.10.19.
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemAutoDestroy : MonoBehaviour
{
    private ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps)
        {
            if (!ps.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}
