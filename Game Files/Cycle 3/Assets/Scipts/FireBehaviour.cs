using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehaviour : MonoBehaviour {
    #region Variables

    [SerializeField] float fireSpeed = 1f;              // the speed the fire moves at
    bool doMove = true;                                 // true if the fire should move, otherwise false

    #endregion

    #region Default Methods

    void Update() {
        // Check the move state of the fire...
        if (doMove) {
            // ... and make it move if it's meant to
            Move();
        }
    }

    #endregion

    #region Helper Methods

    private void Move() {
        // Translate the fire by its speed
        gameObject.transform.Translate(Vector3.forward * fireSpeed * Time.deltaTime);
    }

    #endregion

    #region Public Methods

    // Set the move state of the fire by setting the flag
    public void SetMoveState(bool shouldMove) {
        doMove = shouldMove;
    }

    // Returns the set move state of the fire
    public bool GetMoveState() {
        return doMove;
    }

    #endregion
}
