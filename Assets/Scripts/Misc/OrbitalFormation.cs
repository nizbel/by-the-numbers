using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalFormation : Formation
{
    // Amounts
    private const int LOW_ORBIT_AMOUNT = 1;
    private const int MEDIUM_ORBIT_AMOUNT = 2;
    private const int HIGH_ORBIT_AMOUNT = 3;

    // Speeds
    private const float MIN_ORBIT_SPEED = 90;
    private const float MAX_ORBIT_SPEED = 180;
    private const float MIN_ROTATING_SPEED = 30;
    private const float MAX_ROTATING_SPEED = 180;

    enum OrbitPathEnum {
        Circular,
        Ellipse
    }

    public enum OrbitFormationSpeedsEnum {
        Random,
        SameDirection,
        SameSpeed,
        SameDirectionSameSpeed
    }

    [System.Serializable]
    class OrbitElement {
        public Transform transform;
        public float speed;
        public float radius;
        public float angularPosition;
        // TODO Add circle and ellipse orbits

        public OrbitElement(Transform transform, float speed, float radius, float angularPosition) {
            this.transform = transform;
            this.speed = speed;
            this.radius = radius;
            this.angularPosition = angularPosition;
        }
    }

    OrbitFormationSpeedsEnum speedsType;

    [SerializeField]
    List<OrbitElement> orbitElements = new List<OrbitElement>();

    // Start is called before the first frame update
    void Start() {
        GenerateElements();
        Debug.Break();
    }

    private void GenerateElements() {
        // Select element type at random to be the center element
        GameObject centerElement = ObjectPool.SharedInstance.SpawnPooledObject(elementTypes[Random.Range(0, elementTypes.Length)],
            transform.position,
            GameObjectUtil.GenerateRandomRotation());
        centerElement.transform.parent = transform;
        this.centerElement = centerElement.transform;

        // Select elements at orbits
        for (int i = 0; i < this.amount; i++) {
            // TODO Add ellipse orbits

            // Define radius and angular initial position
            float radius = (i + 1) * Random.Range(1.2f,  1.7f);
            float angularPosition = Random.Range(0, 360f);
            Vector3 positionRelativeToCenter = new Vector3(Mathf.Cos(angularPosition * Mathf.Deg2Rad), 
                Mathf.Sin(angularPosition * Mathf.Deg2Rad), 0) * radius;

            // Generate element
            GameObject orbitElement = ObjectPool.SharedInstance.SpawnPooledObject(elementTypes[Random.Range(0, elementTypes.Length)],
                positionRelativeToCenter + centerElement.transform.position,
                GameObjectUtil.GenerateRandomRotation());
            orbitElement.transform.parent = transform;
            // Fix z position
            orbitElement.transform.position += Vector3.forward * -orbitElement.transform.position.z;

            // Add rotation script
            RotatingObject rotatingScript = orbitElement.AddComponent<RotatingObject>();
            rotatingScript.SetMinSpeed(MIN_ROTATING_SPEED);
            rotatingScript.SetMaxSpeed(MAX_ROTATING_SPEED);

            AddOrbitElement(orbitElement.transform, DefineSpeed(), radius, angularPosition);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Make orbital movement for every orbit element
        for (int i = orbitElements.Count-1; i >= 0; i--) {
            OrbitElement currentOrbitElement = orbitElements[i];

            if (currentOrbitElement.transform == null || currentOrbitElement.transform.gameObject.activeSelf == false) {
                orbitElements.RemoveAt(i);
            } else {
                currentOrbitElement.angularPosition = (currentOrbitElement.angularPosition + currentOrbitElement.speed * Time.fixedDeltaTime) % 360f;
                currentOrbitElement.transform.localPosition = new Vector3(
                    Mathf.Cos(currentOrbitElement.angularPosition * Mathf.Deg2Rad), 
                    Mathf.Sin(currentOrbitElement.angularPosition * Mathf.Deg2Rad), 
                    0) * currentOrbitElement.radius;
            }
        }
    }

    public void AddOrbitElement(Transform transform, float speed, float radius, float angularPosition) {
        orbitElements.Add(new OrbitElement(transform, speed, radius, angularPosition));
    }

    float DefineSpeed() {
        if (speedsType == OrbitFormationSpeedsEnum.Random || orbitElements.Count == 0) {
            return Random.Range(MIN_ORBIT_SPEED, MAX_ORBIT_SPEED) * (GameController.RollChance(50) ? 1:-1);
        } else {
            // Follow the first orbit element
            OrbitElement firstElement = orbitElements[0];
            switch (speedsType) {
                case OrbitFormationSpeedsEnum.SameDirection:
                    return Mathf.Sign(firstElement.speed) * Random.Range(MIN_ORBIT_SPEED, MAX_ORBIT_SPEED);
                case OrbitFormationSpeedsEnum.SameSpeed:
                    return firstElement.speed * (GameController.RollChance(50) ? 1 : -1);
                case OrbitFormationSpeedsEnum.SameDirectionSameSpeed:
                    return firstElement.speed;
                default:
                    return 0;
            }
        }
    }

    public override void SetAmount(ElementsAmount amount) {
        switch (amount) {
            case ElementsAmount.Low:
                this.amount = LOW_ORBIT_AMOUNT;
                break;
            case ElementsAmount.Medium:
                this.amount = MEDIUM_ORBIT_AMOUNT;
                break;
            case ElementsAmount.High:
                this.amount = HIGH_ORBIT_AMOUNT;
                break;
        }
    }
}
