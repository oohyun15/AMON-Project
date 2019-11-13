/****************************************
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
    private Sprite[] eviSprites;
    public Sprite emptySprite;

    private void Start()
    {
        idm = ItemDataManager.Instance;

        eviSprites = idm.eviSprites;

        InitEvidenceScroll();
        InitEviInfo();
    }

    public void InitEvidenceScroll()
    {
        List<Dictionary<string, object>> eviList = idm.GetEvidenceData();
        Evidence[] eviArr = eviContents.GetComponentsInChildren<Evidence>();
        UserDataIO.Stage stage = UserDataIO.ReadStageData();

        Dictionary<string, object> noData = new Dictionary<string, object>
        {
            ["evidenceName"] = "미획득 단서",
            ["evidenceExplain"] = "아직 획득하지 못한 단서입니다"
        };

        for (int i = 0; i < Mathf.Min(eviArr.Length, eviList.Count); i++)
        {
            Dictionary<string, object> eviData = eviList[i];
            Evidence evi = eviArr[i];

            if (stage.isGotEvidence[i] == 0)
                evi.SetEvidence(noData, emptySprite);
            else
                evi.SetEvidence(eviData, eviSprites[i]);

            evi.btn.onClick.AddListener(() => OnClickEvidence(evi.Name, evi.Explain, evi.Img));
        }
    }

    public void InitEviInfo()
    {
        eviImage.sprite = null;
        eviImage.gameObject.SetActive(false);
        eviName.text = "";
        eviInfo.text = "단서를 선택해 주세요";
    }

    public void OnClickEvidence(string name, string explain, Sprite sprite)
    {
        eviImage.sprite = sprite;
        eviImage.SetNativeSize();

        float temp = 350 / Mathf.Max(eviImage.rectTransform.rect.width, eviImage.rectTransform.rect.height);

        eviImage.rectTransform.sizeDelta = eviImage.rectTransform.sizeDelta * temp;

        eviImage.gameObject.SetActive(true);

        eviName.text = name;
        eviInfo.text = explain;
    }
}
