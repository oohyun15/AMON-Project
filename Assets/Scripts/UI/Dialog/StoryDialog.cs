﻿/****************************************
 * StoryDialog.cs
 * 제작: 조예진
 * 스프라이트 표시 대화창 기능
 * 작성일자: 19.11.08.
 ***************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryDialog : Dialog
{ 
    public string sceneNameToMove;              // 대화 끝난 후 씬 이동 불필요한 경우 비워둘 것
    public Image image;
    public Sprite[] sprites;
    public string fileName;

    protected override void UpdateDialog()
    {
        image.sprite = sprites[System.Convert.ToInt32(dialogData[index]["sprite"].ToString())];

        base.UpdateDialog();
    }

    public void SetFile(string name)
    {
        fileName = name;

        InitDialog();
    }

    public override void InitDialog()
    {
        path = rootPath + fileName;

        base.InitDialog();
    }

    protected override void EndDialog()
    {
        if (sceneNameToMove != "")
            Lobby.MoveScene(sceneNameToMove);
    }
}