/****************************************
 * StoryDialog.cs
 * 제작: 조예진
 * 스프라이트 표시 대화창 기능
 * 작성일자: 19.11.08.
 ***************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class StoryDialog : Dialog
{ 
    public string sceneNameToMove;              // 대화 끝난 후 씬 이동 불필요한 경우 비워둘 것
    public Image image;
    public Sprite[] sprites;
    public string fileName;
    public GameObject panel;
    public GameObject loading;

    public static StoryDialog instance;

    private void Awake()
    {
        if (instance == null || instance.gameObject == null)
        {
            instance = this;
        }
    }

    protected override void Start()
    {

    }

    protected override void UpdateDialog()
    {
        image.sprite = sprites[System.Convert.ToInt32(dialogData[index]["sprite"].ToString())];

        base.UpdateDialog();
    }

    public void SetFile(string name)
    {
        if (fileName != null && panel.activeInHierarchy)
        {
            dialogData.AddRange(CSVReader.Read(rootPath + name));
        }
        else
        {
            fileName = name;
            
            InitDialog();
        }
    }

    public override void InitDialog()
    {
        path = rootPath + fileName;

        if (sprites == null || sprites.Length == 0)
            sprites = ItemDataManager.Instance.dialogSprites;

        panel.SetActive(true);

        base.InitDialog();
    }

    public override void EndDialog()
    {
        panel.SetActive(false);

        if (sceneNameToMove != "")
        {
            panel.SetActive(false);

            Lobby.MoveScene(sceneNameToMove);

            loading.SetActive(true);
        }
    }
}
