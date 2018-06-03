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

    [SerializeField] Canvas endCanvas;                      // the canvas that shows the victory/defeat screen
    [SerializeField] Image winImage;                        // the panel shown when the player wins
    [SerializeField] Image loseImage;                    // the panel shown when the player loses
    [SerializeField] Text timeText;                         // the text that shows the player's time
    [SerializeField] Text healthText;                       // the text that shows the player's health
    [SerializeField] Text winScoreText;                     // the text that shows the player's winning score
    [SerializeField] Button playAgainButton;                // button to play again
    [SerializeField] Button menuButton;                     // button to take the player back to the menu
    [SerializeField] float screenLoadTime = 0.5f;           // time it takes for the end screens to fade in

    float endTime;
    int endHealth;
    int endMinutes;
    int endSeconds;
    int score;

    bool gameOver;



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
        CheckWinCondition();
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
            WinScenario();
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


    void WinScenario() {
        if (gameOver) {
            return;
        }
        // Set flag
        gameOver = true;
        // Freeze player
        player.GetComponent<Characters.FirstPersonController>().freeze = true;
        // Make player invulnerable
        player.GetComponent<HealthManager>().MakeInvulnerable();
        // Stop fire
        fire.GetComponentInParent<FireBehaviour>().SetMoveState(false);

        // Get time
        endTime = Time.timeSinceLevelLoad;
        endMinutes = Mathf.FloorToInt(endTime / 60F);
        endSeconds = Mathf.FloorToInt(endTime - endMinutes * 60);
        timeText.text = string.Format("Time: {0:0}:{1:00}", endMinutes, endSeconds);
        // Get health
        endHealth =player.GetComponent<HealthManager>().CurrentHealth();
        healthText.text = string.Format("Health: {0:000}%", endHealth);
        // Calculate points
        int timeScore = (int)((endTime - 60));
        if (timeScore < 0) {
            timeScore = 0;
        } else if (timeScore > 111) {
            timeScore = 111;
        }
        score = (int)(90 * (endHealth + 30 - timeScore));
        if (score < 0) {
            score = 0;
        } else if (score > 9999) {
            score = 9999;
        }
        winScoreText.text = string.Format("Your Score: {0:0000}", score);
        // Activate canvas
        endCanvas.gameObject.SetActive(true);
        // Set panel to clear
        winImage.color = Color.clear;
        // Activate the panel
        winImage.gameObject.SetActive(true);
        // Fade screen in
        StartCoroutine("WinAnimation");
    }

    public void LoseScenario() {
        if (gameOver) {
            return;
        }
        // Set flag
        gameOver = true;
        // Freeze player
        player.GetComponent<Characters.FirstPersonController>().freeze = true;
        // Make player invulnerable
        player.GetComponent<HealthManager>().MakeInvulnerable();
        // Stop fire
        fire.GetComponentInParent<FireBehaviour>().SetMoveState(false);
        // Activate canvas
        endCanvas.gameObject.SetActive(true);
        // Set panel to clear
        loseImage.color = Color.clear;
        // Activate the panel
        loseImage.gameObject.SetActive(true);
        // Fade screen in
        StartCoroutine("LoseAnimation");
    }

    #endregion

    #region Cooroutines

    IEnumerator WinAnimation() {
        // Fade panel in
        while (winImage.color != Color.white) {
            winImage.color = Color.Lerp(winImage.color, Color.white, screenLoadTime);
        }
        yield return new WaitUntil(() => winImage.color == Color.white);
        // Activate buttons and text
        foreach (Transform child in winImage.gameObject.transform) {
            child.gameObject.SetActive(true);
        }
        playAgainButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StopCoroutine("WinAnimation");
    }

    IEnumerator LoseAnimation() {
        // Fade panel in
        while (loseImage.color != Color.black)
        {
            loseImage.color = Color.Lerp(loseImage.color, Color.black, screenLoadTime);
        }
        yield return new WaitUntil(() => loseImage.color == Color.black);
        // Activate buttons and text
        foreach (Transform child in loseImage.gameObject.transform)
        {
            child.gameObject.SetActive(true);
        }
        playAgainButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StopCoroutine("LoseAnimation");
    }

    #endregion
}
