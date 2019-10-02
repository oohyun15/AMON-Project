/****************************************
 * CharacterRotation.cs
 * 제작: 김용현
 * Amon캐릭터의 회전을 조절하는 코드
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.10.02
 * 수정일자: 19.10.02
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterRotation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public bool isTouch = false;                // 터치를 눌렀는 지 확인하는 변수

    private GameObject player;
    private Vector3 rotPosition;
    private Vector2 t_initPos;
    private float rotSpeed;

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 value = eventData.position - t_initPos;

        value = value.normalized;

        // 캐릭터의 회전 위치를 변경
        rotPosition = new Vector3(
            0f,
            value.x * rotSpeed * Time.deltaTime,
            0f);

        Debug.Log(rotPosition);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        isTouch = true;

        t_initPos = eventData.position;

        Debug.Log(t_initPos);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        isTouch = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.player.gameObject;

        rotSpeed = player.GetComponent<AmonController>().rotSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState == GameManager.GameState.Playing && isTouch)
            player.transform.Rotate(rotPosition);
    }
}
