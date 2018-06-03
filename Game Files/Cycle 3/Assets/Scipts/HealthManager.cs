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
    [SerializeField] Color fireFlashColor = new Color(0f, 0f, 0f, 0.1f);    // Colour the colorImage is set to, to flash, when taking fire damage
    [SerializeField] Image damageImage;                                     // Reference to an image to flash on screen upon being hurt
    [SerializeField] float flashSpeed = 5f;                                 // The speed the damage image will fade at
    [SerializeField] float flashDelay = 1;                                  // Delay before fading out the flashColor of the damageImage
    bool fadeOut = false;                                                   // If true, the flashColor of the damageImage can fade out
    [SerializeField] float deathFade = 1;                                   // Speed at which the screen fades to black upon player death
    [SerializeField] float gameEndHold = 3;                                 // Number of seconds the player is held for after dying until the game over sequence starts up
    Characters.FirstPersonController player;                                // The controller script of the player
    [SerializeField] Slider healthSlider;                                   // The player's health bar
    bool hasHealthBar = false;                                              // Returns true if the player has been assigned a health bar
    bool tookFireDamage = false;                                            // Returns true if the player took fire damage

    float FireImTime = 0.1f;
    bool fireImmune = false;

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
        // Check if the player has a health bar...
        if (healthSlider != null) {
            // ... set  the flag accordingly...
            hasHealthBar = true;
            // ... and set the maximum value of the slider component...
            healthSlider.maxValue = startingHealth;
            // ... then reset the current value
            healthSlider.value = healthSlider.maxValue;
        }
    }

    private void Start() {
        // Set the location of the player controller script
        player = GetComponent<Characters.FirstPersonController>();
    }

    private void Update() {
        // If the player has just been damaged...
        if (damaged && !isDead) {
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

        // Update the health bar
        if (hasHealthBar)
            healthSlider.value = currentHealth;

        // If the player is dead...
        if (isDead && damageImage.color != Color.black) {
            damageImage.color = Color.Lerp(damageImage.color, Color.black, deathFade);
        }
    }

    #endregion

    #region Other Public Methods

    public void TakeDamage (int amount, bool fireDamage = false) {
        if (!invulnerable) {
            // set the damaged flag
            damaged = true;
            // set the fir damage tage
            tookFireDamage = fireDamage;
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
        // Lose the game
        FindObjectOfType<GameManager>().LoseScenario();
    }

    public void MakeInvulnerable() {
        invulnerable = true;
    }

    #endregion

    #region Coroutines

    IEnumerator Invulnerability() {
        invulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        invulnerable = false;
        StopCoroutine("Invulnerability");
    }

    IEnumerator FlashImage() {
        fadeOut = false;
        if (tookFireDamage) {
            damageImage.color = fireFlashColor;
        } else {
            damageImage.color = flashColor;
        }
        yield return new WaitForSeconds(flashDelay);
        fadeOut = true;
        StopCoroutine("FlashImage");
    }

    #endregion
}
