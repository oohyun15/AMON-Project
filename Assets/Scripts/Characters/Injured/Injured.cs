/****************************************
 * Injured.cs
 * 제작: 조예진
 * 부상자 캐릭터의 기본 클래스, 구조 시 기본 상호작용 코드
 * 작성일자: 19.07.11
 * 19. 07. 14 수정 - 구조 시 초록색으로 바뀌도록 함, 플레이어 속도 조절
 * 19. 07. 30 수정(용현) - 이동속도 관련 조이스틱 속도 수정할 수 있게 변경
 * 19. 08. 11 수정(용현) - Exit 트리거에 닿았을 때 LeftInjured 수 감소시킴
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Injured : MonoBehaviour
{
    public enum InjuryType { MINOR, SERIOUS }

    public bool isRescued;
    public InjuryType type;

    protected AmonController player;
    protected SpriteRenderer minimapDot;    // 미니맵 표시 점
    protected MeshRenderer meshRenderer;

    public float speed;                     // 플레이어 속도 변경값
    public Vector3 initPos;             // 초기 Position 값
    public Quaternion initRot;          // 초기 Rotation 값

    protected virtual void Start()
    {
        isRescued = false;
        minimapDot = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // 부상자 구조 시
    public virtual void Rescue(AmonController player)
    {
        isRescued = true;
        gameObject.tag = "Rescued";

        // 플레이어가 통과해 다닐 수 있도록 트리거 처리
        GetComponent<Collider>().isTrigger = true;

        CheckMaterialRescued();

        this.player = player;

        // 플레이어 속도 0 이하 되지 않도록 함
        if ((player.moveSpeed -= speed) <= 0) player.moveSpeed += speed;

        // (용현) 조이스틱에서의 플레이어 속도 업데이트
        JoystickController.instance.UpdateSpeed();
    }

    public void CheckMaterialRescued()
    {
        // 미니맵 표시점 색깔 변경
        minimapDot.color = Color.green;

        // material 색깔 변경
        meshRenderer.material.color = Color.green;
    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        // 출구 트리거 발생 시
        if (col.tag == "Exit")
        {
            // 부상자 수 감소
            GameManager.Instance.leftInjured--;

            // 부상자 구조 체크
            GameManager.Instance.CheckGameClear();

            // 플레이어 속도 정상화
            player.moveSpeed += speed;

            // (용현) 조이스틱에서의 플레이어 속도 업데이트
            JoystickController.instance.UpdateSpeed();

            // 부상 타입별 출구 도착 시 행동 구현
            EnteredExit();

            gameObject.SetActive(false);
        }
    }

    protected abstract void EnteredExit();


}
