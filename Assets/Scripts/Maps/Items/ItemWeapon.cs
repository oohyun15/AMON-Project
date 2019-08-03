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
