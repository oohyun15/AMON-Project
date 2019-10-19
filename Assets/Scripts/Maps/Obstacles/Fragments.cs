/****************************************
 * Fragments.cs
 * 제작: 김용현
 * 부서지는 벽 관련 스크립트
 * (19.10.19) 폭발 스크립트 추가
 * 작성일자: 19.10.13.
 * 수정일자: 19.10.19.
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragments : MonoBehaviour, IReset
{

    // [System.Serializable]
    public class Fragment
    {
        public GameObject fragment;
        public Vector3 initPos;
        public Quaternion initRot;
    }

    public Fragment[] _fragment;
    private GameManager gm;
    private int num;
    private new readonly string name = "Fragments";

    public void GetInitValue()
    {
        num = transform.childCount;

        _fragment = new Fragment[num];


        for (int index = 0; index < num; index++)
        {
            _fragment[index] = new Fragment();

            _fragment[index].fragment = transform.GetChild(index).gameObject;
            _fragment[index].initPos = _fragment[index].fragment.transform.position;
            _fragment[index].initRot = _fragment[index].fragment.transform.rotation;
        }
    }

    public void SetInitValue()
    {
        for (int index = 0; index < num; index++)
        {
            _fragment[index].fragment.transform.SetPositionAndRotation(_fragment[index].initPos, _fragment[index].initRot);
        }
    }

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

        GetInitValue();

    }
}
