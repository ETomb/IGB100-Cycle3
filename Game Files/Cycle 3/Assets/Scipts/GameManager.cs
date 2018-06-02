using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    #region Variables

    [SerializeField] float endZ;                            // ending z-coordinate of the level
    [SerializeField] float startZ;                          // starting z-coordinate of the level
    [SerializeField] float fadeZ;                           // z-coordinate of where to begin fading out the fog effect
    bool doFadeOut;                                         // returns true if the fog & audio is to be faded out
    float playerZ;                                          // z-position of the player
    float fireZ;                                            // z-position of the fire
    [SerializeField] Slider playerSlider;                   // the slider that displays the player's position
    [SerializeField] Slider fireSlider;                     // the slider the displays the fire's position
    bool hasSliders = false;                                // returns true if both slider variables have been assigned  
    float currentDensity;                                   // current density level of the fog
    [SerializeField] float lerpTime = 1f;                   // the amount of time the fog density should be lerped by   
    [SerializeField] GameObject player;                     // the player
    [SerializeField] GameObject fire;                       // the fire they are escaping from
    AudioSource playerSource;                               // the audio source attached to the player 
    float sourceVolume;                                     // the volume of the audio source

    #endregion

    #region Default Methods

    void Awake() {
        // If both slider variables have been assigned...
        if (playerSlider != null && fireSlider != null) {
            // ... set flag accordingly
            hasSliders = true;
        }
        // Assign the player source...
        playerSource = player.GetComponent<AudioSource>();
        // ... and the volume
        sourceVolume = playerSource.volume;
    }

    void Start () {
        // check if the sliders were assigned...
        if (hasSliders) {
            // ... and set their minimum and maximum values
            playerSlider.maxValue = endZ;
            playerSlider.minValue = startZ;
            fireSlider.maxValue = endZ;
            fireSlider.minValue = startZ;
            // Reset the starting positions
            playerSlider.value = startZ;
            fireSlider.value = startZ;
            // Disable the fireSlider
            fireSlider.enabled = false;
        }
	}
	
	void Update () {
        CheckPositions();
        DoFadeFog();
        if (doFadeOut) {
            FadeFog();
            FadeAudio();
        }
	}

    #endregion

    #region Helper Methods

    void CheckPositions() {
        // Get the player's z position and set the corresponding variable
        playerZ = player.transform.position.z;
        // Set the value of the slider to match
        playerSlider.value = playerZ;
        // Get the fire's z position and set the corresponding variable
        fireZ = fire.transform.position.z;
        // If this value is at or past the starting z...
        if (fireZ >= startZ) {
            // ... and the fire slider is not enabled...
            if (!fireSlider.enabled) {
                // ... enable the slider and set the value
                fireSlider.enabled = true;
                fireSlider.value = fireZ;
            }
            // Otherwise...
            else {
                // ... set the value of the slider
                fireSlider.value = fireZ;
            }
        }
    }

    void CheckWinCondition() {
        // If the player is past the end of the level...
        if (playerZ >= endZ) {
            // ... they win!
            /// Insert win game over
        }
    }

    void DoFadeFog() {
        // If the flag is already true...
        if (doFadeOut) {
            // Exit the method
            return;
        }
        // If the player is past the point where fog should start fading...
        if (playerZ >= fadeZ) {
            // ... set the flag to true
            doFadeOut = true;
        }
        
    }

    void FadeFog() {
        // If fog is disabled...
        if (RenderSettings.fog == false) {
            // Exit the method
            return;
        }
        // Otherwise,
        // Store the value of the current fog density
        currentDensity = RenderSettings.fogDensity;
        // Lerp the density toward 0
        currentDensity = Mathf.Lerp(currentDensity, 0f, lerpTime);
        // Set the fog density to this new value
        RenderSettings.fogDensity = currentDensity;
        // If the deinsity is 0...
        if (currentDensity == 0f) {
            // ... disable fog
            RenderSettings.fog = false;
        }
    }

    void FadeAudio() {
        // If player source is unassigned...
        if (playerSource == null) {
            // ... exit the method
            return;
        }
        // Otherwise,
        // Store the value f the current volume
        sourceVolume = playerSource.volume;
        // Lerp the volume toward 0
        sourceVolume = Mathf.Lerp(sourceVolume, 0f, lerpTime);
        // Set the volume to this new value
        playerSource.volume = sourceVolume;
        // If the deinsity is 0...
    }

    #endregion
}
