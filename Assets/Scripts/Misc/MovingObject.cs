using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField]
    Vector3 speed = Vector3.zero;

    Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        if (rigidbody == null) {
            transform.position = Vector3.Lerp(transform.position, transform.position + speed, Time.deltaTime);
        } else {
            //rigidbody.MovePosition(Vector3.Lerp(transform.position, transform.position + speed, Time.deltaTime));
            rigidbody.AddForce(speed * rigidbody.mass, ForceMode2D.Force);
        }
    }

    public Vector3 Speed { get => speed; set => speed = value; }
}
