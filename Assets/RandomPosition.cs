using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    private const float DEFAULT_WAIT = 0.4f;

    float wait = DEFAULT_WAIT;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Random.Range(GameController.GetCameraXMin(), GameController.GetCameraXMax()), 
            Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (wait > 0) {
            wait -= Time.deltaTime;
        } else {
            wait = Random.Range(0.1f, DEFAULT_WAIT);
            transform.position = new Vector3(Random.Range(GameController.GetCameraXMin(), GameController.GetCameraXMax()), 
                Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0);
            GetComponent<ParticleSystem>().Stop();
            GetComponent<ParticleSystem>().Play();
        }
    }
}
