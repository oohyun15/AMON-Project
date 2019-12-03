using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credit : MonoBehaviour
{
    bool isTouched;
    public int speed;
    private float bottom;
    public Text txtTouch;
    public GameObject btnTouch;

    private void Start()
    {
        isTouched = false;

        bottom = transform.parent.position.y 
            - transform.parent.GetComponent<RectTransform>().rect.height / 2;

        StartCoroutine(StartCredit());
    }

    IEnumerator StartCredit()
    {
        float height = GetComponent<RectTransform>().rect.height / 2;
        
        while (transform.position.y - height < bottom)
        {
            if (Input.GetMouseButtonDown(0)) speed *= 2;
            else if (Input.GetMouseButtonUp(0)) speed /= 2;

            transform.Translate(new Vector2(0, Time.deltaTime * speed));

            yield return null;
        }

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Vector4 tempColor;

        while (txtTouch.color.a < 1)
        {
            tempColor = txtTouch.color;
            tempColor.w += Time.deltaTime * 3;
            txtTouch.color = tempColor;

            yield return null;
        }

        btnTouch.SetActive(true);
    }

    public void MoveScene(string scene)
    {
        Lobby.MoveScene(scene);
    }
}
