/****************************************
 * Injured.cs
 * 제작: 조예진
 * 부상자 캐릭터의 기본 클래스, 구조 시 기본 상호작용 코드
 * 작성일자: 19.07.11
 * 19. 07. 14 수정(예진) - 구조 시 초록색으로 바뀌도록 함, 플레이어 속도 조절
 * 19. 07. 30 수정(용현) - 이동속도 관련 조이스틱 속도 수정할 수 있게 변경
 * 19. 08. 11 수정(용현) - Exit 트리거에 닿았을 때 LeftInjured 수 감소시킴
 * --> (예진) 계산 방식 바뀌어 다시 삭제
 * 19. 08. 23 수정(예진) - 부상자 제한 시간 계산하도록 변경
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Injured : MonoBehaviour
{
    public enum InjuryType { MINOR, SERIOUS }
    public enum InjuredState { LIVE, DEAD }

    public bool isRescued;
    public InjuryType type;
    public InjuredState state;

    public GameObject lyingBody;
    public GameObject huggedBody;

    protected AmonController player;
    protected SpriteRenderer minimapDot;    // 미니맵 표시 점
    protected MeshRenderer meshRenderer;

    public float playerSpeedChange;     // 플레이어 속도 변화량
    public float timeLimit;
    protected float time;
    public Vector3 initPos;             // 초기 Position 값
    public Quaternion initRot;          // 초기 Rotation 값

    protected IEnumerator timeChecker;

    private void Awake()
    {
        timeChecker = TimeChecker();
    }

    protected virtual void Start()
    {
        isRescued = false;
        minimapDot = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // 부상자 구조 시
    public virtual void Rescue(AmonController player)
    {
        StopCoroutine(timeChecker);

        isRescued = true;
        gameObject.tag = "Rescued";

        // (19.08.12. 예진) 플레이어가 구조된 부상자 리스트 가지고 있도록 함
        player.rescuers.Add(gameObject);

        // 플레이어가 통과해 다닐 수 있도록 트리거 처리
        GetComponent<Collider>().isTrigger = true;

        ChangeMaterialRescued();

        this.player = player;

        // 플레이어 속도 0 이하 되지 않도록 함
        if ((player.moveSpeed -= playerSpeedChange) <= 0) player.moveSpeed += playerSpeedChange;

        // (용현) 조이스틱에서의 플레이어 속도 업데이트
        JoystickController.instance.UpdateSpeed();
    }

    public virtual void ChangeMaterialRescued()
    {
        
        // 미니맵 표시점 색깔 변경
        minimapDot.color = Color.green;

        // material 색깔 변경
        meshRenderer.material.color = Color.green;
        
    }

    public void ChangeMaterialDead()
    {
        // 미니맵 표시점 색깔 변경
        minimapDot.color = Color.black;

        // material 색깔 변경
        meshRenderer.material.color = Color.black;
    }

    // (19.08.12 예진) 플레이어의 출구 트리거 발생 시 함수 호출하는 방식으로 변경
    public virtual void Escaped()
    {
        // 플레이어 속도 정상화
        player.moveSpeed += playerSpeedChange;

        // (용현) 조이스틱에서의 플레이어 속도 업데이트
        JoystickController.instance.UpdateSpeed();

        // 부상 타입별 출구 도착 시 행동 구현
        EnteredExitTrigger();

        gameObject.SetActive(false);
    }

    protected abstract void EnteredExitTrigger();

    // (19.08.23. 예진) 부상자 제한시간 체크 추가
    public void StartTimeCheck()
    {
        StartCoroutine(timeChecker);
    }

    public void StopTimeCheck()
    {
        StopCoroutine(timeChecker);
    }
            

    private IEnumerator TimeChecker()
    {
        while (time > 0)
        {
            time -= Time.deltaTime;

            yield return null;
        }

        TimeEnd();
    }

    protected void TimeEnd()
    {
        state = InjuredState.DEAD;

        ChangeMaterialDead();
    }
}
