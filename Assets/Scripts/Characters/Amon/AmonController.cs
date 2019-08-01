/****************************************
 * AmonController.cs
 * 제작: 김태윤, 조예진, 김용현
 * Amon캐릭터의 움직임 및 구조자와 아이템의 상호작용 코드
 * (19.07.30) UI 인터렉션 버튼을 구현을 위한 Interaction 함수 정의(Space 키)
 * (19.08.01) IReset 클래스 상속, GetInitValue 추가
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.14
 * 수정일자: 19.08.01
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmonController : MonoBehaviour, IReset
{
    // 플레이어 상태 종류
    public enum InteractionState { Idle, Item, Obstacle, Rescue } 

    [Header("Player Info")]
    public float moveSpeed;
    public float rotSpeed;
    public int damage;                  // 장애물 공격 데미지
    public float initMoveSpeed;
    public float initRotSpeed;
    public Vector3 initPos;             // 초기 Position 값
    public Quaternion initRot;          // 초기 Rotation 값

    [Header("Player State")]
    public InteractionState state = InteractionState.Idle; // 현재 플레이어의 상태

    [Header("Currnet Item")]
    public Item currentItem;            // 플레이어가 현재 가지고 있는 아이템을 받아오는 변수
    public ItemController ItemController;

    [Header("Obstacle")]
    [SerializeField]
    private Obstacle obstacle;          // 충돌처리된 장애물을 받아올 변수
    public bool attackDelay = false;    // 장애물 공격 시 딜레이를 주기위한 변수
    public CameraShake _camera;         // 화면 흔듦을 위해 카메라를 받아올 변수

    [Header("Rescue")]
    public bool isRescuing;             // 현재 중상 부상자 구조중인지 저장할 변수
    public Transform backPoint;         // 부상자 업었을 때 위치 받아올 변수
    public Transform rescuers;
    private GameObject target;          // (용현) 부상자(충돌체) 타겟 변수
    private bool isEscaped;             // 플레이어 탈출 확인 변수
    public bool IsEscaped { get { return isEscaped; } }
    
    [Header("Debug")]                   // 키보드로 이동할 때 사용하는 변수, 추후에 삭제해야함
    [SerializeField]
    private float h = 0.0f;             // 좌,우
    [SerializeField]
    private float v = 0.0f;             // 상,하
    private new Transform transform;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello world!");

        ItemController = gameObject.transform.GetComponent<ItemController>();

        transform = GetComponent<Transform>();

        isRescuing = false;

        isEscaped = false;

        // (용현) 초기값 저장
        GetInitValue();
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
        if (Input.GetKeyDown(KeyCode.Space)) Interaction();
    }

    private void OnCollisionStay(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            // 장애물과 충돌할 때 space키를 누르면 장애물을 공격하도록 코딩
            case "Obstacle":

                // 장애물 제거 모드로 변경
                state = InteractionState.Obstacle;

                obstacle = collision.gameObject.GetComponent<Obstacle>();

                break;

            // collision 부상자일 경우 부상자 종류에 따라 상호작용
            case "Injured":

                // 구출모드로 변경
                state = InteractionState.Rescue;

                // (용현) 부상자를 target으로 설정
                target = collision.gameObject;

                break;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        switch(collision.gameObject.tag)
        {
            // 장애물 및 부상자일 때 동일하게 적용
            case "Obstacle":
            case "Injured":

                // 상태를 Idle로 변경
                state = InteractionState.Idle;

                break;
        }
    }

    // (예진) 플레이어 탈출 확인
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exit"))
            isEscaped = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Exit"))
            isEscaped = false;
    }

    // (예진) 부상자 구조 상호작용
    private void RescueInjured(GameObject nearObject)
    {
        Injured nearInjured = nearObject.gameObject.GetComponent<Injured>();

        // 현재 부상자 업고 있을 경우 구조 불가능. (용현) 이중 if절을 한개로 합침
        if (!nearInjured.isRescued &&
            !(isRescuing && nearInjured.type == Injured.InjuryType.SERIOUS))
        {
            // 부상자 구조
            nearInjured.Rescue(this);

            // (용현) 구조 후 플레이어 상태 변경
            state = currentItem ? InteractionState.Item : InteractionState.Idle;
        }
    }

    // 딜레이를 위해 코루틴을 사용하였다.
    private IEnumerator DestroyObs(Obstacle _obstacle)
    {
        attackDelay = true;

        if (currentItem == null) _obstacle.hp -= damage;

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
            // 코루틴 함수는 모두 게임매니저로 걸어놓음
            GameManager.Instance.StartCoroutine(_camera.Shake(0.01f, 0.3f));

            // 장애물 비활성화
            _obstacle.gameObject.SetActive(false);
            
            // (용현) 구조 후 플레이어 상태 변경
            state = currentItem ? InteractionState.Item : InteractionState.Idle;



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

        // (용현) 회전속도 0.7 -> 0.5
        rotSpeed *= _addSpeed * 0.5f;

        // (용현) 조이스틱에서 이동속도 업데이트
        JoystickController.instance.UpdateSpeed();

        yield return new WaitForSeconds(_timer);

        moveSpeed = orgMoveSpeed;

        rotSpeed = orgRotSpeed;

        // (용현) 조이스틱에서 이동속도 업데이트
        JoystickController.instance.UpdateSpeed();
    }

    // (용현) 인터렉션 버튼
    public void Interaction()
    {
        switch (state)
        {
            // 기본 상태
            case InteractionState.Idle:

                Debug.Log("Do Nothing");

                break;

            // 아이템 사용
            case InteractionState.Item:

                currentItem.ItemActive();

                break;

            // 장애물 제거
            case InteractionState.Obstacle:

                if (!attackDelay) GameManager.Instance.StartCoroutine(DestroyObs(obstacle));

                break;
           
            // 부상자 구출
            case InteractionState.Rescue:

                RescueInjured(target);

                break;
        } 

        
    }

    // (용현) 초기값 저장
    public void GetInitValue()
    {
        initMoveSpeed = moveSpeed;

        initRotSpeed = rotSpeed;

        initPos = gameObject.transform.position;

        initRot = gameObject.transform.rotation;
    }
    // (용현) 초기값 설정
    public void SetInitValue()
    {
        // 플레이어가 죽었을 때
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);

            GameManager.Instance.Cam.transform.SetParent(gameObject.transform);
        }
        isRescuing = false;

        isEscaped = false;

        gameObject.transform.SetPositionAndRotation(initPos, initRot);

        moveSpeed = initMoveSpeed;

        rotSpeed = initRotSpeed;
    }
}
