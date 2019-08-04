using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - Player.instance.transform.position;
    }

    void LateUpdate()
    {
        transform.position = Player.instance.transform.position + offset;
    }
}
