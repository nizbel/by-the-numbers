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
                // TODO Fix this workaround, perhaps making negative value as -1 and dumping Operation Blocks
                int energy = collider.GetComponent<Energy>().GetValue();
                if (mine.GetCurrentEnergy() * energy < 0) {
                    mine.Explode();
                } else {
                    collider.attachedRigidbody.AddForce(collider.transform.position - transform.position);
                }
                break;

            case "Player":
                if (mine.GetCurrentEnergy() * PlayerController.controller.GetValue() < 0) {
                    mine.Explode();
                }
                break;
        }
    }
}
