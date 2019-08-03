/****************************************
 * WallActive.cs
 * 제작: 김태윤
 * 카메라와 FireFighter 사이에 있는 벽이 시야를 가리는 것을 방지하기위해 벽을 투명하게 바꾸는 스크립트
 * Raycast와 Rendering Mode 변경, Material.Color의 알파값 수정을 통해서 구현
 * Foreach문은 오류도 뜨고 For문에 비해 2배 좀 안되게 느리대서 전부 For문으로 수정함
 * (!) 벽이 투명해질때 벽 뒤쪽이 보이게되므로 벽을 두껍게 만들어야 할 듯
 * (!) 투명도를 조절할 경우 투명해진 벽에 의한 그림자와 조명 역시 변경되므로 이 부분에 대한 논의 필요
 * (08.03) target을 GameManager의 Player를 받아오도록 변경, _camera는 GameManager의 Cam을 받아오도록 설정
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.07.30
 * 수정일자: 19.08.03
 ***************************************/
using System.Collections.Generic;
using UnityEngine;

public class WallActive : MonoBehaviour
{
    public GameObject target; //FireFighter를 받아옴

    private Camera _camera; //target을 따라다니는 카메라를 받아옴
    private Vector3 targetPoint; //Ray설정을 위한 Target 위치
    private List<GameObject> listWall = new List<GameObject>(); //투명화된 벽을 받아오는 List
    private List<GameObject> listHitInfo;
    private void Start()
    {
        target = GameManager.Instance.player.gameObject;
        _camera = GameManager.Instance.Cam.GetComponent<Camera>();
        listHitInfo = new List<GameObject>();
    }

    void Update()
    {
        targetPoint = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

        Ray ray = new Ray(transform.position, targetPoint - transform.position); // 카메라에서 Target 방향으로 Ray설정 
        RaycastHit[] hits; // RaycastAll을 사용하기때문에 Ray와 부딪히는 모든 오브젝트의 정보를 받기위해서 배열을 사용

        hits = Physics.RaycastAll(ray, 2f); // Ray 발사

        for(int j = 0; j < hits.Length; j++) // 시야를 가리는 벽을 투명하게 만드는 For문
        {
           RaycastHit hit = hits[j]; // hits에 저장된 충돌체 정보를 하나하나 조건에 맞춰보기위한 변수
            
           listHitInfo.Add(hit.collider.gameObject); // Ray와 부딪힌 모든 충돌체를 받아놓는다, 이후 listWall과의 비교를 위해서 사용
           if (hit.collider.tag == "Wall") // 충돌체의 태그가 Wall이면 == 충돌체가 벽 오브젝트라면
           {
              if (!listWall.Contains(hit.collider.gameObject)) // 이미 listWall에 추가돼있을 경우 중복하지 않음
              { 
                 listWall.Add(hit.collider.gameObject); // listWall에 충돌체를 추가
                 Material hitMat = hit.collider.gameObject.GetComponent<MeshRenderer>().materials[0]; // 변수에 메터리얼을 설정
                 SetupMaterialWithBlendMode(hitMat, BlendMode.Fade); // material의 Rendering Mode를 변경하는 함수, 밑에 구체적인 설명
                 hit.collider.gameObject.GetComponent<MeshRenderer>().materials[0].color 
                        = new Color(hitMat.color.r, hitMat.color.g, hitMat.color.b, 0f); // 알파값을 0으로 하여 투명하게 만듦
              }
           }
        }
        Debug.DrawRay(ray.origin, ray.direction * 2, Color.red);
         

        for(int k = 0; k < listWall.Count; k++) // 벽이 더이상 시야를 가리지 않을 경우 알파값을 원래대로 돌려놓는 For문
        {
                GameObject wall = listWall[k]; // listWall에 담긴 벽 오브젝트 정보를 하나하나 받아오는 변수

                if (!listHitInfo.Contains(wall))
                {  // listHitinfo에 저장되어있지않다는 것은 Wall에 담긴 오브젝트가 더이상 Ray와 충돌하지 않는다는 것을 의미함
                Material wallMat = wall.GetComponent<MeshRenderer>().materials[0]; // wall의 메터리얼을 받아오는 변수
                SetupMaterialWithBlendMode(wallMat, BlendMode.Opaque); // 원래 Rendering Mode로 변경
                wall.GetComponent<MeshRenderer>().materials[0].color
                    = new Color(wallMat.color.r, wallMat.color.g, wallMat.color.b, 1f); // 원래 알파값으로 변경
                listWall.Remove(wall); // list에서 오브젝트를 제거
                }
        }
        listHitInfo.Clear(); // 매 프레임마다 listHitInfo는 초기화해준다.
        // Ray와 더이상 충돌되지 않는다면, listWall에는 남아있지만 다음 listHitInfo에는 저장되지 않게되고 이를 비교하여 원래 알파값으로 되돌림
    }

    // Rendering Mode 전환 코드
   

    public enum BlendMode // BlendMode의 키워드들 선언, 렌더링 모드는 다음과 같은 4가지 모드가 있다.
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    /*굳이 Rendering Mode를 변경하는 이유는 Rendering Mode에 따라 투명도 조절이 달라지기 때문이다.
   우리가 필요한 부분만 설명하자면 현재 우리가 적용한 material은 Opaque로 이는 불투명 모드이기때문에 알파값이 변경돼도 투명도가 변하지않는다.
   Fade, Transparent의 경우 알파값에 따라 투명도 조절이 가능한 모드로 대신 이 모드들은 사용 시 Opaque보다 더 많은 용량을 잡아먹는다고 한다.
   따라서 필요한 때에만 렌더링 모드를 변경하여 사용하도록 구현하였다.
   
   아래의 레퍼런스 : https://answers.unity.com/questions/1004666/change-material-rendering-mode-in-runtime.html
   Rendering Mode에 관한 유니티 매뉴얼 : https://docs.unity3d.com/kr/current/Manual/StandardShaderMaterialParameterRenderingMode.html */

    public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case BlendMode.Fade:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case BlendMode.Transparent:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }
}
