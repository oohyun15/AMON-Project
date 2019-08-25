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

    List<Dictionary<string, object>> itemDataList;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        LoadItemData();
    }

    public List<Dictionary<string,object>> GetEquipItemData()
    {
        if (itemDataList == null) LoadItemData();

        return itemDataList;
    }

    private void LoadItemData()
    {
        itemDataList = CSVReader.Read("Data/oxygen_data");
    }
}
