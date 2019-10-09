using System.Collections;
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

    private void Start()
    {
        image = gameObject.transform.GetChild(0).GetComponent<Image>();
        btn = gameObject.transform.GetChild(0).GetComponent<Button>();
        txtName = gameObject.transform.GetChild(1).GetComponent<Text>();
    }

    public void SetEvidence(Dictionary<string, object> evi, Sprite img)
    {
        eviName = evi["evidenceName"].ToString();
        eviExplain = evi["evidenceExplain"].ToString();
        eviImg = img;

        image.sprite = eviImg;
        txtName.text = eviName;
    }

    private void SetDisabled()
    {
        btn.interactable = false;
        txtName.text = "미획득 단서";
    }
}
