using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadDriver : MonoBehaviour
{
    public Material TargetMaterial;
    private static readonly int ObjectPosition = Shader.PropertyToID("_ObjectPosition");
    private static readonly int YRot = Shader.PropertyToID("_YRot");

    [SerializeField] private Vector3 sineMovement;
    [SerializeField] private float speed = 1f;
    private Vector3 startPos;

    public Vector3 Offset;

    public float RotSpeed;

    void Start()
    {
        startPos = transform.position;
    }
    
    void Update()
    {
        transform.position = startPos + (Mathf.Sin(Time.time * speed) * sineMovement);
        transform.eulerAngles += Vector3.up * RotSpeed * Time.deltaTime;
        if(TargetMaterial!=null)TargetMaterial.SetVector(ObjectPosition, -transform.position + Offset);
        TargetMaterial.SetFloat(YRot, -transform.eulerAngles.y * Mathf.Deg2Rad);
    }
}
