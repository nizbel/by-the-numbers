using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundElement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // TODO Decide about speeds, whether it should be decided here or not
        if (transform.GetComponent<Formation>() != null) {
            transform.GetComponent<DestructibleObject>().SetSpeed(StageController.controller.GetPlayerShipSpeed());

            // Do the same for every child
            foreach (Transform child in transform) {
                child.GetComponent<DestructibleObject>().SetSpeed(StageController.controller.GetPlayerShipSpeed()); 
            }
        } else {
            // Default case, every foreground element is destructible
            transform.GetComponent<DestructibleObject>().SetSpeed(StageController.controller.GetPlayerShipSpeed());
        }
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
