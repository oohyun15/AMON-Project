using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FallTrigger : MonoBehaviour
{
    public Obstacle obstacle; // 인스턴스화할 장애물을 받아오는 변수, 하이라키창에서 Object 직접 연결
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
        
        Instantiate(obstacle.gameObject, new Vector3(1.09f, 4.82f, -11.86f), Quaternion.identity);

        Destroy(gameObject);
    }
}
