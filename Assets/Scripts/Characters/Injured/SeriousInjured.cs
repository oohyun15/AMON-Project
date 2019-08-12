/****************************************
* SeriousInjured.cs
* 제작: 조예진
* 중상 부상자 캐릭터의 상세 상호작용 코드
* (19.08.01) 초기화 함수 추가
* 작성일자: 19.07.11
* 수정일자: 19.08.01
***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeriousInjured : Injured, IReset
{
    private BoxCollider col;
    private GameManager gm;
    private new readonly string name = "Serious";

    protected override void Start()
    {
        gm = GameManager.Instance;

        col = GetComponent<BoxCollider>();

        base.Start();
        type = InjuryType.SERIOUS;

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

        player.isRescuing = true;

        // 캐릭터 뒤에 업히기
        col.isTrigger = true;
        transform.SetParent(player.backPoint);
        transform.localPosition = Vector3.zero;
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    protected override void EnteredExitTrigger()
    {
        player.isRescuing = false;
    }

    public void GetInitValue()
    {
        initPos = gameObject.transform.position;

        initRot = gameObject.transform.rotation;
    }

    public void SetInitValue()
    {
        gameObject.SetActive(true);

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

        // 비구조상태로 변경
        isRescued = false;

        // 태그 변경
        gameObject.tag = "Injured";
    }
}
