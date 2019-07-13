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
    public CameraShake _camera; // 화면 흔듦을 위해 카메라를 받아올 변수

    private int damage;
    public Item currentItem; // 플레이어가 현재 가지고 있는 아이템을 받아오는 변수
    public ItemController itemcontroller;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello world!");
        itemcontroller = gameObject.transform.GetComponent<ItemController>();
        transform = GetComponent<Transform>();
        damage = 1;
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
  
        v = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * moveSpeed * v * Time.deltaTime, Space.Self);

        transform.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space)) // 아이템 사용 버튼 - space 키
        {
            if (currentItem == null) return;
            else
            {
                if (currentItem.transform.GetComponent<ItemWeapon>() != null) return; //무기는 평소에 사용 안되도록 구현
                else
                {
                    currentItem.ItemActive();
                }
            }
        } 
    }

    private void OnCollisionStay(Collision collision) // 장애물과 충돌할 때 space키를 누르면 장애물을 공격하도록 코딩
    {
        if (!attackDelay)
        {
            if (collision.gameObject.GetComponent<Obstacle>() != null)
            {
                if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(DestroyObs(collision.gameObject.GetComponent<Obstacle>()));
            }
            else if (collision.gameObject.GetComponent<Item>() != null)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    attackDelay = true;
                    StartCoroutine(itemcontroller.AddItem(collision.gameObject.GetComponent<Item>()));
                    attackDelay = false;
                } 
            }
        }
        
        
    }

    IEnumerator DestroyObs(Obstacle _obstacle) // 딜레이를 위해 코루틴을 사용하였다.
    {
        attackDelay = true;

        if (currentItem == null)
        {
            _obstacle.hp -= damage;
        }
        else if (currentItem.transform.GetComponent<ItemWeapon>() != null) // 무기 사용 시 추가 데미지가 있도록 별개로 구현
        {
            _obstacle.hp -= currentItem.transform.GetComponent<ItemWeapon>().addDamage;
            currentItem.ItemActive();
        }
        else // 맨손 또는 무기를 든 상태가 아닐 경우 장애물과의 상호작용이 없도록 함
        {
            yield return new WaitForSeconds(0.1f);
            attackDelay = false;
            yield break;
        }

        if(_obstacle.hp <= 0)
        {
            StartCoroutine(_camera.Shake(0.01f, 0.3f));
            Destroy(_obstacle.gameObject);
        }
        yield return new WaitForSeconds(0.1f);
        attackDelay = false;
    }

    public IEnumerator UpSpeed(int _addSpeed, int _timer) // 이동속도 및 회전 속도 증가
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
