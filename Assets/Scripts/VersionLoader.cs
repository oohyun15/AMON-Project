/****************************************
 * VersionLoader.cs
 * 제작: 김용현
 * 현재 버전을 불러오는 스크립트
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.11.30
 ***************************************/

using UnityEngine;
using UnityEngine.UI;

public class VersionLoader : MonoBehaviour
{
    public Text version;

    // Start is called before the first frame update
    void Start()
    {
        version.text = "Version: " + Application.version;
    }
}
