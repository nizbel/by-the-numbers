using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundElement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        float playerSpeed = StageController.controller.GetPlayerShipSpeed();

        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x - playerSpeed,
                                                                          transform.position.y, transform.position.z), Time.deltaTime);
    }
}
