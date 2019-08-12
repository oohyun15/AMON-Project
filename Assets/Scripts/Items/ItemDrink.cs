﻿/****************************************
* Item.cs
* 제작: 김태윤
* 이동속도 증가용 아이템
* (19.08.03) player 수정 및 GameManager로 수정
* 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
* 작성일자: 19.07.26
* 수정일자: 19.08.03
***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrink : Item
{
    public int addSpeed; // 속도 증가량 관리 변수
    public int dopingTime; // 지속시간 관리 변수

    public override void ItemActive()
    {
        GameManager.Instance.player.StartCoroutine(GameManager.Instance.player.UpSpeed(addSpeed, dopingTime));

        DurabilityManage();
    }
}