using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMineForceFieldCollider : MonoBehaviour 
{
    [SerializeField]
    EnergyMine mine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D collider) {
        switch (collider.tag) {
            case "Block":
                Debug.Log("Entered trigger " + collider.gameObject.name);
                if (mine.GetCurrentEnergy() * collider.GetComponent<OperationBlock>().GetValue() < 0) {
                    mine.Explode();
                } else {
                    collider.attachedRigidbody.AddForce(collider.transform.position - transform.position, ForceMode2D.Impulse);
                }
                break;

            case "Player":
                Debug.Log("Entered trigger " + collider.gameObject.name);
                if (mine.GetCurrentEnergy() * PlayerController.controller.GetValue() < 0) {
                    mine.Explode();
                }
                break;
        }
    }
}
