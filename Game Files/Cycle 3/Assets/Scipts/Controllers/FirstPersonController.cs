using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

namespace Characters
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    [RequireComponent(typeof (HealthManager))]
    public class FirstPersonController : MonoBehaviour
    {
        public bool isSwooping = false;
        public bool isStalling = false;
        public bool freeze = false;                                         // If true, all processes of this script should freeze
        [SerializeField] private float ambientSpeed = 20f;
        [SerializeField] private float swoopSpeed = 30f;
        [SerializeField] private float stallSpeed = 10f;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float groundForce;
        [SerializeField] private float gravityMultiplier;
        [SerializeField] private MouseLook mouseLook;
        [SerializeField] private float stepInterval;
        [SerializeField] private AudioClip[] flightSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip landSound;           // the sound played when character touches back on ground.
        [SerializeField] private float lerpSpeed = 0.1f;
        [SerializeField] int damageTick = 5;                // the amount of damage you suffer in an area or by colliding into an invisble wall

        private Camera _camera;
        private float yRot;
        private Vector3 input;
        private Vector3 moveDir = Vector3.zero;
        private CharacterController controller;
        private CollisionFlags _collisionFlags;
        private bool previouslyGrounded;
        private Vector3 originalCameraPosition;
        private float stepCycle;
        private float nextStep;
        private AudioSource source;
        private Rigidbody rb;
        private HealthManager playerHealth;
        private GameObject previous;
        private float currentSpeed;
        private float targetSpeed;

        // Use this for initialization
        private void Start() {
            controller = GetComponent<CharacterController>();
            rb = GetComponent<Rigidbody>();
            _camera = Camera.main;
            originalCameraPosition = _camera.transform.localPosition;
            stepCycle = 0f;
            nextStep = stepCycle/2f;
            source = GetComponent<AudioSource>();
			mouseLook.Init(transform , _camera.transform);
            playerHealth = GetComponent<HealthManager>();
            currentSpeed = ambientSpeed;
            targetSpeed = currentSpeed;
        }

 

        // Update is called once per frame
        private void Update() {
            // Freeze processes
            if (freeze)
                return;
    
            RotateView();
            if (!controller.isGrounded && previouslyGrounded)
            {
                moveDir.y = 0f;
            }

            previouslyGrounded = controller.isGrounded;
        }


        private void PlayLandingSound() {
            source.clip = landSound;
            source.Play();
            nextStep = stepCycle + .5f;
        }


        private void FixedUpdate() {
            // Freeze processes
            if (freeze)
                return;

            GetInput();
            // always move along the camera forward as it is the direction that it being aimed at
            Quaternion addRot = Quaternion.identity;
            addRot.eulerAngles = input;

            transform.localRotation *= addRot;
            _camera.transform.localRotation *= addRot;


            //Vector3 desiredMove = transform.forward*input.y + transform.right*input.x;

            //// get a normal for the surface that is being touched to move along it
            //RaycastHit hitInfo;
            //Physics.SphereCast(transform.position, controller.radius, Vector3.down, out hitInfo,
            //                   controller.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            //desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            //moveDir.x = desiredMove.x*speed;
            //moveDir.z = desiredMove.z*speed;


            //if (controller.isGrounded) {
            //    moveDir.y = -groundForce;
            //} else {
            //    moveDir += Physics.gravity*gravityMultiplier*Time.fixedDeltaTime;
            //}

            if (currentSpeed != targetSpeed) {
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, lerpSpeed);
            }

            _collisionFlags = controller.Move(transform.forward * currentSpeed * Time.fixedDeltaTime);

            ProgressFlightCycle(currentSpeed);

            mouseLook.UpdateCursorLock();
        }

        private void ProgressFlightCycle(float speed) {
            if (controller.velocity.sqrMagnitude > 0 && (input.x != 0 || input.y != 0)) {
                stepCycle += (controller.velocity.magnitude + (speed*(isSwooping ? m_RunstepLenghten : 1f)))*
                             Time.fixedDeltaTime;
            }

            if (!(stepCycle > nextStep)) {
                return;
            }

            nextStep = stepCycle + stepInterval;

            //PlayFootStepAudio();
        }


        private void PlayFootStepAudio() {
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            //int n = Random.Range(1, flightSounds.Length);
           // source.clip = flightSounds[n];
            //source.PlayOneShot(source.clip);
            // move picked sound to index 0 so it's not picked next time
            //flightSounds[n] = flightSounds[0];
            //flightSounds[0] = source.clip;
        }

        private void GetInput() {
            // Set swoop/stall state and speed
            if (Input.GetButton("Fire1")) {
                isSwooping = true;
                isStalling = false;
                targetSpeed = swoopSpeed;
            } else if (Input.GetButton("Fire2")) {
                isStalling = true;
                isSwooping = false;
                targetSpeed = stallSpeed;
            } else {
                isStalling = false;
                isSwooping = false;
                targetSpeed = ambientSpeed;
            }
        }


        private void RotateView() {
            mouseLook.LookRotation (transform, _camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit) {
            // If the object isn't an invisible wall...
            if (hit.gameObject.tag != "InvisibleWall") {
                // ... take damage equal to your current speed
                playerHealth.TakeDamage((int)currentSpeed);
            }
            // Otherwise ...
            else {
                // ... take a small amount of damage
                playerHealth.TakeDamage(damageTick);
            }
          
            // Other collision interaction information
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (_collisionFlags == CollisionFlags.Below) {
                return;
            }

            if (body == null || body.isKinematic) {
                return;
            }
            body.AddForceAtPosition(controller.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

        private void OnTriggerStay(Collider other) {
            // If the collider has the AreaDamage tag...
            if (other.tag == "AreaDamage") {
                // ... the player suffers a small amount of damage
                playerHealth.TakeDamage(damageTick);
            }
        }
    }
}
