using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Unity.FPS.Gameplay
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(AudioSource))]
    public class PlayerCharacterController : MonoBehaviour
    {
        [Header("References")] [Tooltip("Reference to the main camera used for the player")]
        public Camera PlayerCamera;

        [Tooltip("Audio source for footsteps, jump, etc...")]
        public AudioSource AudioSource;

        [Header("General")] [Tooltip("Force applied downward when in the air")]
        public float GravityDownForce = 20f;

        [Tooltip("Physic layers checked to consider the player grounded")]
        public LayerMask GroundCheckLayers = -1;

        [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
        public float GroundCheckDistance = 0.05f;

        [Header("Movement")] [Tooltip("Max movement speed when grounded (when not sprinting)")]
        public float BaseMaxSpeedOnGround = 10f;
        public float SpeedOnGround;
        public float MaxSpeedOnGround;
        public float SpeedMult = 1.0f;

        [Tooltip(
            "Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
        public float MovementSharpnessOnGround = 10;
        public float BaseMovementSharpnessOnGround;

        /*[Tooltip("Max movement speed when crouching")] [Range(0, 1)]
        public float MaxSpeedCrouchedRatio = 0.5f;*/

        [Tooltip("Max movement speed when not grounded")]
        public float MaxSpeedInAir;

        [Tooltip("Acceleration speed when in the air")]
        public float AccelerationSpeedInAir = 25f;

        /*[Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
        public float SprintSpeedModifier = 2f;*/

        [Tooltip("Height at which the player dies instantly when falling off the map")]
        public float KillHeight = -50f;

        [Header("Dashing")]
        public Transform orientation;
        private Rigidbody rb;
        public float dashForce;
        public float dashUpwardForce;
        public float dashDuration;
        public float dashCD;
        public float dashCDTimer;
        public float fastDash = 1f;
        public bool isDashing = false;
        public KeyCode dashKey = KeyCode.LeftShift;

        [Header("Rotation")] [Tooltip("Rotation speed for moving the camera")]
        public float RotationSpeed = 200f;

        [Range(0.1f, 1f)] [Tooltip("Rotation speed multiplier when aiming")]
        public float AimingRotationMultiplier = 0.4f;

        [Header("Jump")] [Tooltip("Force applied upward when jumping")]
        public float JumpForce = 9f;

        [Header("Stance")] [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
        public float CameraHeightRatio = 0.9f;

        [Tooltip("Height of character when standing")]
        public float CapsuleHeightStanding = 1.8f;

        /*[Tooltip("Height of character when crouching")]
        public float CapsuleHeightCrouching = 0.9f;*/

        [Tooltip("Speed of crouching transitions")]
        public float CrouchingSharpness = 10f;

        [Header("Audio")] [Tooltip("Amount of footstep sounds played when moving one meter")]
        public float FootstepSfxFrequency = 1f;

        /*[Tooltip("Amount of footstep sounds played when moving one meter while sprinting")]
        public float FootstepSfxFrequencyWhileSprinting = 1f;*/

        [Tooltip("Sound played for footsteps")]
        public AudioClip FootstepSfx;

        [Tooltip("Sound played when jumping")] public AudioClip JumpSfx;
        [Tooltip("Sound played when landing")] public AudioClip LandSfx;

        [Tooltip("Sound played when taking damage froma fall")]
        public AudioClip FallDamageSfx;
        public UnityAction<bool> OnStanceChanged;

        public Vector3 CharacterVelocity { get; set; }
        public bool IsGrounded { get; private set; }
        public bool HasJumpedThisFrame { get; private set; }
        public bool IsDead { get; private set; }
        //public bool IsCrouching { get; private set; }

        public float RotationMultiplier
        {
            get
            {
                /*if (m_WeaponsManager.IsAiming)
                {
                    return AimingRotationMultiplier;
                }*/

                return 1f;
            }
        }

        Health m_Health;
        Money m_Money;
        PlayerInputHandler m_InputHandler;
        CharacterController m_Controller;
        PlayerWeaponsManager m_WeaponsManager;
        Actor m_Actor;
        Vector3 m_GroundNormal;
        Vector3 m_CharacterVelocity;
        Vector3 m_LatestImpactSpeed;
        float m_LastTimeJumped = 0f;
        float m_CameraVerticalAngle = 0f;
        float m_FootstepDistanceCounter;
        float m_TargetCharacterHeight;

        const float k_JumpGroundingPreventionTime = 0.2f;
        const float k_GroundCheckDistanceInAir = 0.07f;

        public Vector3 LastCharacterVelocity;
        public bool CantStopMoving = false;

        void Awake()
        {
            ActorsManager actorsManager = FindObjectOfType<ActorsManager>();
            if (actorsManager != null)
                actorsManager.SetPlayer(gameObject);
        }

        void Start()
        {
            BaseMovementSharpnessOnGround = MovementSharpnessOnGround;
            SpeedOnGround = BaseMaxSpeedOnGround;
            MaxSpeedOnGround = SpeedOnGround * SpeedMult;
            MaxSpeedInAir = MaxSpeedOnGround;
            // fetch components on the same gameObject
            m_Controller = GetComponent<CharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<CharacterController, PlayerCharacterController>(m_Controller,
                this, gameObject);

            m_InputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerCharacterController>(m_InputHandler,
                this, gameObject);

            m_WeaponsManager = GetComponent<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerWeaponsManager, PlayerCharacterController>(
                m_WeaponsManager, this, gameObject);

            m_Health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerCharacterController>(m_Health, this, gameObject);

            m_Money = GetComponent<Money>();
            DebugUtility.HandleErrorIfNullGetComponent<Money, PlayerCharacterController>(m_Money, this, gameObject);

            m_Actor = GetComponent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Actor, PlayerCharacterController>(m_Actor, this, gameObject);

            m_Controller.enableOverlapRecovery = true;

            m_Health.OnDie += OnDie;

            // force the crouch state to false when starting
            SetCrouchingState(false, true);
            UpdateCharacterHeight(true);

            /*rb = GetComponent<Rigidbody>();
            DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, PlayerCharacterController>(rb, this, gameObject);*/
        }

        void Update()
        {
            if(Input.GetKeyDown(dashKey)){
                Dash();
            }

            if(dashCDTimer > 0){
                dashCDTimer -= Time.deltaTime*fastDash;
            }
            //if(CharacterVelocity != null) Debug.Log(CharacterVelocity);
            //if(CharacterVelocity != Vector3.zero) Debug.Log(CharacterVelocity.magnitude);
            MaxSpeedOnGround = SpeedOnGround * SpeedMult;
            MaxSpeedInAir = MaxSpeedOnGround;
            // check for Y kill
            if (!IsDead && transform.position.y < KillHeight)
            {
                m_Health.Kill();
            }

            HasJumpedThisFrame = false;

            bool wasGrounded = IsGrounded;
            GroundCheck();

            // landing
            if (IsGrounded && !wasGrounded)
            {
                
                // land SFX
                AudioSource.PlayOneShot(LandSfx);
            }

            // crouching
            /*if (m_InputHandler.GetCrouchInputDown())
            {
                SetCrouchingState(!IsCrouching, false);
            }*/

            UpdateCharacterHeight(false);

            HandleCharacterMovement();

            /*if(LastCharacterVelocity != Vector3.zero) {
                Debug.Log(LastCharacterVelocity.magnitude);
                Debug.Log(LastCharacterVelocity.normalized*SpeedOnGround);
                Debug.Log((LastCharacterVelocity.normalized*SpeedOnGround).magnitude);
            }*/
        }

        void OnDie()
        {
            IsDead = true;

            // Tell the weapons manager to switch to a non-existing weapon in order to lower the weapon
            m_WeaponsManager.SwitchToWeaponIndex(-1, true);

            EventManager.Broadcast(Events.PlayerDeathEvent);
        }

        void GroundCheck()
        {
            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            float chosenGroundCheckDistance =
                IsGrounded ? (m_Controller.skinWidth + GroundCheckDistance) : k_GroundCheckDistanceInAir;

            // reset values before the ground check
            IsGrounded = false;
            m_GroundNormal = Vector3.up;

            // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
            if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
            {
                // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height),
                    m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    // storing the upward direction for the surface found
                    m_GroundNormal = hit.normal;

                    // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                    // and if the slope angle is lower than the character controller's limit
                    if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                        IsNormalUnderSlopeLimit(m_GroundNormal))
                    {
                        IsGrounded = true;

                        // handle snapping to the ground
                        if (hit.distance > m_Controller.skinWidth)
                        {
                            m_Controller.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }

        void HandleCharacterMovement()
        {
            // horizontal character rotation
            {
                // rotate the transform with the input speed around its local Y axis
                transform.Rotate(
                    new Vector3(0f, (m_InputHandler.GetLookInputsHorizontal() * RotationSpeed * RotationMultiplier),
                        0f), Space.Self);
            }

            // vertical camera rotation
            {
                // add vertical inputs to the camera's vertical angle
                m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical() * RotationSpeed * RotationMultiplier;

                // limit the camera's vertical angle to min/max
                m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

                // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
                PlayerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
            }

            // character movement handling
            //bool isSprinting = m_InputHandler.GetSprintInputHeld();
            {
                /*if (isSprinting)
                {
                    isSprinting = SetCrouchingState(false, false);
                }*/

                //float speedModifier = isSprinting ? SprintSpeedModifier : 1f;
                float speedModifier = 1f;

                // converts move input to a worldspace vector based on our character's transform orientation
                Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());
                //if(worldspaceMoveInput != Vector3.zero) Debug.Log(worldspaceMoveInput);
                // handle grounded movement
                if (IsGrounded)
                {
                    // calculate the desired velocity from inputs, max speed, and current slope
                    Vector3 targetVelocity = worldspaceMoveInput * MaxSpeedOnGround * speedModifier;
                    if(targetVelocity != Vector3.zero) LastCharacterVelocity = worldspaceMoveInput*MaxSpeedOnGround;
                    // reduce speed if crouching by crouch speed ratio
                    /*if (IsCrouching) /// Agachado
                        targetVelocity *= MaxSpeedCrouchedRatio;
                    targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) *
                                     targetVelocity.magnitude;
                    */

                    // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                    if(CantStopMoving){
                        CharacterVelocity = Vector3.Lerp(CharacterVelocity, LastCharacterVelocity,
                            MovementSharpnessOnGround * Time.deltaTime);
                    }else{
                        if(!isDashing){
                            CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                            MovementSharpnessOnGround * Time.deltaTime);
                        }else{
                            CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                            BaseMovementSharpnessOnGround * 2 * Time.deltaTime);
                        }
                    }

                    /*
                    float verticalVelocity;
                    if(CharacterVelocity.y > MaxSpeedInAir) verticalVelocity = MaxSpeedInAir;
                    else verticalVelocity = CharacterVelocity.y;
                    Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                    if(isDashing) {
                        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, dashForce * speedModifier);
                        CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
                    }
                    */


                    // jumping
                    if (IsGrounded && m_InputHandler.GetJumpInputDown())
                    {
                        // force the crouch state to false
                        if (SetCrouchingState(false, false))
                        {
                            // start by canceling out the vertical component of our velocity
                            CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

                            // then, add the jumpSpeed value upwards
                            CharacterVelocity += Vector3.up * JumpForce;

                            // play sound
                            AudioSource.PlayOneShot(JumpSfx);

                            // remember last time we jumped because we need to prevent snapping to ground for a short time
                            m_LastTimeJumped = Time.time;
                            HasJumpedThisFrame = true;

                            // Force grounding to false
                            IsGrounded = false;
                            m_GroundNormal = Vector3.up;
                        }
                    }

                    // footsteps sound
                    float chosenFootstepSfxFrequency =
                        //(isSprinting ? FootstepSfxFrequencyWhileSprinting : FootstepSfxFrequency);
                        (FootstepSfxFrequency);
                    if (m_FootstepDistanceCounter >= 1f / chosenFootstepSfxFrequency)
                    {
                        m_FootstepDistanceCounter = 0f;
                        AudioSource.PlayOneShot(FootstepSfx);
                    }

                    // keep track of distance traveled for footsteps sound
                    m_FootstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
                }
                // handle air movement
                else
                {
                    // add air acceleration
                    CharacterVelocity += worldspaceMoveInput * AccelerationSpeedInAir * Time.deltaTime;

                    // limit air speed to a maximum, but only horizontally
                    float verticalVelocity;
                    //if(CharacterVelocity.y > MaxSpeedInAir) verticalVelocity = MaxSpeedInAir;
                    if(CharacterVelocity.y > JumpForce) verticalVelocity = JumpForce;
                    else verticalVelocity = CharacterVelocity.y;
                    Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                    //if(!isDashing) horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeedInAir * speedModifier);
                    if(!isDashing) horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, BaseMaxSpeedOnGround * speedModifier);
                    else horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, dashForce * speedModifier);
                    CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                    // apply the gravity to the velocity
                    //if(!isDashing) 
                    CharacterVelocity += Vector3.down * GravityDownForce * Time.deltaTime;
                }
            }

            // apply the final calculated velocity value as a character movement
            Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
            Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(m_Controller.height);
            m_Controller.Move(CharacterVelocity * Time.deltaTime);
            //if(!CantStopMoving) m_Controller.Move(CharacterVelocity * Time.deltaTime);
            //else m_Controller.Move((LastCharacterVelocity.normalized*SpeedOnGround) * Time.deltaTime);

            // detect obstructions to adjust velocity accordingly
            m_LatestImpactSpeed = Vector3.zero;
            if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius,
                CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
                QueryTriggerInteraction.Ignore))
            {
                // We remember the last impact speed because the fall damage logic might need it
                m_LatestImpactSpeed = CharacterVelocity;

                CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
            }
        }

        // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
        bool IsNormalUnderSlopeLimit(Vector3 normal)
        {
            return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
        }

        // Gets the center point of the bottom hemisphere of the character controller capsule    
        Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + (transform.up * m_Controller.radius);
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return transform.position + (transform.up * (atHeight - m_Controller.radius));
        }

        // Gets a reoriented direction that is tangent to a given slope
        public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            Vector3 directionRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }

        void UpdateCharacterHeight(bool force)
        {
            // Update height instantly
            if (force)
            {
                m_Controller.height = m_TargetCharacterHeight;
                m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
                PlayerCamera.transform.localPosition = Vector3.up * m_TargetCharacterHeight * CameraHeightRatio;
                m_Actor.AimPoint.transform.localPosition = m_Controller.center;
            }
            // Update smooth height
            else if (m_Controller.height != m_TargetCharacterHeight)
            {
                // resize the capsule and adjust camera position
                m_Controller.height = Mathf.Lerp(m_Controller.height, m_TargetCharacterHeight,
                    CrouchingSharpness * Time.deltaTime);
                m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
                PlayerCamera.transform.localPosition = Vector3.Lerp(PlayerCamera.transform.localPosition,
                    Vector3.up * m_TargetCharacterHeight * CameraHeightRatio, CrouchingSharpness * Time.deltaTime);
                m_Actor.AimPoint.transform.localPosition = m_Controller.center;
            }
        }

        // returns false if there was an obstruction
        bool SetCrouchingState(bool crouched, bool ignoreObstructions)
        {
            // set appropriate heights
            /*if (crouched)
            {
                m_TargetCharacterHeight = CapsuleHeightCrouching;
            }
            else
            {*/
            // Detect obstructions
            if (!ignoreObstructions)
            {
                Collider[] standingOverlaps = Physics.OverlapCapsule(
                    GetCapsuleBottomHemisphere(),
                    GetCapsuleTopHemisphere(CapsuleHeightStanding),
                    m_Controller.radius,
                    -1,
                    QueryTriggerInteraction.Ignore);
                foreach (Collider c in standingOverlaps)
                {
                    if (c != m_Controller)
                    {
                        return false;
                    }
                }
            }

            m_TargetCharacterHeight = CapsuleHeightStanding;
            //}

            if (OnStanceChanged != null)
            {
                OnStanceChanged.Invoke(crouched);
            }

            //IsCrouching = crouched;
            return true;
        }

        private void Dash(){
            if(dashCDTimer > 0) return;
            else dashCDTimer = dashCD;
            
            Transform forwardT = orientation;
            Vector3 direction = GetDirection(forwardT);

            Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;
            isDashing = true;
            //m_WeaponsManager.SetFov(m_WeaponsManager.DefaultFov*1.1f);
            StartCoroutine(ResetDashing());
            //m_Controller.Move(forceToApply);
            delayedForceToApply = forceToApply;
            Invoke(nameof(DelayedDashForce),0.025f);
            //rb.AddForce(forceToApply,ForceMode.Impulse);

            Invoke(nameof(ResetDash),dashDuration);
        }
        IEnumerator ResetDashing(){
            yield return new WaitForSeconds(0.05f);
            isDashing = false;
            //m_WeaponsManager.SetFov(m_WeaponsManager.DefaultFov);
        }
        private Vector3 delayedForceToApply;
        private void DelayedDashForce(){
            if(IsGrounded) {
                //m_Controller.Move(delayedForceToApply);
                CharacterVelocity += delayedForceToApply;
            }
            else{
                CharacterVelocity.Set(CharacterVelocity.x,0,CharacterVelocity.z);
                //m_Controller.Move(delayedForceToApply);
                CharacterVelocity += delayedForceToApply;
            }
            //Debug.Log(CharacterVelocity);
        }
        private void ResetDash(){
            
        }

        private Vector3 GetDirection(Transform forwardT){
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3();

            direction = forwardT.forward*verticalInput + forwardT.right*horizontalInput;

            if(verticalInput == 0 && horizontalInput == 0){
                direction = forwardT.forward;
            }
            return direction.normalized;
        }
    }
}