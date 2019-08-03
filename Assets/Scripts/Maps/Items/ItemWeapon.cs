/****************************************
* Item.cs
* 제작: 김태윤
* 공격력 증가용 아이템
* (19.08.03) (용현) 플레이어 상태가 장애물일 때만 내구도 닳게 변경
* 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
* 작성일자: 19.07.26
* 수정일자: 19.08.03
***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeapon : Item
{
    public int addDamage; //추가 데미지 변수

    public override void ItemActive()
    {
        // (19.08.03) (용현) 플레이어 상태가 장애물일 때만 내구도 닳게 변경
        if (GameManager.Instance.player.state == AmonController.InteractionState.Obstacle)
            DurabilityManage();
    }
}
