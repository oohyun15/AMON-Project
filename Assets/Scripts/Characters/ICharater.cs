/****************************************
 * ICharacter.cs
 * 제작: 김용현
 * 게임 내 공통 캐릭터 특징을 가지는 스크립트
 * 작성일자: 19.08.01
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface IReset
{
    // 초기값 저장
    void GetInitValue();

    // 초기값 설정
    void SetInitValue();

}
