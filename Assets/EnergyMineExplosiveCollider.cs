using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMineExplosiveCollider : MonoBehaviour
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

    void OnTriggerEnter2D(Collider2D collider) { 
        switch (collider.tag) {
            case "Block":
                mine.EnergizeOnCollision(collider.GetComponent<OperationBlock>());
                break;

            case "Mine Force Field":
            case "Player":
                break;

            default:
                mine.Explode();
                break;
        }
    }
}
