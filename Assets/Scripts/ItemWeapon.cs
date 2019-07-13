using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeapon : Item
{
    public int addDamage; //추가 데미지 변수

    public override void ItemActive()
    {
        DurabilityManage();
    }
}
