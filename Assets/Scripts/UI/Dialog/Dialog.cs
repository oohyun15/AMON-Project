/****************************************
 * Dialog.cs
 * 제작: 조예진
 * 대사 csv 파일 불러오고 타이핑 효과와 터치 시 스킵 효과 적용
 * 작성일자: 19.09.16.
 ***************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Dialog : MonoBehaviour
{
    public Text talker;
    public Text dialog;

    public float talkingSpeed;
    protected int index;
    protected bool isTalking;
    private string nowDialog;

    protected List<Dictionary<string, object>> dialogData;
    protected readonly string rootPath = "Data/Dialog/";
    protected string path;

    public virtual void InitDialog()
    {
        index = 0;

        isTalking = false;

        dialogData = CSVReader.Read(path);

        UpdateDialog();
    }

    // 화면 터치했을 경우 ㅡ 타이핑 효과 스킵 혹은 대화 넘기기
    public virtual void Touched()
    {
        if (isTalking && talker.transform.parent.gameObject.activeInHierarchy)
        {
            isTalking = false;

            dialog.text = nowDialog;
        }
        else if (index < dialogData.Count)
            UpdateDialog();
        else
            EndDialog();
    }

    // 새 대사 보여줌
    protected virtual void UpdateDialog()
    {
        talker.text = dialogData[index]["talker"].ToString();

        dialog.text = "";

        isTalking = true;

        nowDialog = dialogData[index]["dialog"].ToString();

        nowDialog = nowDialog.Replace('`', ',');

        StartCoroutine(TypeText(nowDialog));
    }

    // 타이핑 효과
    private IEnumerator TypeText(string text)
    {
        while (text.Length != 0 && isTalking)
        {
            dialog.text += text[0];

            yield return new WaitForSeconds(talkingSpeed);

            text = text.Substring(1);
        }

        isTalking = false;

        index++;
    }

    public abstract void EndDialog();
}
