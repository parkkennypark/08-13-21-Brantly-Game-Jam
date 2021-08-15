using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public Vector3 offset;

    void Start()
    {

    }

    void Update()
    {
        transform.position = transform.parent.position + offset;
        transform.rotation = Quaternion.identity;
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.y = transform.parent.eulerAngles.y;
        transform.eulerAngles = eulerAngles;
    }
}
