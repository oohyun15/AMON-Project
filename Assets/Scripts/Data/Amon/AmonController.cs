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
 * (19.09.13) 주변에 장애물 있을 때만 애니메이션 실행되도록 변수 추가 및 Collision 함수 수정
 * (19.09.22) 인게임 UI 수정. 인터렉션 관련 행동들은 모두 function call 형태로 바꿈
 * (19.10.03) 카메라 위치값 변수 추가
 * (19.10.04) 장애물 파괴 카운팅
 * (19.10.19) 구조 시 FX 추가
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.14.
 * 수정일자: 19.10.19.
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmonController : MonoBehaviour, IReset
{
    // 플레이어 상태 종류
    public enum InteractionState { Idle, Obstacle, Rescue } 

    [Header("Player Info")]
    public float moveSpeed;
    public float rotSpeed;
    public int damage;                  // 장애물 발차기 공격 데미지
    public float initMoveSpeed;
    public float initRotSpeed;
    public int initDamage;
    public Vector3 initPos;             // 초기 Position 값
    public Quaternion initRot;          // 초기 Rotation 값
    public GameObject initCamPos;       // 카메라 위치 값

    [Header("Player State")]
    public InteractionState state = InteractionState.Idle; // 현재 플레이어의 상태

   /* [Header("Currnet Item")]
    public Item currentItem;            // 플레이어가 현재 가지고 있는 아이템을 받아오는 변수
    public ItemController ItemController; */

    [Header("Obstacle")]
    public Obstacle obstacle;          // 충돌처리된 장애물을 받아올 변수
    public bool attackDelay = false;    // 장애물 공격 시 딜레이를 주기위한 변수
    public GameObject axeAttack;
    public GameObject axeIdle;
    private bool IsEquipAxe = false;
    
    [Header("Rescue")]
    public bool isRescuing;             // 현재 중상 부상자 구조중인지 저장할 변수
    public Transform backPoint;         // 부상자 업었을 때 위치 받아올 변수
    //public Transform rescuers;
    private GameObject target;          // (용현) 부상자(충돌체) 타겟 변수
    private bool isEscaped;             // 플레이어 탈출 확인 변수        // (19.09.22) 이제 필요 없을 듯
    public bool IsEscaped { get { return isEscaped; } }

    [Header("Animation")]
    public Animator playerAnim; // 애니메이터 받아오는 변수
    public enum AnimationName { Idle, Drink, Walk, Strike } // 애니메이션 상태 변수들
    public AnimationName animState = AnimationName.Idle; // 현재 애니메이션 상태
    public bool isCollisionObs = false; // 장애물 충돌 확인 변수
    public bool isTouchBack = false; // 이동 중에 애니메이션을 받아왔는지를 알려주는 변수
    public AnimationClip frontWalk;
    public AnimationClip backWalk;
    public float AttackSpd;

    [Header("CameraShake")]
    public float CSAmount;
    public float CSDuration;

    [Header("Debug")]                // 키보드로 이동할 때 사용하는 변수, 추후에 삭제해야함
    public float h = 0.0f;             // 좌,우
    public float v = 0.0f;             // 상,하
    private bool checkBuff;             // 버프상태인지 확인하는 변수
    private new Transform transform;
    private GameManager gm;
    private new readonly string name = "Player";

    [Header("Destroy")]
    private int shoeLv;
    private int axeLv;
    public int axeDamage;
    private Button attackBtn;

    public List<GameObject> rescuers;
    Dictionary<string, object> itemDataList;
    UserDataIO.User user;

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

        itemDataList = ItemDataManager.Instance.GetEquipItemData();
        
        // (용현) 초기값 저장
        GetInitValue();
    }

    /*
    // Update is called once per frame
    void FixedUpdate()
    {
        // (19.10.29) 임시 디버깅 버튼 비활성화
        
        // Unity에서 디버깅용 버튼. 추후에 삭제해야함
        h = Input.GetAxis("Horizontal");

        v = Input.GetAxis("Vertical");
        
        if (Input.touchCount == 0 && !Input.GetMouseButton(0))
        {
            if (!GameManager.Instance.player.playerAnim.GetBool("IsStrike") || !GameManager.Instance.player.playerAnim.GetBool("IsKick"))
            {
                if (v > 0)
                {
                    JoystickController.instance.isBackMove = false;
                    animState = AnimationName.Walk; // 애니메이션 상태 설정
                    PlayerAnimation();

                    transform.Translate(Vector3.forward * moveSpeed * v * Time.deltaTime, Space.Self);
                }

                else if (v < 0)
                {
                    JoystickController.instance.isBackMove = true;
                    animState = AnimationName.Walk; // 애니메이션 상태 설정
                    PlayerAnimation();

                    transform.Translate(Vector3.forward * moveSpeed * v / 2 * Time.deltaTime, Space.Self);
                }

                else
                {
                    JoystickController.instance.isBackMove = false;
                    AnimationIdle();
                }

                transform.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);
            }
        }

        
        
        // if (Input.GetKeyDown(KeyCode.Space)) Interaction();
    }
    */

    private void OnCollisionStay(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            // 장애물과 충돌할 때 space키를 누르면 장애물을 공격하도록 코딩
            case "Obstacle":

                // 장애물 제거 모드로 변경
                state = InteractionState.Obstacle;
                isCollisionObs = true;

                // 도끼 또는 발차기 이미지로 변경
                gm.interactionImage.gameObject.SetActive(true);

                gm.interactionImage.sprite = isRescuing ? gm.itemImages[2] : gm.itemImages[1];

                obstacle = collision.gameObject.GetComponent<Obstacle>();
                
                // (19.09.22) 인터렉션 UI 변경으로 인한 비활성화
                /*
                // 아이템이 도끼일 경우, 인터렉션 아이템 이미지 활성화
                if (currentItem && currentItem.ID_num == 10)
                {
                    gm.interactionImage.gameObject.SetActive(true);

                    // 0: Axe
                    gm.interactionImage.sprite = gm.itemImages[0];
                }
                */

                break;

            // collision 부상자일 경우 부상자 종류에 따라 상호작용
            case "Injured":

                // 구출모드로 변경
                state = InteractionState.Rescue;
                
                // 구조 아이콘으로 변경
                gm.interactionImage.gameObject.SetActive(true);

                gm.interactionImage.sprite = gm.itemImages[0];

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
                state = InteractionState.Idle;

                gm.interactionImage.gameObject.SetActive(false);

                // (19.09.22) 인터렉션 UI 변경으로 인해 비활성화
                /*
                // (19.09.02) 현재 아이템이 도끼거나 없을 시 인터렉션 아이템 이미지 비활성화
                if (!currentItem || currentItem && currentItem.ID_num == 10)
                {
                    gm.interactionImage.gameObject.SetActive(false);
                }
                */

                obstacle = null; // 충돌이 끝나도 obstacle 유지되던 부분 Fix
                isCollisionObs = false;

                break;
            // 부상자일때
            case "Injured":

                // 상태를 Idle로 변경
                state = InteractionState.Idle;

                gm.interactionImage.gameObject.SetActive(false);

                break;
        }
    }

    // (예진) 플레이어 탈출 
    // (19.08.12. 예진) 탈출 시 따라오는 부상자 있으면 구출 처리 하도록 함
    private void OnTriggerEnter(Collider other)
    {
        // (19.09.22) 태그 이름 바꿈 "Exit" -> "Save"
        if (other.CompareTag("Save") && isRescuing)
        {
            isEscaped = true;

            // 따라오는 부상자 있으면 구출 처리
            foreach (GameObject g in rescuers)
                if (g.activeInHierarchy)
                {
                    g.GetComponent<Injured>().Escaped();

                    UserDataIO.User user = UserDataIO.ReadUserData();
                    user.rescuedCount++;
                    UserDataIO.WriteUserData(user);
                }
            rescuers.Clear();

            gm.CheckLeftInjured();

            AudioManager.Instance.PlayAudio("GameManagerEffect", 1, 0f, false);

            // (19.10.19) 구조 시 FX 추가
            GameObject go = Instantiate(gm.FX_Ingame[0]);
            go.transform.SetParent(gameObject.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

        }

        // (19.09.22) 소방관 탈출 시
        else if (other.CompareTag("Exit"))
        {
            // gm.CheckGameClear();
            // (19.09.26) 탈출 트리거 들어가면 경고 패널 나오도록 설정

            if (gm.CheckLeftInjured() == 0)
                gm.CheckGameClear();
            else
            {
                gm.warningPanel.SetActive(true);

                // 게임 일시 정지
                gm.StopGame();

                JoystickController.instance.StopJoystick();
            }
        }

       /* // (19.08.20) 아이템 획득 시
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
        }*/
    }
    /*
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Exit"))
        {
            gm.warningPanel.SetActive(false);

            isEscaped = false;
        }
    }
    */
    // (예진) 부상자 구조 상호작용
    // (19.09.22) Unity 내에서 버튼에 연결하기 위해 public으로 바꿈
    public void RescueInjured()
    {
        // 구조 상태가 아닐 경우 함수 반환
        if (state != InteractionState.Rescue) return;

        Injured nearInjured = target.gameObject.GetComponent<Injured>();

        // 현재 부상자 업고 있을 경우 구조 불가능. (용현) 이중 if절을 한개로 합침
        // (예진) 부상자 생사여부 확인 조건 추가
        if (!nearInjured.isRescued &&
            !(isRescuing && nearInjured.type == Injured.InjuryType.SERIOUS) &&
            nearInjured.state != Injured.InjuredState.DEAD)
        {
            if (playerAnim.GetBool("IsIdle"))
            {
                playerAnim.SetBool("IsIdle",false);
                playerAnim.SetBool("IsIdleResc", true);
            }

            nearInjured.lyingBody.SetActive(false);
            nearInjured.huggedBody.SetActive(true);
            // 부상자 구조
            nearInjured.Rescue(this);

            // (용현) 구조 후 플레이어 상태 변경
            state = InteractionState.Idle;

            gm.interactionImage.gameObject.SetActive(false);
        }
        AudioManager.Instance.PlayAudio("GameManagerEffect", 0, 0f, false);
    }

    // (19.09.22) 장애물 파괴
    public void DestroyObstacle()
    {
        if (state != InteractionState.Obstacle) return;

        if (animState == AnimationName.Strike) return;
        
        if(!isRescuing)
        {
            axeIdle.SetActive(false);
            axeAttack.SetActive(true);
        }
        attackBtn.interactable = false;
        animState = AnimationName.Strike;
        PlayerAnimation();
    }
    
    public void CalculateDamage()
    {
        if (isRescuing)
        {
            obstacle.hp -= damage;
            Debug.Log("발차기! 데미지는 = " + damage);
        }
        else
        {
            obstacle.hp -= axeDamage;
            Debug.Log("도끼질! 데미지는 = " + axeDamage);
        }

        // (19.10.19) 도끼질 FX 추가
        GameObject go = Instantiate(gm.FX_Ingame[3]);
        go.transform.SetParent(gameObject.transform);
        go.transform.localPosition = new Vector3(0.004f, 0.092f, 0.122f);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one*5f;

        if (obstacle.hp > 0)
        {
            gm.StartCoroutine(GameManager.Instance.Cam.transform.GetComponent<CameraShake>().Shake(CSAmount/4, CSDuration));

            if(playerAnim.GetBool("IsKick")) AudioManager.Instance.PlayAudio("Player", 3, 0f, false);
            else
            {
                if (GameManager.Instance.stageNum < 6) AudioManager.Instance.PlayAudio("Player", 1, 0f, false);
                else AudioManager.Instance.PlayAudio("Player", 2, 0f, false);
            }
        }
        else
        {
            if (obstacle._type == Obstacle.ObsType.GlassDoor) AudioManager.Instance.PlayAudio("Obstacle", 1, 0f, false);
            else AudioManager.Instance.PlayAudio("Obstacle", 0, 0f, false);

            // 코루틴 함수는 모두 게임매니저로 걸어놓음
            gm.StartCoroutine(GameManager.Instance.Cam.transform.GetComponent<CameraShake>().Shake(CSAmount, CSDuration));
            
            // 장애물 비활성화
            obstacle.gameObject.SetActive(false);

            // (용현) 구조 후 플레이어 상태 변경
            state =  InteractionState.Idle;

            // (19.09.02) 인터렉션 버튼 아이템 이미지 비활성화
            gm.interactionImage.gameObject.SetActive(false);


            // 현재 장애물 null로 바꿈
            obstacle = null;
            isCollisionObs = false;

            // (19.10.04) 장애물 파괴 카운트
            UserDataIO.User user = UserDataIO.ReadUserData();

            user.destroyCount++;

            UserDataIO.WriteUserData(user);
        }
    }

    // (19.09.22) 드링크 사용
   /* public void UseDrink()
    {
        // 버프 중이면 사용 X
        if (checkBuff) return;

        Debug.Log("swap 2");

        // 드링크로 무기 교체
        ItemController.Instance.ItemSwap(1);

        if (!currentItem || currentItem.durability <= 0) return;

        currentItem.ItemActive();
    }*/

    // 이동속도 및 회전 속도 증가
    public IEnumerator UpSpeed(int _addSpeed, int _timer)
    {
        checkBuff = true;

        moveSpeed *= _addSpeed;

        // (용현) 회전속도 0.7 -> 0.5
        rotSpeed *= _addSpeed * 0.5f;

        // (용현) 조이스틱에서 이동속도 업데이트
        JoystickController.instance.UpdateSpeed();

        yield return new WaitForSeconds(_timer);

        checkBuff = false;

        moveSpeed = initMoveSpeed;

        rotSpeed = initRotSpeed;

        // (용현) 조이스틱에서 이동속도 업데이트
        JoystickController.instance.UpdateSpeed();
    }

    // (19.09.22) 인터렉션 버튼 미사용
    // (용현) 인터렉션 버튼
    public void Interaction()
    {
        /*
        // (9.9 태윤, 이동 중에 인터렉션 시 움직임을 멈추도록 조건문 추가)
        if (JoystickController.instance.isTouch) 
        {
            JoystickController.instance.isTouch = false;
            isTouchBack = true;
        }
        */

        switch (state)
        {
            // 기본 상태
            case InteractionState.Idle:

                TouchBack(); // Idle 상태에서 인터렉션 버튼 사용 시 이동이 끊기지 않도록 수정
                Debug.Log("Do Nothing");

                break;

            // 장애물 제거
            case InteractionState.Obstacle:

                DestroyObstacle();

                break;
           
            // 부상자 구출
            case InteractionState.Rescue:

                RescueInjured();

                break;
        } 
    }
    
    // (용현) 초기값 저장
    public void GetInitValue()
    {
        transform = GetComponent<Transform>();

        isRescuing = false;

        isEscaped = false;

        initMoveSpeed = moveSpeed;

        initRotSpeed = rotSpeed;

        initDamage = damage;

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

        checkBuff = false;
        
        damage = initDamage;

        axeAttack.SetActive(false);

        axeIdle.SetActive(true);

        attackBtn = GameManager.Instance.UI[1].transform.GetChild(0).transform.GetChild(0).GetComponent<Button>();
    }

    public void PlayerAnimation() // (9.9 태윤, 움직이다가 애니메이션 바뀌는 것때문에 IsWalk도 false로 바꾸도록 함)
    {
        switch (animState)
        {
            case AnimationName.Walk:

                playerAnim.SetBool("IsIdle", false);
                playerAnim.SetBool("IsIdleResc", false);

                if (isRescuing)
                {
                    if (playerAnim.GetBool("IsWalk")) playerAnim.SetBool("IsWalk", false);
                    playerAnim.SetBool("IsWalkResc", true);

                    if (JoystickController.instance.isBackMove) playerAnim.SetBool("IsBackMove", true);
                    else playerAnim.SetBool("IsBackMove", false);
                }
                else
                {
                    if (playerAnim.GetBool("IsWalkResc")) playerAnim.SetBool("IsWalkResc", false);
                    playerAnim.SetBool("IsWalk", true);

                    if (JoystickController.instance.isBackMove) playerAnim.SetBool("IsBackMove", true);
                    else playerAnim.SetBool("IsBackMove", false);
                }

                playerAnim.SetFloat("WalkAnimSpd", moveSpeed/5);
                // AudioManager.Instance.PlayAudio("Player", 0, 0f, false);
                break;

            case AnimationName.Strike:

                if (JoystickController.instance.isTouch)
                {
                    JoystickController.instance.isTouch = false;
                    isTouchBack = true;
                }

                playerAnim.SetBool("IsIdle", false);
                playerAnim.SetBool("IsIdleResc", false);
                playerAnim.SetBool("IsWalk", false);
                playerAnim.SetBool("IsWalkResc", false);

                if (isRescuing)
                {
                    playerAnim.SetBool("IsKick", true);
                }

                else
                {
                    playerAnim.SetBool("IsStrike", true);
                } 

                break;

            case AnimationName.Drink:

                playerAnim.SetBool("IsIdle", false);
                playerAnim.SetBool("IsIdleResc", false);
                playerAnim.SetBool("IsWalk", false);
                playerAnim.SetBool("IsWalkResc", false);

                playerAnim.SetBool("IsDrink", true);
                
                break;
        }
    }

    public void AnimationIdle()
    {
        if (axeAttack.activeInHierarchy)
        {
            axeAttack.SetActive(false);
            axeIdle.SetActive(true);
        }

        animState = AnimationName.Idle;

        foreach (AnimatorControllerParameter prmt in playerAnim.parameters)
        {
            if (prmt.name.Contains("Is"))
            {
                playerAnim.SetBool(prmt.name, false);
            }
            else if (prmt.name == "WalkAnimSpd") playerAnim.SetFloat(prmt.name, moveSpeed / 5);
            else playerAnim.SetFloat(prmt.name, AttackSpd);
        }
        if(isRescuing) playerAnim.SetBool("IsIdleResc", true);
        else playerAnim.SetBool("IsIdle", true);
        JoystickController.instance.isBackMove = false;
    }

    public void TouchBack() // 인터렉션 때 움직임을 멈춘 부분을 다시 되돌려 조이스틱을 다시 클릭하지 않아도 움직이도록 하는 함수 
    {
        if (isTouchBack)
        {
            JoystickController.instance.isTouch = true;
            isTouchBack = false;
        }
        else return;
    }

    public void SetButtonOn()
    {
       attackBtn.interactable = true;
    }
}
