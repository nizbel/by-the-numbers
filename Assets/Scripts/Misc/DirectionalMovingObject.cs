﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalMovingObject : MonoBehaviour
{
    [SerializeField]
    float speed = 1.5f;

    [SerializeField]
    float offsetAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate() {
        float directionAngle = transform.localRotation.eulerAngles.z + offsetAngle;

        float speedX = speed * Mathf.Cos(directionAngle * Mathf.Deg2Rad);
        float speedY = speed * Mathf.Sin(directionAngle * Mathf.Deg2Rad);
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + speedX,
                                                                          transform.position.y + speedY, transform.position.z), Time.deltaTime);
    }
}