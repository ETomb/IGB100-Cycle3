using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
    #region Variables

    [SerializeField] int startingHealth = 100;                              // Amount of health the player starts with
    int currentHealth;                                                      // Amount of health the player currently has
    bool damaged;                                                           // True when the player gets damaged
    [SerializeField] float invulnerabilityTime = 0.5f;                      // Amount of time the player will be invulnerable for after taking significant damage
    bool invulnerable = false;                                              // Returns true if the player is invulnerable
    bool isDead = false;                                                    // Returns true if the player is dead 
    [SerializeField] Color flashColor = new Color(0f, 0f, 0f, 0.1f);        // Colour the colorImage is set to, to flash
    [SerializeField] Image damageImage;                                     // Reference to an image to flash on screen upon being hurt
    [SerializeField] float flashSpeed = 5f;                                 // The speed the damage image will fade at
    [SerializeField] float flashDelay = 1;                                  // Delay before fading out the flashColor of the damageImage
    bool fadeOut = false;                                                   // If true, the flashColor of the damageImage can fade out
    [SerializeField] float deathFade = 1;                                   // Speed at which the screen fades to black upon player death
    [SerializeField] float gameEndHold = 3;                                 // Number of seconds the player is held for after dying until the game over sequence starts up
    Characters.FirstPersonController player;                                // The controller script of the player


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

    private void Start() {
        // Set the location of the player controller script
        player = GetComponent<Characters.FirstPersonController>();
    }

    private void Update() {
        // If the player has just been damaged...
        if (damaged) {
            // ... set the colour of damageImage to the flash colour
            StartCoroutine(FlashImage());
        }
        // Otherwise...
        else {
            // ... transition the colour back to clear
            if (fadeOut)
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed);
        }

        // Reset damaged flag
        damaged = false;

        // If the player is dead...
        if (isDead && damageImage.color != Color.black) {
            damageImage.color = Color.Lerp(damageImage.color, Color.black, deathFade);
        }
    }

    #endregion

    #region Other Public Methods

    public void TakeDamage (int amount) {
        if (!invulnerable) {
            // set the damaged flag
            damaged = true;
            // Reduce current health by damage amount
            currentHealth -= amount;
            // Make the player invulnerable
            StartCoroutine(Invulnerability());
            // If the player has lost all their health and the death flag hasn't been set yet...
            if (currentHealth <= 0 && !isDead) {
                // ... it should die;
                Death();
            }
        }
    }

    private void Death() {
        // Set the death flag so this function won't be called again
        isDead = true;
        // Freeze all player processes
        player.freeze = true;
    }

    #endregion

    #region Coroutines

    IEnumerator Invulnerability() {
        invulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        invulnerable = false;
    }

    IEnumerator FlashImage() {
        fadeOut = false;
        damageImage.color = flashColor;
        yield return new WaitForSeconds(flashDelay);
        fadeOut = true;
    }

    IEnumerator DeathSequence() {
        // Wait until the screen has faded to black
        yield return new WaitUntil(() => damageImage.color == Color.black);
        // Then wait a few seconds
        yield return new WaitForSeconds(gameEndHold);
        /// GameOver
        
    }

    #endregion
}
