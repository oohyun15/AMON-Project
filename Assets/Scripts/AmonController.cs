using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmonController : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;

    private new Transform transform;
    public float moveSpeed;
    public float rotSpeed;

    private Obstacle obstacle; // 충돌처리된 장애물을 받아올 변수
    public bool attackDelay = false; // 장애물 공격 시 딜레이를 주기위한 변수
    public CameraShake camera; // 화면 흔듦을 위해 카메라를 받아올 변수

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello world!");

        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
  
        v = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * moveSpeed * v * Time.deltaTime, Space.Self);

        transform.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);
    }

    private void OnCollisionStay(Collision collision) // 장애물과 충돌할 때 space키를 누르면 장애물을 공격하도록 코딩
    {
        Obstacle nearObs = collision.gameObject.GetComponent<Obstacle>();
        if(!attackDelay && nearObs != null)
        {
            if (Input.GetKey(KeyCode.Space)) StartCoroutine(DestroyObs(nearObs));
        }
        
    }

    IEnumerator DestroyObs(Obstacle _obstacle) // 딜레이를 위해 코루틴을 사용하였다.
    {
        attackDelay = true;
        _obstacle.hp -= 1;

        if(_obstacle.hp <= 0)
        {
            StartCoroutine(camera.Shake(0.01f, 0.3f));
            Destroy(_obstacle.gameObject);
        }
        yield return new WaitForSeconds(0.1f);
        attackDelay = false;
    }
}
