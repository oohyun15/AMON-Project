/****************************************
 * StagePanel.cs
 * 제작: 김용현
 * 로비 씬에서 스테이지 패널에 대한 스크립트
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.10.12
 * 수정일자: 19.10.12
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePanel : MonoBehaviour
{   
    public StageLevel[] stageLevel;
    public Sprite[] stageSprite;
    public Sprite[] rescueSprite;
    public Image stageImage;
    public Text stageTitle;

    public int totalStageNum;
    private int index = 0;

    private UserDataIO.User user;
    private List<Dictionary<string, object>> stageData;
    private readonly string stageDataPath = "Data/stage_data";

    // Start is called before the first frame update
    void Start()
    {
        user = UserDataIO.ReadUserData();

        stageData = CSVReader.Read(stageDataPath);

        SetStage(0);
    }

    // 아직 스테이지 1만 있어서 이런식으로 구현함
    // 이후에는 totalStageNum에 따라서 ChangeStage 함수에서 스테이지 3개 설정 해줘야함
    public void SetStage(int stageNum)
    {
        int stageCount = stageNum*3;

        int temp = 0;

        for (int index = stageCount; index < stageCount+3; index++)
        {
            stageLevel[temp++].stageName.text = stageData[index]["sceneName"].ToString();

            Debug.Log(index);
        }
    }

    public void ChangeStage(int num)
    {
        if (index+num >= 0 && index+num < totalStageNum)
        {
            index += num;
            stageImage.sprite = stageSprite[index];
            SetStage(index);
        }
    }
}
