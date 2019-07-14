﻿/****************************************
 * MinorInjured.cs
 * 제작: 조예진
 * 경상 부상자 캐릭터의 상세 상호작용 코드
 * 작성일자: 19.07.11
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinorInjured : Injured
{
    private FollowPlayer follow;

    protected override void Start()
    {
        follow = GetComponent<FollowPlayer>();
        base.Start();
        type = InjuryType.MINOR;
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
}
