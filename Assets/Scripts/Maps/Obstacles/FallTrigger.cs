/****************************************
 * AmonController.cs
 * 제작: 김태윤
 * 트리거를 밟으면 장애물 추락하는 효과 추가 스크립트
 * (19.07.31) 자식 오브젝트를 찾을 때 순서보다는 Find를 통해 직접 찾는 걸로 수정
 * (19.08.02) FInd 사용하지 않고 AmonController는 GameManager에서 불러와 사용함
 * (19.08.03) IReset 인터페이스 추가
 * (19.08.07) 초기화 관련 수정
 * (19.10.19) 폭발 스크립트 추가
 * 작성일자: 19.07.09
 * 수정일자: 19.10.19
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FallTrigger : MonoBehaviour, IReset
{
    //public Obstacle obstacle; // 인스턴스화할 장애물을 받아오는 변수, 하이라키창에서 Object 직접 연결

    public Material _material;                       // 하이라키창에서 material 직접 연결
    public GameObject FallObs;                      // 장애물(Obstacle) 또는 부서지는 벽(Fragments)를 넣을 수 있음
    public GameObject obs;
    public float time;

    private new Renderer renderer;
    private Material initMaterial;
    private Vector3 initTriggerPos;
    private Vector3 initFallObsPos;

    private GameManager gm;
    private new readonly string name = "FallTrigger";
    

    void Start()
    {
        gm = GameManager.Instance;

        renderer = transform.GetComponent<MeshRenderer>();

        var go = new List<GameObject>();

        // Object에 키가 있으면 추가
        if (gm.objects.ContainsKey(name))
            gm.objects[name].Add(gameObject);

        // 키가 없을 경우 생성
        else
        {
            gm.objects.Add(name, go);

            gm.objects[name].Add(gameObject);
        }

        GetInitValue();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // (용현) AmonController를 GameManager에서 가져오고 게임매니저가 코루틴돌게 수정
        if (other.gameObject == gm.player.gameObject) gm.StartCoroutine(FallingObs());
    }

    IEnumerator FallingObs()
    {
        // Red material 적용
        renderer.material = _material;

        // time 뒤 장애물 생성, rigidbody에 의해 생성된 위치에서 자동으로 떨어짐
        yield return new WaitForSeconds(time);

        FallObs.SetActive(true);

        yield return new WaitForSeconds(0.9f);

        // 발판(트리거) 비활성화
        gameObject.SetActive(false);

        // 떨어지는 장애물 비활성화
        FallObs.SetActive(false);

        // 기존에 있던 장애물 활성화
        obs.SetActive(true);
    }

    IEnumerator Explosion()
    {
        // Red material 적용
        renderer.material = _material;

        // 3초 뒤 장애물 생성, rigidbody에 의해 생성된 위치에서 자동으로 떨어짐
        yield return new WaitForSeconds(time);
    }

    public void GetInitValue()
    {
        initMaterial = renderer.material;

        initTriggerPos = gameObject.transform.position;

        initFallObsPos = FallObs.transform.position;
    }

    public void SetInitValue()
    {
        renderer.material = initMaterial;

        gameObject.SetActive(true);

        gameObject.transform.position = initTriggerPos;

        FallObs.transform.position = initFallObsPos;

        for (int i =0; i < FallObs.transform.childCount; i++)
        {
            // 자식(벽돌)들에게 초기화 명령
            FallObs.transform.GetChild(i).GetComponent<FallObstacle>().SetInitValue();
        }

        FallObs.SetActive(false);

        obs.GetComponent<Obstacle>().initActive = false;
    }
}
