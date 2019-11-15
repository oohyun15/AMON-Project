/****************************************
 * EquipItemMat.cs
 * 제작: 김태윤
 * 아이템 레벨에 따른 텍스쳐 변경 스크립트
 * 작성일자: 19.11.10
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipItemMat : MonoBehaviour
{
    public string item;
    public Texture[] itemTexture;
    public Button itemUpgradeButton;

    private string[] textureName;
    private int itemLv;
    private Material[] itemMat;

    public void Start()
    {
        UpdateItemMat(item);
    }

    public void UpdateItemMat(string _item)
    {
        UserDataIO.User user = UserDataIO.ReadUserData();
        _item = item;
        itemLv = 0;

        switch (_item)
        {
            case "oxygen":
                itemLv = user.oxygenlv;
                itemMat = GetComponent<MeshRenderer>().materials;
                GetComponent<MeshRenderer>().materials[0].mainTexture = itemTexture[itemLv];
                break;

            case "axe":
                itemLv = user.axelv;
                itemMat = GetComponent<MeshRenderer>().materials;
                GetComponent<MeshRenderer>().materials[0].mainTexture = itemTexture[itemLv];
                break;

            case "gloves":
                itemLv = user.gloveslv;
                itemMat = GetComponent<SkinnedMeshRenderer>().materials;
                GetComponent<SkinnedMeshRenderer>().materials[0].mainTexture = itemTexture[itemLv];
                break;

            case "shoes":
                itemLv = user.shoeslv;
                itemMat = GetComponent<SkinnedMeshRenderer>().materials;
                GetComponent<SkinnedMeshRenderer>().materials[0].mainTexture = itemTexture[itemLv];
                break;
        }
    }
}
