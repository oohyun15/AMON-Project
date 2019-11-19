/****************************************
 * LoadingManager.cs
 * 제작: 조예진
 * 로딩씬
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.11.19
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static string nextScene;

    public Sprite[] imgs;
    public Image img;

    private void Start()
    {
        img.sprite = imgs[Random.Range(0, imgs.Length)];

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);

        //StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;

        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);

        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            yield return null;

            Debug.Log(op.progress);
        }

        op.allowSceneActivation = true;
    }
}
