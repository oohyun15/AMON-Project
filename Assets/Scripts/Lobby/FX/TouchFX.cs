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
    private GameObject[] FX_Touchs;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(eventData.position);

        Vector2 initPos = eventData.position;


        // Debug.Break();
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

            FX_Touchs[idx] = go;

            go.transform.localPosition = Vector3.zero;

            go.transform.localScale = Vector3.one;
        }
    }

}
