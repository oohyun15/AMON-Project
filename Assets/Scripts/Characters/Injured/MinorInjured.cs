/****************************************
 * MinorInjured.cs
 * 제작: 조예진
 * 경상 부상자 캐릭터의 상세 상호작용 코드
 * (19.08.01) 초기화 코드 추가
 * (19.08.03) 게임 시작시 GameManager 클래스의 Field Object에 들어가도록 수정
 * 작성일자: 19.07.11
 * 수정일자: 19.08.03
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinorInjured : Injured, IReset
{
    private FollowPlayer follow;
    private GameManager gm;            // GameManager 변수
    private new readonly string name = "Minor";

    protected override void Start()
    {
        follow = GetComponent<FollowPlayer>();

        gm = GameManager.Instance;

        base.Start();

        type = InjuryType.MINOR;

        var go = new List<GameObject>();

        // Object에 키가 있으면 추가
        if (gm.objects.ContainsKey(name))
            gm.objects[name].Add(gameObject);

        // 키가 없을 경우 생성
        else
        {
            gm.objects.Add(name, go);

            gm.objects[name].Add(gameObject);
        }

        // (용현) 초기값 저장
        GetInitValue();
    }

    public override void Rescue(AmonController player)
    {
        base.Rescue(player);

        // 따라다니게 하기
        transform.SetParent(player.rescuers);   // 구조된 오브젝트 확인 위해 parent 변경
        follow.SetTarget(player.transform);     // 부상자가 따라다닐 타겟 설정
        follow.enabled = true;                  // 플레이어를 따라다니도록 함
    }

    protected override void EnteredExit()
    {

    }

    public void GetInitValue()
    {
        initPos = gameObject.transform.position;

        initRot = gameObject.transform.rotation;
    }

    public void SetInitValue()
    {
        gameObject.SetActive(true);

        FollowPlayer follow = GetComponent<FollowPlayer>();

        // 따라다니는 거 비활성화
        follow.enabled = false;

        // 부상자 목록으로 이동, 부상자랑 구조자 위치가 각각 GameManager, AmonController로 달라서 수정좀 해야할 듯
        transform.SetParent(GameManager.Instance.injuredParent.transform);

        // 초기값 위치
        gameObject.transform.SetPositionAndRotation(initPos, initRot);

        // 플레이어가 통과해 다닐 수 있도록 트리거 처리
        GetComponent<Collider>().isTrigger = false;

        // 미니맵 표시점 색깔 변경
        minimapDot.color = Color.red;

        // material 색깔 변경
        meshRenderer.material.color = Color.red;

        isRescued = false;

        gameObject.tag = "Injured";
    }
}
