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

    public float speed;

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

        // 미니맵 표시점 색깔 변경
        minimapDot.color = Color.green;

        // material 색깔 변경
        meshRenderer.material.color = Color.green;

        this.player = player;
    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        // 출구 트리거 발생 시
        if (col.tag == "Exit")
        {
            // 점수 추가, 부상자 구조 체크 - GameManager에서 설정

            // 플레이어 속도 정상화


            // 부상 타입별 출구 도착 시 행동 구현
            EnteredExit();

            gameObject.SetActive(false);
        }
    }

    protected abstract void EnteredExit();
}
