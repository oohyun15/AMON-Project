using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public GameObject FogPlane;
    public Transform Character;
    public LayerMask FogLayer;
    public float radius;
    public float radiusSqr { get { return radius * radius; } }

    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = new Ray(transform.position, Character.position - transform.position);
        RaycastHit hit;
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

    void UpdateColor()
    {
        mesh.colors = colors;
    }
}
