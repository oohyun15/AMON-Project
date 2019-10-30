/****************************************
 * CharacterRotation.cs
 * 제작: 김용현
 * Amon캐릭터의 회전을 조절하는 코드
 * (19.10.03) 위, 아래로 터치시 카메라 회전 구현중...
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.10.02
 * 수정일자: 19.10.03
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterRotation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [HideInInspector]
    public bool isTouch = false;                // 터치를 눌렀는 지 확인하는 변수
    // public float camMoveSpeed;
    
    private GameObject player;
    // private GameObject initCamPos;
    // private GameObject cam;
    private Vector3 playerRot;
    // private Vector3 camRot;
    // private Vector3 camPos;
    private Vector2 t_initPos;
    private float rotSpeed;
    private int width;
    // private float angle;
    // private float distance;

 

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.player.gameObject;

        Vector3 playerCenter = player.transform.position + Vector3.up * 1.9f;

        // initCamPos = player.GetComponent<AmonController>().initCamPos;

        // cam = GameManager.Instance.Cam;

        rotSpeed = player.GetComponent<AmonController>().rotSpeed;

        // angle = Mathf.Atan2(initCamPos.transform.localPosition.y, initCamPos.transform.localPosition.z);

        // distance = Vector3.Distance(playerCenter, initCamPos.transform.position);

        // Debug.Log(distance);

        // Debug.Log(angle * Mathf.Deg2Rad);

        width = Screen.width;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState == GameManager.GameState.Playing && isTouch)
        {
            player.transform.Rotate(playerRot);

            // cam.transform.Rotate(camRot);

            // initCamPos.transform.Translate(camPos);
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 value = eventData.position - t_initPos;

        float distance = Vector2.Distance(eventData.position, t_initPos) / width;

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

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        isTouch = true;

        t_initPos = eventData.position;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        isTouch = false;
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
