/****************************************
 * FogOfWar.cs
 * 제작: 김용현
 * 스테이지 내 전장의 안개를 구현한 코드로 캐릭터 위치에 따라 안개 plane의 alpha값을 변경하는 코드
 * https://www.youtube.com/watch?v=iGAdaZ1ICaI 참고
 * (19.10.05) 미니맵에 안개 추가. IReset 추가
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.09
 * 수정일자: 19.10.05
 ***************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour, IReset
{
    public GameObject FogPlane;             // 안개 Plane
    public Transform Character;             // 캐릭터 좌표(Transform) 변수
    public LayerMask FogLayer;              // 안개의 LayerMask 변수
    public float radius;                    // 캐릭터 중심으로 밝혀질 원의 반지름
    public float radiusSqr { get { return radius * radius; } }  // 반지름 제곱

    private Mesh mesh;                      // 안개 Plane의 Mesh 컴포넌트를 받기위한 변수
    private Vector3[] vertices;             // 안개에서 보여질 위치(벡터)값
    private Color[] colors;                 // 안개의 색깔 변수
    private Color[] initColors;             // 초기 안개 색깔
    private GameManager gm;
    private new readonly string name = "FogOfWar";

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;

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

        // 게임 시작 시 안개 게임오브젝트에 대해 초기화
        Initialize();

        GetInitValue();

    }

    // Update is called once per frame
    void Update()
    {
        // 캐릭터 사망 시 오류뜨는거 방지용 if문. 김태윤 추가
        if (Character != null) 
        {
            Ray r = new Ray(transform.position, Character.position - transform.position);

            RaycastHit hit;

            // 캐릭터 중심으로 radius만큼 시야가 밝혀짐
            if (Physics.Raycast(r, out hit, 1000, FogLayer, QueryTriggerInteraction.Collide))
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 v = FogPlane.transform.TransformPoint(vertices[i]);

                    float dist = Vector3.SqrMagnitude(v - hit.point);

                    if (dist < radiusSqr)
                    {
                        float alpha = Mathf.Min(colors[i].a, dist / radiusSqr);

                        colors[i].a = alpha;
                    }
                }
                UpdateColor();
            }
        }
    }

    // 안개 초기화 함수
    void Initialize()
    {
        mesh = FogPlane.GetComponent<MeshFilter>().mesh;

        vertices = mesh.vertices;

        colors = new Color[vertices.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.black;
        }
        UpdateColor();
    }

    // 안개 Mesh를 업데이트하는 함수
    void UpdateColor()
    {
        mesh.colors = colors;
    }

    public void GetInitValue()
    {
        /* Not Implemented */
    }

    public void SetInitValue()
    {
        Initialize();
    }
}
