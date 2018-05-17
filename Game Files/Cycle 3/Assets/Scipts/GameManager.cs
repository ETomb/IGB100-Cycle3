using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    #region Variables

    /// Insert varialbes
    /// 
    #endregion

    #region Initialise Singleton

    public static GameManager instance = null;          // Set GameManager instance variable

    private void Awake() {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Default Methods

    private void Start() {
        
    }

    private void Update() {
        
    }

    #endregion

    #region Public Methods

    /// Insert public methods

    #endregion
}