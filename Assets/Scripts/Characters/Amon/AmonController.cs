/****************************************
 * AmonController.cs
 * 제작: 김태윤, 조예진
 * Amon캐릭터의 움직임 및 구조자와 아이템의 상호작용 코드
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.14
 ***************************************/

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

    public bool isRescuing;             // 현재 중상 부상자 구조중인지 저장할 변수
    public Transform backPoint;         // 부상자 업었을 때 위치 받아올 변수
    public Transform rescuers;

    private Obstacle obstacle;          // 충돌처리된 장애물을 받아올 변수
    public bool attackDelay = false;    // 장애물 공격 시 딜레이를 주기위한 변수
    public CameraShake _camera;         // 화면 흔듦을 위해 카메라를 받아올 변수

    private int damage;
    public Item currentItem;            // 플레이어가 현재 가지고 있는 아이템을 받아오는 변수
    public ItemController itemcontroller;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello world!");

        itemcontroller = gameObject.transform.GetComponent<ItemController>();

        transform = GetComponent<Transform>();

        isRescuing = false;

        damage = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Unity에서 디버깅용 버튼. 추후에 삭제해야함
        h = Input.GetAxis("Horizontal");

        v = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * moveSpeed * v * Time.deltaTime, Space.Self);

        transform.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);

        // 아이템 사용 버튼 - space 키
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentItem == null) return;

            else
            {
                //무기는 평소에 사용 안되도록 구현
                if (currentItem.transform.GetComponent<ItemWeapon>() != null) return;

                else currentItem.ItemActive();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            // 장애물과 충돌할 때 space키를 누르면 장애물을 공격하도록 코딩
            case "Obstacle":
                Obstacle nearObs = collision.gameObject.GetComponent<Obstacle>();

                if (!attackDelay && nearObs != null)
                {
                    if (Input.GetKey(KeyCode.Space)) StartCoroutine(DestroyObs(nearObs));
                }
                break;

            // collision 부상자일 경우 부상자 종류에 따라 상호작용
            case "Injured":

                RescueInjured(collision.gameObject);

                break;
        }
    }

    // (예진) 부상자 구조 상호작용
    private void RescueInjured(GameObject nearObject)
    {
        Injured nearInjured = nearObject.gameObject.GetComponent<Injured>();

        if (Input.GetKey(KeyCode.Space) && !nearInjured.isRescued)
        {
            // 현재 부상자 업고 있을 경우 구조 불가능
            if (nearInjured.type == Injured.InjuryType.SERIOUS && isRescuing) return;

            nearInjured.Rescue(this);
        }
    }

    // 딜레이를 위해 코루틴을 사용하였다.
    IEnumerator DestroyObs(Obstacle _obstacle) 
    {
        attackDelay = true;

        if (currentItem == null)
        {
            _obstacle.hp -= damage;
        }
        // 무기 사용 시 추가 데미지가 있도록 별개로 구현
        else if (currentItem.transform.GetComponent<ItemWeapon>() != null) 
        {
            _obstacle.hp -= currentItem.transform.GetComponent<ItemWeapon>().addDamage;

            currentItem.ItemActive();
        }
        // 맨손 또는 무기를 든 상태가 아닐 경우 장애물과의 상호작용이 없도록 함
        else
        {
            yield return new WaitForSeconds(0.1f);

            attackDelay = false;

            yield break;
        }

        if (_obstacle.hp <= 0)
        {
            StartCoroutine(_camera.Shake(0.01f, 0.3f));

            Destroy(_obstacle.gameObject);
        }
        yield return new WaitForSeconds(0.1f);

        attackDelay = false;
    }

    // 이동속도 및 회전 속도 증가
    public IEnumerator UpSpeed(int _addSpeed, int _timer)
    {
        float orgMoveSpeed = moveSpeed;

        float orgRotSpeed = rotSpeed;

        moveSpeed *= _addSpeed;

        rotSpeed *= _addSpeed * 0.7f;

        yield return new WaitForSeconds(_timer);

        moveSpeed = orgMoveSpeed;

        rotSpeed = orgRotSpeed;
    }
}
