/****************************************
 * TouchFX.cs
 * 제작: 김용현
 * 로비 내에서 터치시 생성되는 FX
 * 작성일자: 19.11.10
 * 수정일자: 19.11.10
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

        if (count >= initNum) count = 0;

        GameObject touch = FX_Lists.transform.GetChild(count).gameObject;

        touch.transform.localPosition = initPos;

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


            go.transform.localPosition = Vector3.zero;

            go.transform.localScale = Vector3.one;

            go.SetActive(false);
        }
    }

}
