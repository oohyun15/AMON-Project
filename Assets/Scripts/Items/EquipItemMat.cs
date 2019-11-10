using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItemMat : MonoBehaviour
{
    public string item;
    public Texture[] itemTexture;

    private string[] textureName;
    private int itemLv;
    private Material[] itemMat;

    public void Start()
    {
        UserDataIO.User user = UserDataIO.ReadUserData();
        itemLv = 0;

        switch (item)
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
        Debug.Log(itemMat[itemLv].name);
        // Debug.Log("mat = " + GetComponent<SkinnedMeshRenderer>().materials[0].name);
        Debug.Log(itemLv);
    }
}
