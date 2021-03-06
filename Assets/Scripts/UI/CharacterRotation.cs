﻿/****************************************
 * CharacterRotation.cs
 * 제작: 김용현
 * Amon캐릭터의 회전을 조절하는 코드
 * (19.10.03) 위, 아래로 터치시 카메라 회전 구현중...
 * (19.11.03) 로비씬 락커룸 플레이어렌더 회전
 * (19.11.14) 조예진 회전 방식 바꿨음
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.10.02
 * 수정일자: 19.11.03
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterRotation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [HideInInspector]
    public bool isTouch = false;                // 터치를 눌렀는 지 확인하는 변수
    public int type;                            // 0: Lobby, 1: Ingame\
    public bool isLobby = false;
    // public float camMoveSpeed;
    
    public GameObject player;
    // private GameObject initCamPos;
    // private GameObject cam;
    private Vector3 playerRot;
    // private Vector3 camRot;
    // private Vector3 camPos;
    private Vector2 t_initPos;
    public float rotSpeed;
    private float width;
    // private float angle;
    // private float distance;

    private Vector3 touchStart;


    /* 조예진 회전 */
    Vector2 prevPosition, touchPosition;
    Quaternion fir_rotation;
    float _rotSpeed = 5;
    int finId = -1;

    bool isAndroid;

    public Transform joyPos;
    public Slider slider;
   


    // Start is called before the first frame update
    void Start()
    {
        width = GetComponent<RectTransform>().sizeDelta.x;

        if (Application.platform == RuntimePlatform.WindowsEditor) isAndroid = false;
        else if (Application.platform == RuntimePlatform.Android) isAndroid = true;

        if (type == 1)
        {
            player = GameManager.Instance.player.gameObject;

            // Vector3 playerCenter = player.transform.position + Vector3.up * 1.9f;

            // initCamPos = player.GetComponent<AmonController>().initCamPos;

            // cam = GameManager.Instance.Cam;

            // 김용현 회전 속도
            rotSpeed = player.GetComponent<AmonController>().rotSpeed;

            // 조예진 회전 속도
            _rotSpeed = UserDataIO.ReadUserData().rotSpeed;

            // angle = Mathf.Atan2(initCamPos.transform.localPosition.y, initCamPos.transform.localPosition.z);

            // distance = Vector3.Distance(playerCenter, initCamPos.transform.position);

            // Debug.Log(distance);

            // Debug.Log(angle * Mathf.Deg2Rad);

            //width = Screen.width;

            slider.value = _rotSpeed;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (type == 0) player.transform.Rotate(-playerRot);
         /*
        else if (GameManager.Instance.gameState == GameManager.GameState.Playing && isTouch)
        {
            player.transform.Rotate(playerRot);

            // cam.transform.Rotate(camRot);

            // initCamPos.transform.Translate(camPos);
        }*/
    }

    public void OnChangeRotSpeed()
    {
        _rotSpeed = slider.value;

        UserDataIO.User user = UserDataIO.ReadUserData();

        user.rotSpeed = _rotSpeed;

        UserDataIO.WriteUserData(user);
    }

    /* 김용현 회전 */
    /*
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 value = eventData.position - t_initPos;

        float distance = Vector2.Distance(eventData.position, t_initPos) / width;

        if (distance > 0.3f) distance = 0.3f;

        value = value.normalized;

        // Debug.Log("value: " + value);

        // Debug.Log("distance: " + distance);



        // 캐릭터의 회전 위치를 변경
        playerRot = new Vector3(
            0f,
            value.x * rotSpeed * distance * Time.deltaTime,
            0f);

        // UpdateCamPos(value.y);
    }
    */

    void CheckRotFingerId()
    {
        int max = 0;
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (i == 0)
                continue;
            else
            {
                if (Vector2.Distance(joyPos.position, Input.touches[i].position)
                    > Vector2.Distance(joyPos.position, Input.touches[max].position))
                    max = i;
            }
        }

        finId = max;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (!isTouch)
        {            
            t_initPos = eventData.position;

            /* 조예진 회전 */
            isTouch = true;

            fir_rotation = player.transform.rotation;

            if (isAndroid)
            {
                finId = Input.touches[Input.touchCount - 1].fingerId;

                CheckRotFingerId();
                prevPosition = Input.GetTouch(finId).position;
            }
            else
            {
                prevPosition = Input.mousePosition;
            }

            touchPosition = prevPosition;
        }
    }

    /* 조예진 회전 */

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (isTouch)
        {
            if (isAndroid && finId != -1)
            {
                if (Input.touchCount == 1) finId = 0;

                CheckRotFingerId();
                touchPosition = Input.GetTouch(finId).position;
            } else
            {
                touchPosition = Input.mousePosition;
            }
            player.transform.Rotate(new Vector3(0, (touchPosition.x - prevPosition.x) / _rotSpeed, 0)
                * (isLobby ? -1 : 1));

            prevPosition = touchPosition;
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (isTouch)
        {
            finId = -1;

            isTouch = false;

            playerRot = Vector3.zero;
        }
    }


    // (19.10.03) 구현중
    /*
    void UpdateCamPos(float value)
    {
        camRot = new Vector3(
        value * camMoveSpeed * Time.deltaTime,
        0f,
        0f);

        Debug.Log(value);

        angle += value * 0.1f;

        camPos = new Vector3(0f,
            distance * Mathf.Cos(angle) * camMoveSpeed * Time.deltaTime,
            distance * Mathf.Sin(angle) * camMoveSpeed * Time.deltaTime);

        // Debug.Log(angle);

        // Debug.Log(camPos);
    }
    */

}
