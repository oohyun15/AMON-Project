/****************************************
 * AmonController.cs
 * 제작: 김태윤, 조예진, 김용현
 * Amon캐릭터의 움직임 및 구조자와 아이템의 상호작용 코드
 * (19.07.30) UI 인터렉션 버튼을 구현을 위한 Interaction 함수 정의(Space 키)
 * (19.08.01) IReset 클래스 상속, GetInitValue 추가
 * (19.08.04) 장애물 충돌 시 플레이어 상태 수정(우선순위: 아이템 > 장애물)
 * (19.08.16) 태윤 : 플레이어 애니메이션 변수 및 함수 추가 
 * (19.08.19) attackDelay를 공격 애니메이션에 맞게 수정
 * (19.08.20) 캐릭터 초기화 시 currentItem = null로 수정
 * (19.09.02) 인터렉션 버튼에 아이템 이미지 추가
 * (19.09.09) 이동 중 애니메이션 버그 수정
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.14
 * 수정일자: 19.09.09
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
    public Obstacle obstacle;          // 충돌처리된 장애물을 받아올 변수
    public bool attackDelay = false;    // 장애물 공격 시 딜레이를 주기위한 변수

    [Header("Rescue")]
    public bool isRescuing;             // 현재 중상 부상자 구조중인지 저장할 변수
    public Transform backPoint;         // 부상자 업었을 때 위치 받아올 변수
    //public Transform rescuers;
    private GameObject target;          // (용현) 부상자(충돌체) 타겟 변수
    private bool isEscaped;             // 플레이어 탈출 확인 변수
    public bool IsEscaped { get { return isEscaped; } }

    [Header("Animation")]
    public Animator playerAnim; // 애니메이터 받아오는 변수
    public enum AnimationName { Idle, Drink, Walk, Strike } // 애니메이션 상태 변수들
    public AnimationName animState = AnimationName.Idle; // 현재 애니메이션 상태

    [Header("CameraShake")]
    public float CSAmount;
    public float CSDuration;

    [Header("ETC")]
    private bool isTouchBack = false; // 이동 중에 애니메이션을 받아왔는지를 알려주는 변수

    // [Header("Debug")]                // 키보드로 이동할 때 사용하는 변수, 추후에 삭제해야함
    private float h = 0.0f;             // 좌,우
    private float v = 0.0f;             // 상,하
    private new Transform transform;
    private GameManager gm;
    private new readonly string name = "Player";

    public List<GameObject> rescuers;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello world!");

        gm = GameManager.Instance;
        playerAnim = GetComponent<Animator>();
        playerAnim.SetBool("IsIdle", true);

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

        // (용현) 초기값 저장
        GetInitValue();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Unity에서 디버깅용 버튼. 추후에 삭제해야함
        h = Input.GetAxis("Horizontal");

        v = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * moveSpeed * v * Time.deltaTime, Space.Self);

        transform.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);

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

                // 아이템이 도끼일 경우, 인터렉션 아이템 이미지 활성화
                if (currentItem && currentItem.ID_num == 10)
                {
                    gm.interactionImage.gameObject.SetActive(true);

                    // 0: Axe
                    gm.interactionImage.sprite = gm.itemImages[0];
                }
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
            // 장애물일때
            case "Obstacle":

                // (용현) 구조 후 플레이어 상태 변경, 아이템 들고있을 때 고려함
                state = currentItem ? InteractionState.Item : InteractionState.Idle;

                // (19.09.02) 현재 아이템이 도끼거나 없을 시 인터렉션 아이템 이미지 비활성화
                if (!currentItem || currentItem && currentItem.ID_num == 10)
                {
                    gm.interactionImage.gameObject.SetActive(false);
                }

                obstacle = null; // 충돌이 끝나도 obstacle 유지되던 부분 Fix

                break;
            // 부상자일때
            case "Injured":

                // 상태를 Idle로 변경
                state = InteractionState.Idle;

                break;
        }
    }

    // (예진) 플레이어 탈출 
    // (19.08.12. 예진) 탈출 시 따라오는 부상자 있으면 구출 처리 하도록 함
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exit"))
        {
            isEscaped = true;

            // 따라오는 부상자 있으면 구출 처리
            foreach (GameObject g in rescuers)
                if (g.activeInHierarchy)
                    g.GetComponent<Injured>().Escaped();

            rescuers.Clear();

            gm.CheckGameClear();
        }

        // (19.08.20) 아이템 획득 시
        else if(other.CompareTag("FieldItem"))
        {
            FieldItem fi = other.GetComponent<FieldItem>();

            for (int index = 0; index < ItemController.TIN; index++)
            {
                Item item = ItemController.keyItems[index];

                if (fi.ID_num == item.ID_num)
                {
                    item.durability += fi.itemCount;

                    ItemController.UpdateItemCount(item);

                    other.gameObject.SetActive(false);

                    break;
                }
            }
        }
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
        // (예진) 부상자 생사여부 확인 조건 추가
        if (!nearInjured.isRescued &&
            !(isRescuing && nearInjured.type == Injured.InjuryType.SERIOUS) &&
            nearInjured.state != Injured.InjuredState.DEAD)
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

        // 맨손
        if (!currentItem) _obstacle.hp -= damage;

        // 도끼
        // (19.08.02 용현) 내구도가 있을때만 작동하도록 수정 
        else if (currentItem.ID_num == 10 &&
                 currentItem.durability > 0)
        {
            // 도끼 데미지 넣는 코드인데 수정 필요할 거 같음
            _obstacle.hp -= currentItem.transform.GetComponent<ItemWeapon>().addDamage;

            currentItem.ItemActive();
        }

        // 도끼가 아닌 아이템을 들고 있을 때 그 아이템 먼저 사용 (아직 3번키 아이템 적용 안됨!!)
        else
        {
            currentItem.ItemActive();

            state = InteractionState.Obstacle;
        }

        // 이건 추후에 Obstacle 클래스에 추가해야 할 듯
        if (_obstacle.hp <= 0)
        {
            // 코루틴 함수는 모두 게임매니저로 걸어놓음
            gm.StartCoroutine(GameManager.Instance.Cam.transform.GetComponent<CameraShake>().Shake(CSAmount, CSDuration));

            // 장애물 비활성화
            _obstacle.gameObject.SetActive(false);
            
            // (용현) 구조 후 플레이어 상태 변경
            state = currentItem ? InteractionState.Item : InteractionState.Idle;

            // (19.09.02) 인터렉션 버튼 아이템 이미지 비활성화
            gm.interactionImage.gameObject.SetActive(false);

            // 현재 장애물 null로 바꿈
            obstacle = null;
        }

        // (19.08.19) 공격 애니메이션 총 길이로 설정
        yield return new WaitForSeconds(1f);

        attackDelay = false;
    }

    // 이동속도 및 회전 속도 증가
    public IEnumerator UpSpeed(int _addSpeed, int _timer)
    {
        moveSpeed *= _addSpeed;

        // (용현) 회전속도 0.7 -> 0.5
        rotSpeed *= _addSpeed * 0.5f;

        // (용현) 조이스틱에서 이동속도 업데이트
        JoystickController.instance.UpdateSpeed();

        yield return new WaitForSeconds(_timer);

        moveSpeed = initMoveSpeed;

        rotSpeed = initRotSpeed;

        // (용현) 조이스틱에서 이동속도 업데이트
        JoystickController.instance.UpdateSpeed();
    }

    // (용현) 인터렉션 버튼
    public void Interaction()
    {
        if(JoystickController.instance.isTouch) // (9.9 태윤, 이동 중에 인터렉션 시 움직임을 멈추도록 조건문 추가)
        {
            JoystickController.instance.isTouch = false;
            isTouchBack = true;
        }

        switch (state)
        {
            // 기본 상태
            case InteractionState.Idle:

                Debug.Log("Do Nothing");

                break;

            // 아이템 사용
            case InteractionState.Item:

                currentItem.ItemActive();

                // (19.09.02) 아이템 이미지 비활성화
                if (!currentItem) gm.interactionImage.gameObject.SetActive(false);

                break;

            // 장애물 제거
            case InteractionState.Obstacle:

                if (!attackDelay) gm.StartCoroutine(DestroyObs(obstacle));

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
        ItemController = gameObject.transform.GetComponent<ItemController>();

        transform = GetComponent<Transform>();

        isRescuing = false;

        isEscaped = false;

        initMoveSpeed = moveSpeed;

        initRotSpeed = rotSpeed;

        initPos = gameObject.transform.position;

        initRot = gameObject.transform.rotation;
    }

    // (용현) 초기값 설정
    public void SetInitValue()
    {
        rescuers = new List<GameObject>();

        // 플레이어가 죽었을 때
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);

            gm.Cam.transform.SetParent(gameObject.transform);
        }
        isRescuing = false;

        isEscaped = false;

        state = InteractionState.Idle;

        AnimationIdle();

        gameObject.transform.SetPositionAndRotation(initPos, initRot);

        moveSpeed = initMoveSpeed;

        rotSpeed = initRotSpeed;

        currentItem = null;
    }

    public void PlayerAnimation() // (9.9 태윤, 움직이다가 애니메이션 바뀌는 것때문에 IsWalk도 false로 바꾸도록 함)
    {
        switch (animState)
        {
            case AnimationName.Walk:

                playerAnim.SetBool("IsIdle", false);
                playerAnim.SetBool("IsWalk", true);

                break;

            case AnimationName.Strike:

                playerAnim.SetBool("IsIdle", false);
                playerAnim.SetBool("IsWalk", false);
                playerAnim.SetBool("IsStrike", true);

                break;

            case AnimationName.Drink:

                playerAnim.SetBool("IsIdle", false);
                playerAnim.SetBool("IsWalk", false);
                playerAnim.SetBool("IsDrink", true);
                
                break;
        }
    }

    public void AnimationIdle()
    {
        animState = AnimationName.Idle;

        foreach (AnimatorControllerParameter prmt in playerAnim.parameters)
        {
            playerAnim.SetBool(prmt.name, false);
        }
        playerAnim.SetBool("IsIdle", true);
    }

    private void TouchBack() // 인터렉션 때 움직임을 멈춘 부분을 다시 되돌려 조이스틱을 다시 클릭하지 않아도 움직이도록 하는 함수 
    {
        if (isTouchBack)
        {
            JoystickController.instance.isTouch = true;
            isTouchBack = false;
        }
        else return;
    }
}
