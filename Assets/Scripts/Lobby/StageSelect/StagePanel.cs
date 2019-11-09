﻿/****************************************
 * StagePanel.cs
 * 제작: 김용현
 * 로비 씬에서 스테이지 패널에 대한 스크립트
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * (19.11.09) 스테이지 스프라이트 추가
 * (19.11.09 ㅇㅈ) stage_data 로드 방식 수정
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
    // (19.11.09 예진) stage data 로드 방식 변경
    public void SetStage(int stageNum)
    {
        int stageCount = stageNum*3;

        int idx_level = 0;

        for (int i = 0; i < stageData.Count; i++)
        {
            string tempStage = stageData[i]["sceneName"].ToString();

            // sceneName의 값이 공백이 아닐 때
            if (tempStage != " ")

                // 큰 스테이지가 stageNum과 같을 경우 다음 실행
                if (tempStage[5] - 48 == stageNum + 1)
                {
                    int idx_data = stageCount;

                    stageLevel[idx_level].stageName.text = stageData[i]["sceneName"].ToString();

                    int total = System.Convert.ToInt32(stageData[i]["save"]);
                    int rescueNum = stage.rescueNum[idx_data];

                    // (예진) 스테이지마다 총 부상자 수가 다르기 때문에
                    // 원래 방식으로 부상자 스프라이트 변경하면 에러가 나요
                    // 그래서 스테이지의 부상자 수에 맞게 오브젝트 활성화되게 해줌
                    for (int j = 0; j < stageLevel[idx_level].rescueImage.Length; j++)
                    {
                        if (j < total)
                            stageLevel[idx_level].rescueImage[j].gameObject.SetActive(true);
                        else
                            stageLevel[idx_level].rescueImage[j].gameObject.SetActive(false);
                    }

                    if (stage.isPlayed[idx_data] == 0)
                    {
                        for (int idx = 0; idx < total; idx++) stageLevel[idx_level].rescueImage[idx].sprite = rescueSprite[0];
                    }
                    else
                    {
                        for (int idx = 0; idx < rescueNum; idx++) stageLevel[idx_level].rescueImage[idx].sprite = rescueSprite[1];

                        for (int idx = total - 1; idx >= rescueNum; idx--) stageLevel[idx_level].rescueImage[idx].sprite = rescueSprite[2];
                    }

                    idx_level++;
                }
                // 큰 스테이지가 stageNum과 같지 않고 레벨 인덱스가 0이 아닌 경우
                // 읽고자 하는 스테이지의 다음 스테이지를 읽고 있는 것이므로 for문을 종료
                else if (idx_level != 0)
                {
                    break;
                }
        }
    }

    public void ChangeStage(int num)
    {
        if (index+num >= 0 && index+num < totalStageNum)
        {
            index += num;

            stageImage.sprite = stageSprite[index];

            // (예진) 이거 그냥 하드코딩하면 안돼요? ㅎㅎ
            stageTitle.text = ItemDataManager.Instance.stageTitle[index];
            
            SetStage(index);
        }
    }
}
