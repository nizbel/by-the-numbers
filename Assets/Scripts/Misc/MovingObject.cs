using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField]
    Vector3 speed = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        transform.position = Vector3.Lerp(transform.position, transform.position + Speed, Time.deltaTime);
    }

    public Vector3 Speed { get => speed; set => speed = value; }
}
