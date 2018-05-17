using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {
    #region Variables

    [SerializeField] int startingHealth = 100;                  // Amount of health the player starts with
    int currentHealth;                                          // Amount of health the player currently has
    bool damaged;                                               // True when the player gets damaged

    #endregion

    #region Getter Methods

    // Get starting health
    public int StartingHealth() {
        return startingHealth;
    }

    // Get current health
    public int CurrentHealth() {
        return currentHealth;
    }

    #endregion

    #region Default Methods

    private void Awake() {
        // Set the initial health of the player
        currentHealth = startingHealth;
    }

    private void Update() {
        // If the player has just been damaged...
        if (damaged) {
            /// Insert what happens when the player is damaged
        }
        // Otherwise...
        else {
            /// Insert what happens when the player isn't damaged
        }

        // Reset damaged flag
        damaged = false;
    }

    #endregion

    #region Other Public Methods

    public void TakeDamage (int amount) {
        // set the damaged flag
        damaged = true;

        // Reduce current health by damage amount
        currentHealth -= amount;
    }

    #endregion
}
