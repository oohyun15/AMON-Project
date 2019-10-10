/****************************************
 * ItemDataManager.cs
 * 제작: 조예진
 * 아이템 정보가 담긴 csv 파일을 불러오고 저장하기 위한 스크립트
 * Lobby 씬에서 생성되어 DontDestroyOnLoad로 계속 살아있는 오브젝트이지만
 * Lobby 씬을 거치지 않고 Debug 씬에서만 실행할 것을 대비, Debug 씬에도 따로 오브젝트를 만들어 두었고
 * 로비 씬으로 들어갈 때마다 Data 오브젝트가 쌓이는 것을 대비하여 start 함수에서 instance를 확인하여 
 * ItemDataManager 컴포넌트가 하나만 존재하도록 해두었습니다
 * (19.10.10) 단서 데이터 함께 저장하도록 함 
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.08.07
 * 수정일자: 19.08.11
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataManager : MonoBehaviour
{
    private static ItemDataManager instance;
    public static ItemDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(ItemDataManager)) as ItemDataManager;

                if (instance == null)
                {
                    Debug.LogError("There's no active ItemDataManager object");
                }
            }
            return instance;
        }
    }

    Dictionary<string, object> itemDataList;

    private readonly string evidenceDataPath = "Data/evidence_data";
    public List<Dictionary<string, object>> evidenceData;

    private readonly string rankDataPath = "Data/rank_data";
    public List<Dictionary<string, object>> rankData;

    public Sprite[] eviSprites;


    void Start()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void LoadEquipItemData()
    {
        Debug.Log("loading equip item data");

        itemDataList = new Dictionary<string, object>
        {
            ["oxygen"] = CSVReader.Read("Data/oxygen_data"),
            ["gloves"] = CSVReader.Read("Data/gloves_data"),
            ["axe"] = CSVReader.Read("Data/axe_data")
        };
    }

    public void LoadEvidenceData()
    {
        Debug.Log("loading Evidence Data");

        evidenceData = CSVReader.Read(evidenceDataPath);
    }

    public void LoadRankData()
    {
        rankData = CSVReader.Read(rankDataPath);
    }

    // (19.09.11.) 장착 아이템 개수 추가 -> 검색 편의를 위해 딕셔너리 리스트 형식으로 변경
    public Dictionary<string,object> GetEquipItemData()
    {
        if (itemDataList == null)
            LoadEquipItemData();

        return itemDataList;
    }

    public List<Dictionary<string,object>> GetEvidenceData()
    {
        if (evidenceData == null)
            LoadEvidenceData();

        return evidenceData;
    }

    public List<Dictionary<string, object>> GetRankData()
    {
        if (rankData == null)
            LoadRankData();

        return rankData;
    }
}
