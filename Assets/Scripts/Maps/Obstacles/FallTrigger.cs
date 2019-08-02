/****************************************
 * AmonController.cs
 * 제작: 김태윤
 * 트리거를 밟으면 장애물 추락하는 효과 추가 스크립트
 * (19.07.31) 자식 오브젝트를 찾을 때 순서보다는 Find를 통해 직접 찾는 걸로 수정
 * (19.08.02) FInd 사용하지 않고 AmonController는 GameManager에서 불러와 사용함
 * 작성일자: 19.07.09
 * 수정일자: 19.08.02
 ***************************************/

using System.Collections;
using UnityEngine;


public class FallTrigger : MonoBehaviour
{
    //public Obstacle obstacle; // 인스턴스화할 장애물을 받아오는 변수, 하이라키창에서 Object 직접 연결

    public Material _material; // 하이라키창에서 material 직접 연결
    public GameObject fallObs;
    public GameObject obs;
    
    private void OnTriggerEnter(Collider other)
    {
        // (용현) AmonController를 GameManager에서 가져오고 게임매니저가 코루틴돌게 수정
        if (other.gameObject == GameManager.Instance.player.gameObject) GameManager.Instance.StartCoroutine(FallingObs());
    }

    IEnumerator FallingObs()
    {
        Renderer renderer = transform.GetComponent<MeshRenderer>();

        // Red material 적용
        renderer.material = _material;

        // 3초 뒤 장애물 생성, rigidbody에 의해 생성된 위치에서 자동으로 떨어짐
        yield return new WaitForSeconds(3f);

        //Instantiate(obstacle.gameObject, new Vector3(1.09f, 4.82f, -11.86f), Quaternion.identity);

        // Find함수로 대체함
        GameObject fallObs = transform.Find("Fallobstacle").gameObject; 
        GameObject obs = transform.Find("obstacle").gameObject;
        fallObs.SetActive(true);
        fallObs.transform.parent = null;

        yield return new WaitForSeconds(0.9f);

        Destroy(fallObs);
        obs.SetActive(true);

        // 떨어지는 오브젝트와 길막는 오브젝트를 시간 간격을 두고 생성 및 파괴
        obs.transform.parent = null;

        gameObject.SetActive(false);
    }
}
