/****************************************
 * SeriousInjured.cs
 * 제작: 조예진
 * 중상 부상자 캐릭터의 상세 상호작용 코드
 * 작성일자: 19.07.11
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeriousInjured : Injured
{
    private BoxCollider col;

    protected override void Start()
    {
        col = GetComponent<BoxCollider>();

        base.Start();
        type = InjuryType.SERIOUS;
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

    protected override void EnteredExit()
    {
        player.isRescuing = false;
    }
}
