/****************************************
 * StageLevel.cs
 * 제작: 김용현
 * 스테이지 선택 패널에 사용될 오브젝트 클래스
 * 작성일자: 19.10.12.
 * 수정일자: 19.10.12.
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageLevel : MonoBehaviour
{
    public Image[] rescueImage;
    public Text stageName;

    public void MoveScene()
    {
        Lobby.MoveScene(stageName.text);
    }
}
