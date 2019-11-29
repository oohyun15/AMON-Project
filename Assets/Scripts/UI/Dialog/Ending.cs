using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Ending : StoryDialog
{
    int stress = -1;
    int end = -1;

    public Sprite[] endings;
    public Image back;

    protected void Start()
    {
        int str = UserDataIO.ReadUserData().stress;

        ItemDataManager idm = ItemDataManager.Instance;

        for (int i = 0; i < idm.stressValues.Length; i++)
        {
            if (idm.stressValues[i] > str)
            {
                stress = i - 1;
                break;
            }
        }

        if (stress == -1)
            stress = idm.stressValues.Length - 1;

        SetFile("ending");

        back.sprite = endings[stress];
    }

    public override void InitDialog()
    {
        path = rootPath + fileName;

        dialogData = CSVReader.Read(path);

        index = -1;

        for (int i = 0; i < dialogData.Count; i++)
        {
            Debug.Log(dialogData[i]["talker"].ToString() + " " + stress.ToString());
            if (dialogData[i]["talker"].ToString() == stress.ToString())
            {
                if (index == -1)
                    index = i;
            }
            else if (index != -1)
            {
                end = i;
                break;
            }
        }

        panel.SetActive(true);

        isTalking = false;

        UpdateDialog();
    }

    public override void Touched()
    {
        if (index == end)
            EndDialog();
        else
            base.Touched();
    }

    public override void EndDialog()
    {
        base.EndDialog();
    }
}
