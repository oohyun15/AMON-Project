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
    private float _time;

    // Start is called before the first frame update
    void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        _time = ps.main.duration;
    }

    public void SetTimer()
    {
        StartCoroutine(Timer(_time));
    }

    private IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }

    /*
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
    */
}
