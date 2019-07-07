using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmonController : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;

    private Transform transform;
    public float moveSpeed;
    public float rotSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello world!");

        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
  
        v = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * moveSpeed * v * Time.deltaTime, Space.Self);

        transform.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);
    }
}
