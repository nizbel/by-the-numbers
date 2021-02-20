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
    private const float DEFAULT_DISTANCE = 2f;

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
    // Changes elements every 2 elements
    public const int DOUBLE_SEQUENTIAL_ELEMENTS_TYPE = 3;

    public enum ElementsDistanceType {
        FixedDistance,
        RandomDistance,
        FixedOffset,
        RandomOffset
    }

    List<Transform> transforms = new List<Transform>();

    bool moving = false;

    Vector3 speed = Vector3.up;

    int type = RANDOM_ELEMENTS_TYPE;

    // Distance between elements
    float distance = DEFAULT_DISTANCE;
    ElementsDistanceType distanceType = ElementsDistanceType.FixedDistance;
    // Minimum and maximum distances in case of random distance type
    float minDistance = DEFAULT_DISTANCE;
    float maxDistance = DEFAULT_DISTANCE;

    float speedDirectionTimer = SPEED_DIRECTION_TIMER;

    // Start is called before the first frame update
    void Start()
    {
        // Choose size
        if (amount == 0) {
            SetRandomAmount();
        }

        // Define element types as current moment's default if not set
        if (elementTypes == null) {
            elementTypes = StageController.controller.GetCurrentMomentAvailableElements();
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
            // Define position in the wall
            if (distanceType != ElementsDistanceType.FixedOffset && distanceType != ElementsDistanceType.RandomOffset) {
                childTransform.position = currentPosition + currentOffset;
            } else {
                // If offset distance is chosen, elements size must be taken into account
                childTransform.position = currentPosition + currentOffset + 
                    Mathf.Sign(currentOffset.y) * Vector3.up * GameObjectUtil.GetBiggestSideOfSpriteByGameObject(childTransform.gameObject) / 2;
            }
            currentPosition = childTransform.position;

            currentOffset = DefineCurrentOffset(currentOffset, currentPosition);
        }
    }

    private Vector3 DefineCurrentOffset(Vector3 currentOffset, Vector3 currentPosition) {
        switch (distanceType) {
            case ElementsDistanceType.FixedDistance:
                currentOffset = DefineOffsetByDistance(currentOffset);
                break;

            case ElementsDistanceType.RandomDistance:
                // Randomizes distance before setting current offset
                distance = Random.Range(minDistance,maxDistance);
                currentOffset = DefineOffsetByDistance(currentOffset);
                break;

            case ElementsDistanceType.FixedOffset:
                DefineOffsetByElementsSize(out currentOffset, currentPosition);
                break;

            case ElementsDistanceType.RandomOffset:
                // Randomizes distance before setting current offset
                distance = Random.Range(minDistance, maxDistance);

                DefineOffsetByElementsSize(out currentOffset, currentPosition);
                break;
        }
            return currentOffset;
    }

    private Vector3 DefineOffsetByDistance(Vector3 currentOffset) {
        if (currentOffset.y > 0) {
            currentOffset += Vector3.up * distance;
            currentOffset *= -1;
        }
        else if (currentOffset.y < 0) {
            currentOffset -= Vector3.up * distance;
            currentOffset *= -1;
        }
        else {
            currentOffset += Vector3.up * distance;
        }

        return currentOffset;
    }

    // Get the next to last element in order to make the next position
    // (Ex.: current one is above, so last one was below and next to last was also above)
    private void DefineOffsetByElementsSize(out Vector3 currentOffset, Vector3 currentPosition) {
        int elementIndex;
        if (transforms.Count >= 2) {
            elementIndex = transforms.Count - 2;
        }
        else {
            elementIndex = 0;
        }
        currentOffset = transforms[elementIndex].position - currentPosition;
        currentOffset += Mathf.Sign(currentOffset.y) * Vector3.up * (GameObjectUtil.GetBiggestSideOfSpriteByGameObject(transforms[elementIndex].gameObject) / 2 + distance);
    }

    Transform AddElement() {
        ElementsEnum chosenElementType = DefineElement(out chosenElementType);

        Transform newTransform = ObjectPool.SharedInstance.SpawnPooledObject(chosenElementType).transform;
        newTransform.parent = transform;
        transforms.Add(newTransform);

        return newTransform;
    }

    ElementsEnum DefineElement(out ElementsEnum chosenElementType) {
        // If there's only one element, type is irrelevant
        if (elementTypes.Length == 1) {
            chosenElementType = elementTypes[0];
        } else {
            switch (type) {
                case RANDOM_ELEMENTS_TYPE:
                    chosenElementType = elementTypes[Random.Range(0, elementTypes.Length)];
                    break;

                case SEQUENTIAL_ELEMENTS_TYPE:
                    chosenElementType = elementTypes[transforms.Count % elementTypes.Length];
                    break;

                case DOUBLE_SEQUENTIAL_ELEMENTS_TYPE:
                    int index = (transforms.Count + 1) / 2;
                    chosenElementType = elementTypes[index % elementTypes.Length];
                    break;

                default:
                    // Fallback is random elements
                    chosenElementType = elementTypes[Random.Range(0, elementTypes.Length)];
                    break;
            }
        }
        return chosenElementType;
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

    public void SetSpeed(Vector3 speed) {
        this.speed = speed;
    }

    public void SetSpeedDirectionTimer(float speedDirectionTimer) {
        this.speedDirectionTimer = speedDirectionTimer;
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

    public void SetDistanceType(ElementsDistanceType distanceType) {
        this.distanceType = distanceType;
    }

    public void SetMinDistance(float minDistance) {
        this.minDistance = minDistance;
    }

    public void SetMaxDistance(float maxDistance) {
        this.maxDistance = maxDistance;
    }
}
