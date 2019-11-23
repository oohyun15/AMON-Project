using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCameraRot : MonoBehaviour
{
    public GameObject center;

    void Start()
    {
        StartCoroutine(rot());
    }


    IEnumerator rot()
    {
        while (true)
        {
            gameObject.transform.RotateAround(center.transform.position, 
                new Vector3(0,1,0), 
                Time.deltaTime * 50);

            yield return null;
        }
    }
}
