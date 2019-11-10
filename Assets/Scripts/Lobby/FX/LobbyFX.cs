/****************************************
 * LobbyFX.cs
 * 제작: 김용현
 * 로비 내에서 사용되는 FX 목록
 * 작성일자: 19.11.10
 * 수정일자: 19.11.10
 ***************************************/

using UnityEngine;

public class LobbyFX : MonoBehaviour
{
    public static LobbyFX instance = null;       // 싱글톤 구현을 위한 변수

    public GameObject FX_Lists;
    public GameObject FX_Touch;
    public GameObject FX_AchieveGlow;
    public GameObject FX_Evidence;
    public GameObject FX_GearUp;
    public GameObject FX_LockerGlow;
    public GameObject FX_EvidenceGlow;
    public GameObject FX_HonorUp;
    public GameObject FX_Alarm;

    void Awake()
    {
        // 싱글톤 구현, 인스턴스가 이미 있는지 확인, 없으면 인스턴스를 this로 할당
        if (instance == null) instance = this;

        // 인스턴스가 this로 할당되있다면  게임오브젝트 삭제
        else if (instance != this) Destroy(gameObject);
    }
}
