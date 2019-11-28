/****************************************
 * AmonController.cs
 * 제작: 김태윤
 * 트리거를 밟으면 장애물 추락하는 효과 추가 스크립트
 * (19.07.31) 자식 오브젝트를 찾을 때 순서보다는 Find를 통해 직접 찾는 걸로 수정
 * (19.08.02) FInd 사용하지 않고 AmonController는 GameManager에서 불러와 사용함
 * (19.08.03) IReset 인터페이스 추가
 * (19.08.07) 초기화 관련 수정
 * (19.10.19) 폭발 스크립트 추가
 * (19.11.03) 천장 오브젝트(CellingFragments)를 통해 새롭게 로직 구성
 * (19.11.16) 천장 오브젝트 관련 FX 추가
 * (19.11.17) 장애물 떨어지고 나서 남아있도록 수정
 * 작성일자: 19.07.09
 * 수정일자: 19.11.03
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FallTrigger : MonoBehaviour, IReset
{
    //public Obstacle obstacle; // 인스턴스화할 장애물을 받아오는 변수, 하이라키창에서 Object 직접 연결

    // public Material _material;                       // 하이라키창에서 material 직접 연결
    public GameObject cellingFragments;              // 장애물(Obstacle)
    public GameObject cellingFX;                     // 장애물 떨어지는 위치
    public Fragments wallFragments;                  // 부서지는 벽(Fragments)
    public float time;
    public int type;                                 // 0: 장애물, 1: 부서지는 벽

    private new Renderer renderer;
    private Material initMaterial;
    private Vector3 initTriggerPos;
    private Vector3 initCellingPos;

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
        if (other.gameObject == gm.player.gameObject)
        {
            switch (type)
            {
                case 0:
                    gm.StartCoroutine(FallingObs());
                    break;

                case 1:
                    gm.StartCoroutine(Explosion());
                    break;
            }

        }
    }

    IEnumerator FallingObs()
    {
        // Red material 적용
        // renderer.material = _material;

        // FX 활성화
        cellingFX.SetActive(true);
        if(!AudioManager.Instance.audioPlayers[7].isPlaying) AudioManager.Instance.PlayAudio("Warning", 0, 0f, true);
        ;        // time 뒤 장애물 생성, rigidbody에 의해 생성된 위치에서 자동으로 떨어짐
        yield return new WaitForSeconds(time);

        // FX 비활성화
        cellingFX.SetActive(false);

        // 천장 조각들 중력 사용
        for (int idx = 0; idx < cellingFragments.transform.childCount; idx++)
        {
            cellingFragments.transform.GetChild(idx).GetComponent<Rigidbody>().useGravity = true;
        }
        yield return new WaitForSeconds(1.0f);

        // 발판(트리거) 비활성화
        AudioManager.Instance.StopAudio("Warning");
        gameObject.SetActive(false);

        // 떨어지는 장애물 비활성화
        for (int idx = 0; idx < cellingFragments.transform.childCount; idx++)
        {
            cellingFragments.transform.GetChild(idx).GetComponent<FallObstacle>().isFalling = false;
        }
    }

    IEnumerator Explosion()
    {
        // Red material 적용
        // renderer.material = _material;

        // 3초 뒤 장애물 생성, rigidbody에 의해 생성된 위치에서 자동으로 떨어짐
        yield return new WaitForSeconds(time);

        wallFragments.Explosion();
    }

    public void GetInitValue()
    {
        // initMaterial = renderer.material;

        initTriggerPos = gameObject.transform.position;

        if (type == 0)
        {
            initCellingPos = cellingFragments.transform.position;

            cellingFX.SetActive(false);
        }
    }

    public void SetInitValue()
    {
        // renderer.material = initMaterial;

        gameObject.SetActive(true);

        gameObject.transform.position = initTriggerPos;

        if (type == 0) cellingFragments.SetActive(true);

    }
}
