﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Evidence : MonoBehaviour
{
    private string eviName;
    public string Name { get { return eviName; } }
    private string eviExplain;
    public string Explain { get { return eviExplain; } }
    private Sprite eviImg;
    public Sprite Img { get { return eviImg; } }
    public int id;

    private Image image;
    private Text txtName;
    public Button btn;
    public bool isGot = true;

    private void Awake()
    {
        image = gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        btn = gameObject.transform.GetChild(0).GetComponent<Button>();
        txtName = gameObject.transform.GetChild(1).GetComponent<Text>();
    }

    public void SetEvidence(Dictionary<string, object> evi, Sprite img)
    {
        eviName = evi["evidenceName"].ToString();
        eviExplain = evi["evidenceExplain"].ToString();
        eviImg = img;

        image.sprite = eviImg;

        if (eviImg == null)
            image.color = Color.grey ;

        image.SetNativeSize();

        float temp = 100 / Mathf.Max(image.rectTransform.rect.width, image.rectTransform.rect.height);

        image.rectTransform.sizeDelta = image.rectTransform.sizeDelta * temp;

        txtName.text = eviName;
    }

    public void SetDisabled()
    {
        //btn.interactable = false;
        txtName.text = "미획득 단서";
        isGot = false;
    }
}
