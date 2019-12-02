using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credit : MonoBehaviour
{
    bool isTouched;
    public int speed;

    private void Start()
    {
        isTouched = false;

        StartCoroutine(StartCredit());
    }

    IEnumerator StartCredit()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0)) speed *= 2;
            else if (Input.GetMouseButtonUp(0)) speed /= 2;

            transform.Translate(new Vector2(0, Time.deltaTime * speed));

            yield return null;
        }
    }

    public void MoveScene(string scene)
    {
        Lobby.MoveScene(scene);
    }
}
