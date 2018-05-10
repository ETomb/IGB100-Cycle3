using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

namespace Characters
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        public bool notEncum = true;
        [SerializeField] private float flightSpeed;
        [SerializeField] private float encumSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float groundForce;
        [SerializeField] private float gravityMultiplier;
        [SerializeField] private MouseLook mouseLook;
        [SerializeField] private float stepInterval;
        [SerializeField] private AudioClip[] footstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip landSound;           // the sound played when character touches back on ground.

        private Camera _camera;
        private float yRot;
        private Vector2 input;
        private Vector3 moveDir = Vector3.zero;
        private CharacterController controller;
        private CollisionFlags _collisionFlags;
        private bool previouslyGrounded;
        private Vector3 originalCameraPosition;
        private float stepCycle;
        private float nextStep;
        private AudioSource source;
        GameObject previous;

        // Use this for initialization
        private void Start() {
            controller = GetComponent<CharacterController>();
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
            Vector3 desiredMove = transform.forward*input.y + transform.right*input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, controller.radius, Vector3.down, out hitInfo,
                               controller.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            moveDir.x = desiredMove.x*speed;
            moveDir.z = desiredMove.z*speed;


            if (controller.isGrounded) {
                moveDir.y = -groundForce;
            } else {
                moveDir += Physics.gravity*gravityMultiplier*Time.fixedDeltaTime;
            }

            _collisionFlags = controller.Move(moveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);

            mouseLook.UpdateCursorLock();
        }

        private void ProgressStepCycle(float speed) {
            if (controller.velocity.sqrMagnitude > 0 && (input.x != 0 || input.y != 0)) {
                stepCycle += (controller.velocity.magnitude + (speed*(notEncum ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(stepCycle > nextStep)) {
                return;
            }

            nextStep = stepCycle + stepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio() {
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, footstepSounds.Length);
            source.clip = footstepSounds[n];
            source.PlayOneShot(source.clip);
            // move picked sound to index 0 so it's not picked next time
            footstepSounds[n] = footstepSounds[0];
            footstepSounds[0] = source.clip;
        }

        private void GetInput(out float speed) {
            // Read input
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            bool waswalking = notEncum;

            // set the desired speed to be walking or running
            speed = notEncum ? flightSpeed : encumSpeed;
            input = new Vector2(horizontal, vertical);

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
