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
    public string sceneName;

    public void MoveScene()
    {
        int isPlayed = UserDataIO.ReadStageData().isPlayed[(stageName.text[5] - 49) * 3];

        if (isPlayed == 0)
        {
            // 입장 대화 출력
            StoryDialog.instance.SetFile("enter" + (stageName.text[5] - 48));

            StoryDialog.instance.sceneNameToMove = stageName.text;
        }
        else
        {
            Lobby.MoveScene(sceneName);
            Debug.Log(sceneName);
        }
    }
}
