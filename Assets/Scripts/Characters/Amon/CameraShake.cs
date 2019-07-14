using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Vector3 originPos; // Shaking 후 다시 제자리로 돌아가기위한 변수

    void Start()
    {
        originPos = transform.localPosition;
    }

    public IEnumerator Shake(float _amount, float _duration)
    {
        float timer = 0;
        while (timer <= _duration)
        {
            transform.localPosition = (Vector3)Random.insideUnitCircle * _amount + originPos;
            // 일정 반경을 무작위로 움직임으로써 카메라 흔들림 효과를 준다.
            timer += Time.deltaTime; // deltaTime을 이용해서 Shaking 지속 시간을 관리
            yield return null;
        }
        transform.localPosition = originPos; // position값 원래대로 되돌림

    }
}
