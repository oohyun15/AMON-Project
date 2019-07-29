/****************************************
 * JoystickController.cs
 * 제작: 김용현
 * Amon캐릭터의 움직임을 조절하는 조이스틱 로직 코드
 * (19.07.14) 현재 Canvas내 Background 중심이 센터로 잡혀있음. 추후에 화면 좌측 하단으로 바꿔서 코드 수정해야함!
 * (19.07.30) 조이패드의 x좌표를 이용해 플레이어의 rotation 값을 설정해줌
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.14
 * 수정일자: 19.07.30
 ***************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;         // 터치 이벤트 구현을 위해 추가

public class JoystickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // 조이스틱 UI 관련 변수            19.07.14 추가
    public RectTransform Background;    // 좌측 하단 백그라운드
    public RectTransform Joystick;      // 촤즉 하단 조이스틱 버튼
    private float radius;               // 백그라운드 내에서 조이스틱이 이동가능한 범위의 반지름

    public GameObject Player;           // 이동시킬 플레이어 오브젝트
    private float moveSpeed;            // 플레이어의 이동속도. AmonController에서 가져올 예정
    private float rotSpeed;             // 플레이어의 회전속도. AmonController에서 가져올 예정
    private bool isTouch = false;       // 터치를 눌렀는 지 확인하는 변수
    private Vector3 movePosition;
    private Vector3 rotPosition;

    void Start()
    {
        // AmonController에서 캐릭터의 이동속도 및 회전속도를 가져옴
        moveSpeed = Player.GetComponent<AmonController>().moveSpeed;

        rotSpeed = Player.GetComponent<AmonController>().rotSpeed;

        // 백그라운드 이미지의 가로 길이의 절반을 반지름으로 사용
        radius = Background.rect.width * 0.5f;
    }

    void Update()
    {
        // 터치 중에는 movePosition 값으로 캐릭터가 이동
        if (isTouch)
        {
            Player.transform.Translate(movePosition);

            Player.transform.Rotate(rotPosition);
        }
    }

    // 터치 시 실행되는 함수
    public void OnPointerDown(PointerEventData eventData)
    {
        isTouch = true;
    }

    // 터치 화면에서 땔 시 실행되는 함수
    public void OnPointerUp(PointerEventData eventData)
    {
        isTouch = false;

        // 조이스틱 위치를 초기 값으로 바꿔줌
        Joystick.localPosition = Vector3.zero;

        // 이전에 플레이어가 이동했던 방향 값을 제거해줌
        movePosition = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 터치 포인트와 백그라운드 사이의 거리
        Vector2 value = eventData.position - (Vector2)Background.position;

        // 백그라운드 이미지 밖으로 나오지 않게 설정
        value = Vector2.ClampMagnitude(value, radius);

        // 위 value를 현재 조이스틱의 위치로 설정
        Joystick.localPosition = value;

        // value normalized
        value = value.normalized;
        
        // 조이스틱의 위치에 따른 백그라운드 거리. 캐릭터 속도차이를 주기위해 선언
        float distance = Vector2.Distance(Background.position, Joystick.position) / radius;

        // 캐릭터의 포지션 위치를 변경
        movePosition = new Vector3(
            value.x * moveSpeed * distance * Time.deltaTime, 
            0f, 
            value.y * moveSpeed * distance * Time.deltaTime);

        // 캐릭터의 회전 위치를 변경
        rotPosition = new Vector3(
            0f,
            value.x * rotSpeed * Time.deltaTime,
            0f);
    }
}
