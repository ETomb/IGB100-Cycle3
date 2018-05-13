using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

namespace Characters
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        public bool isSwooping = true;
        [SerializeField] private float ambientSpeed = 4f;
        [SerializeField] private float pitchSpeed = 1f;
        [SerializeField] private float rollSpeed = 1f;
        [SerializeField] private float yawSpeed = 1f;
        [SerializeField] private float swoopSpeed = 8f;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float groundForce;
        [SerializeField] private float gravityMultiplier;
        [SerializeField] private MouseLook mouseLook;
        [SerializeField] private float stepInterval;
        [SerializeField] private AudioClip[] flightSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip landSound;           // the sound played when character touches back on ground.

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
        GameObject previous;

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
        }

 

        // Update is called once per frame
        private void Update() {
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
            float speed;
            GetInput(out speed);
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

            _collisionFlags = controller.Move(transform.forward * speed * Time.fixedDeltaTime);

            ProgressFlightCycle(speed);

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

        private void GetInput(out float speed) {
            // Read input
            float roll = Input.GetAxisRaw("Roll") * Time.deltaTime * rollSpeed;
            float pitch = Input.GetAxisRaw("Pitch") * Time.deltaTime * pitchSpeed;
            float yaw = Input.GetAxisRaw("Yaw") * Time.deltaTime * yawSpeed;

            // Set if the player is swooping or not
            if (Input.GetKey(KeyCode.Space)) {
                isSwooping = true;
            } else {
                isSwooping = false;
            }

            // set the desired speed to be walking or running
            speed = isSwooping ? swoopSpeed : ambientSpeed;
            input = new Vector3(-pitch, yaw, -roll);

            // normalize input if it exceeds 1 in combined length:
            if (input.sqrMagnitude > 1) {
                input.Normalize();
            }
        }


        private void RotateView() {
            mouseLook.LookRotation (transform, _camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit) {
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
    }
}
