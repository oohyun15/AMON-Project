/****************************************
 * AmonController.cs
 * 제작: 김태윤
 * 트리거를 밟으면 장애물 추락하는 효과 추가 스크립트
 * (19.07.31)자식 오브젝트를 찾을 때 순서보다는 Find를 통해 직접 찾는 걸로 수정, 
 * 작성일자: 19.07.09
 * 수정일자: 19.07.31
 ***************************************/

using System.Collections;
using UnityEngine;


public class FallTrigger : MonoBehaviour
{
    //public Obstacle obstacle; // 인스턴스화할 장애물을 받아오는 변수, 하이라키창에서 Object 직접 연결

    public Material _material; // 하이라키창에서 material 직접 연결
    
    private void OnTriggerEnter(Collider other)
    {
        AmonController amon = other.transform.GetComponent<AmonController>();
        if (amon) StartCoroutine(FallingObs());
    }

    IEnumerator FallingObs()
    {
        Renderer renderer = transform.GetComponent<MeshRenderer>();
        renderer.material = _material; // Red material 적용

        yield return new WaitForSeconds(3f); // 3초 뒤 장애물 생성, rigidbody에 의해 생성된 위치에서 자동으로 떨어짐 

        //Instantiate(obstacle.gameObject, new Vector3(1.09f, 4.82f, -11.86f), Quaternion.identity);

        GameObject fallObs = transform.Find("Fallobstacle").gameObject; // Find함수로 대체함
        GameObject obs = transform.Find("obstacle").gameObject;
        fallObs.SetActive(true);
        fallObs.transform.parent = null;

        yield return new WaitForSeconds(0.9f);

        Destroy(fallObs);
        obs.SetActive(true);
        obs.transform.parent = null;  // 떨어지는 오브젝트와 길막는 오브젝트를 시간 간격을 두고 생성 및 파괴

        Destroy(gameObject);
    }
}
