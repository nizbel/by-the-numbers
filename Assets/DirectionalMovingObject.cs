using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalMovingObject : MonoBehaviour
{
    [SerializeField]
    float speed = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate() {
        float speedX = speed * Mathf.Cos(transform.localRotation.eulerAngles.z * Mathf.Deg2Rad);
        float speedY = speed * Mathf.Sin(transform.localRotation.eulerAngles.z * Mathf.Deg2Rad);
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + speedX,
                                                                          transform.position.y + speedY, transform.position.z), Time.deltaTime);
    }
}
