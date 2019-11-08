/****************************************
 * StagePanel.cs
 * 제작: 김용현
 * 로비 씬에서 스테이지 패널에 대한 스크립트
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * (19.11.09) 스테이지 스프라이트 추가
 * 작성일자: 19.10.12
 * 수정일자: 19.11.09
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePanel : MonoBehaviour
{   
    public StageLevel[] stageLevel;
    public Sprite[] stageSprite;        // 0: FastFood, 1: Building, 2: School
    public Sprite[] rescueSprite;       // 0: Before, 1: Success, 2: Fail
    public Image stageImage;
    public Text stageTitle;

    public int totalStageNum;
    private int index = 0;

    private UserDataIO.Stage stage;
    private List<Dictionary<string, object>> stageData;
    private readonly string stageDataPath = "Data/stage_data";

    // Start is called before the first frame update
    void Start()
    {
        stage = UserDataIO.ReadStageData();

        stageData = CSVReader.Read(stageDataPath);

        SetStage(0);
    }

    // 아직 Stage01만 있어서 이런식으로 구현함
    // 이후에는 totalStageNum에 따라서 ChangeStage 함수에서 스테이지 3개 설정 해줘야함
    public void SetStage(int stageNum)
    {
        int stageCount = stageNum*3;

        for (int idx_data = stageCount, idx_level = 0; idx_data < stageCount+3; idx_data++, idx_level++)
        {
            stageLevel[idx_level].stageName.text = stageData[idx_data]["sceneName"].ToString();

            int rescueNum = stage.rescueNum[idx_data];

            Debug.Log(rescueNum);

            if (stage.isPlayed[idx_data] == 0)
            {
                for (int idx = 0; idx < 3; idx++) stageLevel[idx_level].rescueImage[idx].sprite = rescueSprite[0];
            }
            else
            {
                for (int idx = 0; idx < rescueNum; idx++) stageLevel[idx_level].rescueImage[idx].sprite = rescueSprite[1];

                for (int idx = 2; idx >= rescueNum; idx--) stageLevel[idx_level].rescueImage[idx].sprite = rescueSprite[2];
            }
        }
    }

    public void ChangeStage(int num)
    {
        if (index+num >= 0 && index+num < totalStageNum)
        {
            index += num;

            stageImage.sprite = stageSprite[index];

            stageTitle.text = stageData[index]["stageTitle"].ToString();
            
            SetStage(index);
        }
    }
}
