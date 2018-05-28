using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalManager : MonoBehaviour {

    #region Variables

    [SerializeField] float endZ;                            // ending z-coordinate of the level
    [SerializeField] float startZ;                          // starting z-coordinate of the level
    float playerZ;                                          // z-position of the player
    float fireZ;                                            // z-position of the fire
    [SerializeField] Slider playerSlider;                   // the slider that displays the player's position
    [SerializeField] Slider fireSlider;                     // the slider the displays the fire's position
    bool hasSliders = false;                                // returns true if both slider variables have been assigned   
    [SerializeField] GameObject player;                     // the player
    [SerializeField] GameObject fire;                       // the fire they are escaping from

    #endregion

    #region Default Methods

    void Awake() {
        // If both slider variables have been assigned...
        if (playerSlider != null && fireSlider != null) {
            // ... set flag accordingly
            hasSliders = true;
        }
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
        UpdateSliders();

        if(playerSlider.value <= fireSlider.value)
        {
            player.GetComponent<HealthManager>().FireDamage();
        }
	}

    #endregion

    #region Helper Methods

    void UpdateSliders() {
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

    #endregion
}
