using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrink : Item
{
    public int addSpeed; // 속도 증가량 관리 변수
    public int dopingTime; // 지속시간 관리 변수

    public override void ItemActive()
    {
        player.StartCoroutine(player.UpSpeed(addSpeed, dopingTime));
        DurabilityManage();
    }
}
