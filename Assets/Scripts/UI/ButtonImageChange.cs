/****************************************
 * ButtonImageCHange.cs
 * 제작: 김태윤
 * InteractionButton 이미지 교체하는 스크립트
 * 작성일자: 19.07.09
 * 수정일자: 19.08.07
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonImageChange : MonoBehaviour
{
    public GameObject axeImage;
    public GameObject kickImage;

    private bool _isRescuing = false;

   
    // Update is called once per frame
    void Update()
    {
        _isRescuing = GameManager.Instance.player.isRescuing;
        if (_isRescuing)
        {
            if (axeImage.activeInHierarchy) axeImage.SetActive(false);

            kickImage.SetActive(true);
        }
        else
        {
            if (kickImage.activeInHierarchy) kickImage.SetActive(false);

            axeImage.SetActive(true);
        }
    }
}
