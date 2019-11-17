using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultAnimationController : MonoBehaviour
{
    public GameObject hor1;
    public GameObject hor2;

    public Text moneyText;
    public Text honorText;
    public Text stressText;
    public Image stressSlider;
    public Transform evidence;
    public Transform evidencePanel;
    public GameObject normalBtn;
    public GameObject eventBtn;

    public Sprite saveSprite;

    readonly float fillSpeed = 1.5f;
    WaitForSeconds checkTime;

    int total, save, left, money, honor, stress;

    public void StartResultAnimation(int total, int left, int money, int honor, int stress)
    {
        checkTime = new WaitForSeconds(Time.deltaTime * fillSpeed * 15);

        moneyText.text = "";
        honorText.text = "";
        stressText.text = "";

        this.total = total;
        this.left = left;
        save = total - left;
        this.money = money;
        this.honor = honor;
        this.stress = stress;

        // 버튼 설정
        if (stress >= 100  || DataManager.Instance.IsLastStage())
        {
            normalBtn.SetActive(false);
            eventBtn.SetActive(true);
        }
        else
        {
            normalBtn.SetActive(true);
            eventBtn.SetActive(false);
        }

        gameObject.SetActive(true);
        // 애니메이션 시작
        ActiveInjured();
    }

    private void ActiveInjured()
    {
        switch (total)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                for (int i = 0; i < total; i++)
                {
                    hor2.transform.GetChild(i).gameObject.SetActive(true);
                }
                hor1.SetActive(false);
                break;

            case 5:
                for (int i = 0; i < 2; i++)
                {
                    hor1.transform.GetChild(i).gameObject.SetActive(true);
                }
                for (int i = 0; i < 3; i++)
                {
                    hor2.transform.GetChild(i).gameObject.SetActive(true);
                }
                break;
            case 6:
                for (int i = 0; i < 3; i++)
                {
                    hor1.transform.GetChild(i).gameObject.SetActive(true);
                    hor2.transform.GetChild(i).gameObject.SetActive(true);
                }
                break;
            case 7:
                for (int i = 0; i < 4; i++)
                {
                    if (i != 3) hor1.transform.GetChild(i).gameObject.SetActive(true);
                    hor2.transform.GetChild(i).gameObject.SetActive(true);
                }
                break;
        }

        StartCoroutine(CheckSavedInjured());
        //CheckSavedInjured();
    }

    private IEnumerator CheckSavedInjured()
    {

        List<Image> imgs = new List<Image>();

        for (int i = 0; i < hor1.transform.childCount; i++)
        {
            if (save > 0 && hor1.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                hor1.transform.GetChild(i).GetComponent<Image>().sprite = saveSprite;

                Image saveImg = hor1.transform.GetChild(i).GetChild(0).GetComponent<Image>();

                imgs.Add(saveImg);

                save--;
            }
            else break;
        }

        if (save > 0)

            for (int i = 0; i < hor2.transform.childCount; i++)
            {
                if (save > 0 && hor2.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    hor2.transform.GetChild(i).GetComponent<Image>().sprite = saveSprite;

                    Image saveImg = hor2.transform.GetChild(i).GetChild(0).GetComponent<Image>();

                    imgs.Add(saveImg);

                    save--;
                }
            }


        for (int i = 0; i < imgs.Count; i++)
        {
            StartCoroutine(FillImage(imgs[i], 100));

            yield return checkTime;
        };

        //ShowRewards();
        StartCoroutine(ShowRewards());
    }

    private IEnumerator ShowRewards()
    {
        int moneySpeed = 7, honorSpeed = 3;
        checkTime = new WaitForSeconds(Time.deltaTime * fillSpeed * 15);

        StartCoroutine(AddText(moneyText, money, moneySpeed, "+", ""));
        yield return new WaitForSeconds(money / moneySpeed * Time.deltaTime);

        StartCoroutine(AddText(honorText, honor, honorSpeed, "+", ""));
        yield return new WaitForSeconds(honor / honorSpeed * Time.deltaTime);

        GetEvidence();

        StartCoroutine(AddText(stressText, stress, honorSpeed, "", "%"));
        StartCoroutine(FillImage(stressSlider, stress));
        yield return checkTime;
    }

    private IEnumerator FillImage(Image img, int max)
    {
        while (img.fillAmount < max)
        {
            if ((img.fillAmount += Time.deltaTime * fillSpeed) > max) img.fillAmount = max;

            yield return null;
        }
    }

    private IEnumerator AddText(Text text, int max, int speed, string front, string end)
    {
        int count = 0;
        text.text = front + count + end;

        while (count < max)
        {
            if ((count += speed) > max) count = max;
            
            text.text = front + count + end;

            yield return null;
        }
    }

    private void GetEvidence()
    {
        UserDataIO.Stage stage = UserDataIO.ReadStageData();
        int eviIndex = DataManager.Instance.SceneName[5] - 49;

        if (stage.isGotEvidence[eviIndex] == 0 ||true)
        {
            if (left == 0 || true)
            {
                stage.isGotEvidence[eviIndex] = 1;
                UserDataIO.WriteStageData(stage);

                Dictionary<string, object> eviData = ItemDataManager.Instance.GetEvidenceData()[eviIndex];

                evidence.transform.GetChild(0).GetComponent<Text>().text
                    = eviData["evidenceName"].ToString();
                evidence.GetChild(0).gameObject.SetActive(true);
                evidence.GetChild(1).gameObject.SetActive(true);
                evidence.transform.GetChild(1).GetComponent<Image>().sprite = ItemDataManager.Instance.eviSprites[eviIndex];
                evidencePanel.GetChild(1).GetComponent<Image>().sprite = ItemDataManager.Instance.eviSprites[eviIndex];
                evidencePanel.GetChild(2).GetComponent<Text>().text = eviData["evidenceName"].ToString();
                evidencePanel.GetChild(3).GetComponent<Text>().text = eviData["evidenceExplain"].ToString();

                evidencePanel.parent.parent.gameObject.SetActive(true);
            }
            else
            {
                evidence.GetChild(1).gameObject.SetActive(false);
                evidence.GetChild(0).GetComponent<Text>().text = "단서\n획득 실패";
                evidence.GetChild(0).gameObject.SetActive(true);
            }
        }
        else
        {
            evidence.GetChild(1).gameObject.SetActive(false);
            evidence.GetChild(0).GetComponent<Text>().text = "단서\n획득 완료";
            evidence.GetChild(0).gameObject.SetActive(true);
        }
    }

}
