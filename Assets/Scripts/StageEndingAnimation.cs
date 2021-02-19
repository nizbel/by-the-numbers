using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StageEndingAnimation : MonoBehaviour
{
    /*
     * Constants
     */
    // Effects
    public const float BLOOM_INTENSITY = 5f;
    public const float BLOOM_THRESHOLD = 0.5f;

    public const float DEFAULT_BLOOM_INTENSITY = 0.5f;
    public const float DEFAULT_BLOOM_THRESHOLD = 0.9f;

    public const float BLOOM_STEP_SIZE = 30f;

    public const float DEFAULT_STARTING_DELAY = 1f;
    public const float DEFAULT_STAR_MOVEMENT_DELAY = 0.05f;

    // States
    private const int STOPPED = 1;
    private const int STARTING = 2;
    private const int RUNNING = 3;

    /*
     * Variables
     */
    // Bloom variables
    Bloom bloom = null;
    float bloomIntensityStep = (BLOOM_INTENSITY - DEFAULT_BLOOM_INTENSITY) / BLOOM_STEP_SIZE;
    float bloomThresholdStep = (BLOOM_THRESHOLD - DEFAULT_BLOOM_THRESHOLD) / BLOOM_STEP_SIZE;

    // Star control variables
    Star[] stars;
    List<FadingStar> fadingStars = new List<FadingStar>();
    int currentStarIndex = 0;
    // Use lists to mark possible positions for stars
    float startPosX;
    float endPosX;
    List<float> listUpperPosition = new List<float>();
    List<float> listLowerPosition = new List<float>();
    // Chance to pick star (in %, starts at max)
    float chance = 100;
    float movementDelay = DEFAULT_STAR_MOVEMENT_DELAY;

    // Constellation
    Constellation constellation = null;

    // Animation and action
    int state = STOPPED;

    float startingDelay = DEFAULT_STARTING_DELAY;

    // Start is called before the first frame update
    void Start() {
        // Get constellation if it exists
        ConstellationController constellationController = ConstellationController.controller;
        if (constellationController != null) {
            constellation = ConstellationController.controller.GetConstellation();
        }

        // Change bloom intensity
        Volume volume = GameObject.FindObjectOfType<Volume>();
        volume.sharedProfile.TryGet<Bloom>(out bloom);

        stars = GameObject.FindObjectsOfType<Star>();

        // Bring stars to front
        foreach (Star star in stars) {
            star.GetComponent<SpriteRenderer>().sortingLayerName = "Interface";
            star.GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state == RUNNING) {
            //bloom.intensity.SetValue(new NoInterpMinFloatParameter(20, 0));
            if (bloom.intensity.value < BLOOM_INTENSITY) {
                bloom.intensity.value = Mathf.Clamp(Mathf.Lerp(bloom.intensity.value, bloom.intensity.value + bloomIntensityStep, Time.deltaTime), 0, BLOOM_INTENSITY);
            }

            if (bloom.threshold.value > BLOOM_THRESHOLD) {
                bloom.threshold.value = Mathf.Clamp(Mathf.Lerp(bloom.threshold.value, bloom.threshold.value + bloomThresholdStep, Time.deltaTime), BLOOM_THRESHOLD, 1);
            }

            //Debug.Log(bloom.intensity.value + "..." + bloom.threshold.value);

            // If there are fading stars, update them
            if (GameController.controller.GetCurrentDay() != 32) {
                for (int i = fadingStars.Count - 1; i >= 0; i--) {
                    FadingStar currentFadingStar = fadingStars[i];
                    if (currentFadingStar == null) {
                        fadingStars.RemoveAt(i);
                    }
                    else {
                        currentFadingStar.UpdateFading();
                    }
                }
            }

            // Animate stars one by one
            if (currentStarIndex < stars.Length) {

                if (GameController.controller.GetCurrentDay() != 32) {
                    if (constellation != null) {
                        FadingStar fadingStar = null;
                        while (fadingStar == null && currentStarIndex < stars.Length) {
                            if (!constellation.StarInConstellation(stars[currentStarIndex])) {
                                fadingStars.Add(stars[currentStarIndex].gameObject.AddComponent<FadingStar>());
                            }
                            currentStarIndex++;
                        }
                    }
                }
                else {
                    if (movementDelay <= 0) {
                        Star star = stars[currentStarIndex];

                        float minDistanceFromCenter = Mathf.Pow(GameController.GetCameraYMin() / 2, 2);
                        bool isInsideArea = (star.transform.position - Vector3.up).sqrMagnitude <= minDistanceFromCenter;
                        // Not all stars shall be used in the formation
                        if (GameController.RollChance(chance) || isInsideArea) {
                            // Decrease chance slowly
                            chance -= 3.33f;

                            AnimateStar(star);
                        }
                        currentStarIndex += 1;

                        // Reset delay
                        movementDelay = DEFAULT_STAR_MOVEMENT_DELAY;
                    }
                    else {
                        movementDelay -= Time.deltaTime;
                    }
                }
            }

        } else if (state == STARTING) {
            startingDelay -= Time.deltaTime;
            if (startingDelay <= 0) {
                // Disable out screen destroyer controller
                OutScreenDestroyerController.controller.Stop();

                // Stop foreground and background layers
                ForegroundLayer[] foreLayers = GameObject.FindObjectsOfType<ForegroundLayer>();
                foreach (ForegroundLayer layer in foreLayers) {
                    layer.enabled = false;
                }
                BackgroundLayer[] backLayers = GameObject.FindObjectsOfType<BackgroundLayer>();
                foreach (BackgroundLayer layer in backLayers) {
                    layer.enabled = false;
                }

                // Reload stars
                stars = GameObject.FindObjectsOfType<Star>();

                // Deactivate player ship
                PlayerController.controller.enabled = false;
                
                // Finally start animation
                PrepareEnvironment();
                state = RUNNING;
            }
        }
    }

    void OnDestroy() {
        if (bloom != null) {
            bloom.intensity.value = DEFAULT_BLOOM_INTENSITY;
            bloom.threshold.value = DEFAULT_BLOOM_THRESHOLD;
        }
    }

    public void StartAnimation() {
        state = STARTING;
    }

    void PrepareEnvironment() { 
        // Use lists to mark possible positions for stars
        startPosX = GameController.GetCameraYMin() / 2 + 0.2f;
        endPosX = GameController.GetCameraYMax() / 2 - 0.2f;

        for (float i = startPosX; i <= endPosX; i += 0.05f) {
            listUpperPosition.Add(i);
            listLowerPosition.Add(i);
        }

    }

    void AnimateStar(Star star) {
        // Define colors in a binary string
        string binaryHolder = System.Convert.ToString(Random.Range(1, (int)Mathf.Pow(2, 3)), 2).PadLeft(3, '0');
        //star.GetComponent<SpriteRenderer>().color = new Color(
        //    binaryHolder[0] == '1'? 1f:0, binaryHolder[1] == '1' ? 1f : 0, binaryHolder[2] == '1' ? 1f : 0);

        // Choose position randomly
        //float x = Random.Range(startPosX, endPosX);
        float x;
        float y;
        int neighborsToRemove = 2;
        
        //if (GameController.RollChance(50)) {
        if (listLowerPosition.Count > listUpperPosition.Count) {
            if (listLowerPosition.Count > 0) {
                // Choose among currently available X positions
                int positionIndex = Random.Range(0, listLowerPosition.Count);
                x = listLowerPosition[positionIndex];

                y = -GameController.GetCameraYMax() / 2 + Mathf.Abs(x);

                // Remove X positions with up to 2 distance
                int removalStart = Mathf.Max(0, positionIndex - neighborsToRemove);
                int amountToRemove = Mathf.Min(2 * neighborsToRemove + 1, listLowerPosition.Count - removalStart);
                listLowerPosition.RemoveRange(removalStart, amountToRemove);
            } else {
                // In case there are no more positions left, send them to position 0
                x = 0;
                y = -GameController.GetCameraYMax() / 2;
            }
        }
        else {
            if (listUpperPosition.Count > 0) {
                // Choose among currently available X positions
                int positionIndex = Random.Range(0, listUpperPosition.Count);
                x = listUpperPosition[positionIndex];

                y = 0.4f * GameController.GetCameraYMax() / 2 * Mathf.Sin(
                    (GameController.GetCameraYMax() / 2 - Mathf.Abs(x)) / (GameController.GetCameraYMax() / 2) * Mathf.PI);

                // Remove X positions with up to 2 distance
                int removalStart = Mathf.Max(0, positionIndex - neighborsToRemove);
                int amountToRemove = Mathf.Min(2 * neighborsToRemove + 1, listUpperPosition.Count - removalStart);
                listUpperPosition.RemoveRange(removalStart, amountToRemove);
            }
            else {
                // In case there are no more positions left, send them to position 0
                x = 0;
                y = 0.4f * GameController.GetCameraYMax() / 2 * Mathf.Sin(
                    (GameController.GetCameraYMax() / 2) / (GameController.GetCameraYMax() / 2) * Mathf.PI);
            }
        }

        //Debug.Log("Lower: " + listLowerPosition.Count + " ... Upper: " + listUpperPosition.Count);

        AligningStar aligningScript = star.gameObject.AddComponent<AligningStar>();
        aligningScript.SetDestination(new Vector3(x, y, 0) + Vector3.up);
        //star.transform.localScale *= 1.5f;
        aligningScript.SetTargetColor(new Color(
            binaryHolder[0] == '1' ? 1f : 0, binaryHolder[1] == '1' ? 1f : 0, binaryHolder[2] == '1' ? 1f : 0));

        if (star.GetComponent<ShinyStar>() == null) {
            star.gameObject.AddComponent<ShinyStar>();
        }
    }
}
