﻿/****************************************
 * EvidenceController.cs
 * 제작: 조예진
 * 단서 창 관리 스크립트
 * 작성일자: 19.10.12.
 * 수정일자: 
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvidenceController : MonoBehaviour
{
    ItemDataManager idm;
    [Header("Evidence")]
    public Image eviImage;
    public Text eviName;
    public Text eviInfo;
    public Transform eviContents;
    public Sprite[] eviSprites;

    private void Start()
    {
        idm = ItemDataManager.Instance;
        
        InitEvidenceScroll();
        InitEviInfo();
    }

    public void InitEvidenceScroll()
    {
        List<Dictionary<string, object>> eviList = idm.GetEvidenceData();
        Evidence[] eviArr = eviContents.GetComponentsInChildren<Evidence>();

        for (int i = 0; i < eviArr.Length; i++)
        {
            Dictionary<string, object> eviData = eviList[i];
            Evidence evi = eviArr[i];
            evi.SetEvidence(eviData, eviSprites[i]);
            evi.btn.onClick.AddListener(() => OnClickEvidence(evi.Name, evi.Explain, evi.Img));
        }

        gameObject.SetActive(false);
    }

    public void InitEviInfo()
    {
        eviImage.sprite = null;
        eviName.text = "";
        eviInfo.text = "단서를 선택해 주세요";
    }

    public void OnClickEvidence(string name, string explain, Sprite sprite)
    {
        eviImage.sprite = sprite;
        eviName.text = name;
        eviInfo.text = explain;
    }
}