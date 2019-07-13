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

    public bool isRescuing; // 현재 구조중인지 저장할 변수
    public Transform backPoint; // 부상자 업었을 때 위치 받아올 변수
    public Transform rescuers;

    private Obstacle obstacle; // 충돌처리된 장애물을 받아올 변수
    public bool attackDelay = false; // 장애물 공격 시 딜레이를 주기위한 변수
    public CameraShake _camera; // 화면 흔듦을 위해 카메라를 받아올 변수

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello world!");

        transform = GetComponent<Transform>();

        isRescuing = false;
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
  
        v = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * moveSpeed * v * Time.deltaTime, Space.Self);

        transform.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);
    }

    private void OnCollisionStay(Collision collision)    {
        switch (collision.gameObject.tag) {
            case "Obstacle": // 장애물과 충돌할 때 space키를 누르면 장애물을 공격하도록 코딩
                Obstacle nearObs = collision.gameObject.GetComponent<Obstacle>();
                if (!attackDelay && nearObs != null)
                {
                    if (Input.GetKey(KeyCode.Space)) StartCoroutine(DestroyObs(nearObs));
                }
                break;

            case "Injured": // collision 부상자일 경우 부상자 종류에 따라 상호작용
                Injured nearInjured = collision.gameObject.GetComponent<Injured>();

                if (Input.GetKey(KeyCode.Space) && !nearInjured.isRescued)
                {
                    // 현재 부상자 업고 있을 경우 구조 불가능
                    if (nearInjured.type == Injured.InjuryType.SERIOUS && isRescuing) break;
                    nearInjured.Rescue(this);
                }

                break;
        }
    }

    IEnumerator DestroyObs(Obstacle _obstacle) // 딜레이를 위해 코루틴을 사용하였다.
    {
        attackDelay = true;
        _obstacle.hp -= 1;

        if(_obstacle.hp <= 0)
        {
            StartCoroutine(_camera.Shake(0.01f, 0.3f));
            Destroy(_obstacle.gameObject);
        }
        yield return new WaitForSeconds(0.1f);
        attackDelay = false;
    }
}
