﻿/****************************************
* Item.cs
* 제작: 김태윤
* 공격력 증가용 아이템
* (19.08.03) (용현) 플레이어 상태가 장애물일 때만 내구도 닳게 변경
* (19.08.16) Interaction시 해당 player 애니메이션을 실행하도록 코드 수정
* (19.08.20) 도끼는 내구도가 달지 않음
* (19.09.13) 애니메이션 조건 추가
* 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
* 작성일자: 19.07.26
* 수정일자: 19.09.13
***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeapon : Item
{
    public int addDamage; //추가 데미지 변수
    
    public override void ItemActive()
    {
        Debug.Log(GameManager.Instance.player.isCollisionObs); // 주변에 장애물 없으면 애니메이션도 적용하지 않음
        if (GameManager.Instance.player.isCollisionObs)
        {
            GameManager.Instance.player.animState = AmonController.AnimationName.Strike; // 애니메이션 상태 설정
            GameManager.Instance.player.PlayerAnimation(); // 애니메이션 실행
        }
        else GameManager.Instance.player.TouchBack();
        // (19.08.20) 도끼는 내구도 안닮
        /*
        // (19.08.03) (용현) 플레이어 상태가 장애물일 때만 내구도 닳게 변경
        if (GameManager.Instance.player.state == AmonController.InteractionState.Obstacle)
            DurabilityManage();

        */
    }
}
