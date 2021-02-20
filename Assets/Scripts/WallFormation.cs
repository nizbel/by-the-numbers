using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFormation : Formation
{
    // Constants
    private const float SPEED_DIRECTION_TIMER = 0.3f;
    public const int MIN_AMOUNT = 3;
    public const int MAX_AMOUNT = 9;

    // Distance between elements
    private const float DEFAULT_DISTANCE = 2.25f;

    // Amounts
    private const int MIN_LOW_AMOUNT = MIN_AMOUNT;
    private const int MAX_LOW_AMOUNT = 4;
    private const int MIN_MEDIUM_AMOUNT = 5;
    private const int MAX_MEDIUM_AMOUNT = 7;
    private const int MIN_HIGH_AMOUNT = 8;
    private const int MAX_HIGH_AMOUNT = MAX_AMOUNT;

    // Types
    public const int RANDOM_ELEMENTS_TYPE = 1;
    public const int SEQUENTIAL_ELEMENTS_TYPE = 2;

    List<Transform> transforms = new List<Transform>();

    bool moving = false;

    Vector3 speed = Vector3.up;

    int type = RANDOM_ELEMENTS_TYPE;

    // Distance between elements
    float distance = DEFAULT_DISTANCE;

    float speedDirectionTimer = SPEED_DIRECTION_TIMER;

    // Start is called before the first frame update
    void Start()
    {
        // Choose size
        if (amount == 0) {
            SetRandomAmount();
        }

        GenerateWall();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving) {
            //foreach (Transform childTransform in transforms) {
            //    if (childTransform != null) {
            //        childTransform.position = Vector3.Lerp(childTransform.position, childTransform.position + speed, Time.deltaTime);
            //    }
            //}
            transform.position += speed * Time.deltaTime;


            // Check timer if it is a moving wall
            if (speedDirectionTimer <= 0) {
                speed *= -1;
                speedDirectionTimer = SPEED_DIRECTION_TIMER;
            }
            else {
                speedDirectionTimer -= Time.deltaTime;
            }
        }
    }

    void SetRandomAmount() {
        amount = Random.Range(MIN_AMOUNT, MAX_AMOUNT);
    }

    void GenerateWall() { 
        Vector3 currentPosition = transform.position;
        Vector3 currentOffset = Vector3.zero;

        // Size is dictated by center, then upper and bottom sequentially
        while (transforms.Count < amount) {
            Transform childTransform = AddElement();
            childTransform.position = currentPosition + currentOffset;
            currentPosition = childTransform.position;

            if (currentOffset.y > 0) {
                currentOffset += Vector3.up * distance;
                currentOffset *= -1;
            } else if (currentOffset.y < 0) {
                currentOffset -= Vector3.up * distance;
                currentOffset *= -1;
            } else {
                currentOffset += Vector3.up * distance;
            }
        }
    }

    Transform AddElement() {
        ElementsEnum chosenElementType;
        if (type == RANDOM_ELEMENTS_TYPE) {
            chosenElementType = elementTypes[Random.Range(0, elementTypes.Length)];
        } else if (type == SEQUENTIAL_ELEMENTS_TYPE) {
            chosenElementType = elementTypes[transforms.Count % elementTypes.Length];
        } else {
            // Fallback is random elements
            chosenElementType = elementTypes[Random.Range(0, elementTypes.Length)];
        }

        Transform newTransform = ObjectPool.SharedInstance.SpawnPooledObject(chosenElementType).transform;
        newTransform.parent = transform;
        transforms.Add(newTransform);

        return newTransform;
    }

    /*
     * Getters and Setters
     */
    public bool GetMoving() {
        return moving;
    }

    public void SetMoving(bool moving) {
        this.moving = moving;
    }
    
    // Allows to set amount directly
    public void SetAmount(int amount) {
        this.amount = amount;
    }

    public override void SetAmount(ElementsAmount amount) {
        switch (amount) {
            case ElementsAmount.Low:
                this.amount = Random.Range(MIN_LOW_AMOUNT, MAX_LOW_AMOUNT);
                break;
            case ElementsAmount.Medium:
                this.amount = Random.Range(MIN_MEDIUM_AMOUNT, MAX_MEDIUM_AMOUNT);
                break;
            case ElementsAmount.High:
                this.amount = Random.Range(MIN_HIGH_AMOUNT, MAX_HIGH_AMOUNT);
                break;
        }
    }

    public void SetType(int type) {
        this.type = type;
    }

    public void SetDistance(float distance) {
        this.distance = distance;
    }
}
