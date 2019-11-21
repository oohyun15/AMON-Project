/****************************************
 * TouchFX.cs
 * 제작: 김용현
 * 로비 내에서 터치시 생성되는 FX
 * (19.11.21) 터치 FX가 캔버스에 정확한 위치에 나오지 않던 버그 수정(localPosition -> position 교체)
 * 작성일자: 19.11.10
 * 수정일자: 19.11.21
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchFX : MonoBehaviour, IPointerDownHandler
{
    public int initNum;
    private LobbyFX fx;
    private GameObject FX_Touch, FX_Lists;
    private int count = 0;

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 initPos = eventData.position;

        Debug.Log(initPos);

        if (count >= initNum) count = 0;

        GameObject touch = FX_Lists.transform.GetChild(count).gameObject;

        touch.transform.position = initPos;

        touch.SetActive(true);

        touch.GetComponent<ParticleSystemAutoDestroy>().SetTimer();
        
        count++;
    }

    // Start is called before the first frame update
    void Start()
    {
        fx = LobbyFX.instance;

        FX_Touch = fx.FX_Touch;

        FX_Lists = fx.FX_Lists;

        for (int idx = 0; idx < initNum; idx++)
        {
            GameObject go = Instantiate(FX_Touch, FX_Lists.transform);

            go.SetActive(false);
        }
    }

}
