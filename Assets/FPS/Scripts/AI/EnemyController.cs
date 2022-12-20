using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;

namespace Unity.FPS.AI
{
    [RequireComponent(typeof(Health), typeof(Actor), typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        public UnityAction onDefeated;
        [System.Serializable]
        public struct RendererIndexData
        {
            public Renderer Renderer;
            public int MaterialIndex;

            public RendererIndexData(Renderer renderer, int index)
            {
                Renderer = renderer;
                MaterialIndex = index;
            }
        }
        [Header("On kill")]
        [Tooltip("Min ammount of money given to the player when killed")]
        public int minMoney = 10;
        [Tooltip("Max ammount of money given to the player when killed")]
        public int maxMoney = 20;
        [Header("Text for Boss")]
        public ChatBox chatBox;
        public List<string> Speach = new List<string>();
        public List<string> EnrageSpeach = new List<string>();

        [Header("Parameters")]
        [Tooltip("The Y height at which the enemy will be automatically killed (if it falls off of the level)")]
        public float SelfDestructYHeight = -20f;

        [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
        public float PathReachingRadius = 2f;

        [Tooltip("The speed at which the enemy rotates")]
        public float OrientationSpeed = 10f;

        [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
        public float DeathDuration = 0f;


        [Header("Weapons Parameters")] [Tooltip("Allow weapon swapping for this enemy")]
        public bool SwapToNextWeapon = false;

        [Tooltip("Time delay between a weapon swap and the next attack")]
        public float DelayAfterWeaponSwap = 0f;

        [Header("Eye color")] [Tooltip("Material for the eye color")]
        public Material EyeColorMaterial;

        [Tooltip("The default color of the bot's eye")] [ColorUsageAttribute(true, true)]
        public Color DefaultEyeColor;

        [Tooltip("The attack color of the bot's eye")] [ColorUsageAttribute(true, true)]
        public Color AttackEyeColor;

        [Header("Flash on hit")] [Tooltip("The material used for the body of the hoverbot")]
        public Material BodyMaterial;

        [Tooltip("The gradient representing the color of the flash on hit")] [GradientUsageAttribute(true)]
        public Gradient OnHitBodyGradient;

        [Tooltip("The duration of the flash on hit")]
        public float FlashOnHitDuration = 0.5f;

        [Header("Sounds")] [Tooltip("Sound played when recieving damages")]
        public AudioClip DamageTick;

        [Header("VFX")] [Tooltip("The VFX prefab spawned when the enemy dies")]
        public GameObject DeathVfx;

        [Tooltip("The point at which the death VFX is spawned")]
        public Transform DeathVfxSpawnPoint;

        [Header("Loot")] [Tooltip("The object this enemy can drop when dying")]
        public GameObject LootPrefab;

        [Tooltip("The chance the object has to drop")] [Range(0, 1)]
        public float DropRate = 1f;

        [Header("Debug Display")] [Tooltip("Color of the sphere gizmo representing the path reaching range")]
        public Color PathReachingRangeColor = Color.yellow;

        [Tooltip("Color of the sphere gizmo representing the attack range")]
        public Color AttackRangeColor = Color.red;

        [Tooltip("Color of the sphere gizmo representing the detection range")]
        public Color DetectionRangeColor = Color.blue;

        [Tooltip("Discurso del Boss")]
        public AudioSource discurso;
        [Tooltip("Enojo del Boss")]
        public AudioSource enojo;
        [Tooltip("Carga disparo tortuga")]
        public AudioSource SonidoCargandoDisparo;
        [Tooltip("Carga hasta el discurso del Boss")]
        public float TiempoHablando = 0.0f;
        [Tooltip("Duracion del discurso del Boss")]
        public float TiempoDiscurso = 10.0f;
        [Tooltip("Velocidad Boss sin Enojado")]
        public float BaseVel = 3;
        [Tooltip("Velocidad Boss Enojado")]
        public float BaseVelEnojado = 4;
        private bool Enojado = false;
        [Tooltip("Listado Arma")]
        public WeaponController[] weaponlist;
        [Tooltip("Ver vida")]
        public float seeHealth;
        [Tooltip("Q wea habla este mono")]
        public bool hablando;

        [Tooltip("Cooldown Ataque a Melee")]
        public float CooldownMeleeAtack = 10.0f;
        [Tooltip("Contador para Cooldown Ataque a Melee")]
        public float CountCooldownMeleeAtack = 9.0f;
        [Tooltip("Cooldown Ataque a distancia despues de ataque a Melee")]
        public float CooldownRangeAtack = 5.0f;
        [Tooltip("Cooldown Ataque Cargado")]
        public float CooldownChargeAtack = 10.0f;
        private bool aux = true;
        private bool aux2 = true;
        private int aux3 = 0;
        private int ContShoots = 200;

        public bool auxiliar = false;
        

        //Dash enemigo serpiente
        
        private bool canDash = true;
        //private bool isDashing;
        private float dashingPower = 50.0f;
        private float dashingTime = 2.0f;
        private float dashingCooldown = 6.0f;
        [SerializeField] private TrailRenderer tr;

        
    
        public UnityAction onAttack;
        public UnityAction onDetectedTarget;
        public UnityAction onLostTarget;
        public UnityAction onDamaged;

        List<RendererIndexData> m_BodyRenderers = new List<RendererIndexData>();
        MaterialPropertyBlock m_BodyFlashMaterialPropertyBlock;
        float m_LastTimeDamaged = float.NegativeInfinity;

        RendererIndexData m_EyeRendererData;
        MaterialPropertyBlock m_EyeColorMaterialPropertyBlock;

        public PatrolPath PatrolPath { get; set; }
        public GameObject KnownDetectedTarget => DetectionModule.KnownDetectedTarget;
        public bool IsTargetInAttackRange => DetectionModule.IsTargetInAttackRange;
        public bool IsSeeingTarget => DetectionModule.IsSeeingTarget;
        public bool HadKnownTarget => DetectionModule.HadKnownTarget;
        public NavMeshAgent NavMeshAgent { get; private set; }
        public DetectionModule DetectionModule { get; private set; }

        int m_PathDestinationNodeIndex;
        EnemyManager m_EnemyManager;
        ActorsManager m_ActorsManager;
        Health m_Health;
        Actor m_Actor;
        Collider[] m_SelfColliders;
        GameFlowManager m_GameFlowManager;
        bool m_WasDamagedThisFrame;
        float m_LastTimeWeaponSwapped = Mathf.NegativeInfinity;
        int m_CurrentWeaponIndex;
        WeaponController m_CurrentWeapon;
        WeaponController[] m_Weapons;
        NavigationModule m_NavigationModule;
        bool auxHablando = true;
        private GameObject dmgSource;

        void Start()
        {
            m_EnemyManager = FindObjectOfType<EnemyManager>();
            DebugUtility.HandleErrorIfNullFindObject<EnemyManager, EnemyController>(m_EnemyManager, this);

            m_ActorsManager = FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, EnemyController>(m_ActorsManager, this);

            m_EnemyManager.RegisterEnemy(this);

            m_Health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, EnemyController>(m_Health, this, gameObject);

            m_Actor = GetComponent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Actor, EnemyController>(m_Actor, this, gameObject);

            NavMeshAgent = GetComponent<NavMeshAgent>();
            m_SelfColliders = GetComponentsInChildren<Collider>();

            m_GameFlowManager = FindObjectOfType<GameFlowManager>();
            DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, EnemyController>(m_GameFlowManager, this);

            // Subscribe to damage & death actions
            m_Health.OnDie += OnDie;
            m_Health.OnDamaged += OnDamaged;

            // Find and initialize all weapons
            FindAndInitializeAllWeapons();
            var weapon = GetCurrentWeapon();
            weapon.ShowWeapon(true);

            var detectionModules = GetComponentsInChildren<DetectionModule>();
            DebugUtility.HandleErrorIfNoComponentFound<DetectionModule, EnemyController>(detectionModules.Length, this,
                gameObject);
            DebugUtility.HandleWarningIfDuplicateObjects<DetectionModule, EnemyController>(detectionModules.Length,
                this, gameObject);
            // Initialize detection module
            DetectionModule = detectionModules[0];
            DetectionModule.onDetectedTarget += OnDetectedTarget;
            DetectionModule.onLostTarget += OnLostTarget;
            onAttack += DetectionModule.OnAttack;

            var navigationModules = GetComponentsInChildren<NavigationModule>();
            DebugUtility.HandleWarningIfDuplicateObjects<DetectionModule, EnemyController>(detectionModules.Length,
                this, gameObject);
            // Override navmesh agent data
            if (navigationModules.Length > 0)
            {
                m_NavigationModule = navigationModules[0];
                NavMeshAgent.speed = m_NavigationModule.MoveSpeed;
                
                NavMeshAgent.angularSpeed = m_NavigationModule.AngularSpeed;
                NavMeshAgent.acceleration = m_NavigationModule.Acceleration;
            }

            foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    if (renderer.sharedMaterials[i] == EyeColorMaterial)
                    {
                        m_EyeRendererData = new RendererIndexData(renderer, i);
                    }

                    if (renderer.sharedMaterials[i] == BodyMaterial)
                    {
                        m_BodyRenderers.Add(new RendererIndexData(renderer, i));
                    }
                }
            }

            m_BodyFlashMaterialPropertyBlock = new MaterialPropertyBlock();

            // Check if we have an eye renderer for this enemy
            if (m_EyeRendererData.Renderer != null)
            {
                m_EyeColorMaterialPropertyBlock = new MaterialPropertyBlock();
                m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", DefaultEyeColor);
                m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock,
                    m_EyeRendererData.MaterialIndex);
            }
        }

        void Awake()
        {
            /*if (discurso)
            {
                discurso.Play();
                hablando = true;
                //NavMeshAgent.speed = 0.000001f;
            }*/
            if(chatBox) {
                chatBox.DoneWriting += DoneWriting;
                chatBox.NewText(Speach,0.1f);
                hablando = true;
            }
        }

        void DoneWriting(){
            if(auxHablando){
                auxHablando = false;
                hablando = false;
            }
        }

        void Update()
        {
            EnsureIsWithinLevelBounds();

            DetectionModule.HandleTargetDetection(m_Actor, m_SelfColliders);

            Color currentColor = OnHitBodyGradient.Evaluate((Time.time - m_LastTimeDamaged) / FlashOnHitDuration);
            m_BodyFlashMaterialPropertyBlock.SetColor("_EmissionColor", currentColor);
            foreach (var data in m_BodyRenderers)
            {
                data.Renderer.SetPropertyBlock(m_BodyFlashMaterialPropertyBlock, data.MaterialIndex);
            }

            m_WasDamagedThisFrame = false;

            //if (discurso && hablando || (chatBox && hablando))
            if ((chatBox && hablando))
            {
                TiempoHablando += 1*Time.deltaTime;
                //if (TiempoHablando>=TiempoDiscurso)
                //{
                //    hablando = false;
                //}
            }
            seeHealth= m_Health.CurrentHealth;
            //if (hablando && discurso)
            if (hablando && chatBox)
            {
                NavMeshAgent.speed = 0.0001f;
            }
            //else if (Enojado && discurso)
            else if (Enojado && chatBox)
            {
                if (this.tag=="SnakeBoss" && aux)
                {
                    NavMeshAgent.speed = 15;
                    aux = false;
                    tr.emitting = true;
                    
                }
                else{
                NavMeshAgent.speed = BaseVelEnojado;
                }
            }
            //else if (Enojado == false && discurso)
            else if (Enojado == false && chatBox)
            {
                if (this.tag=="SnakeBoss" && aux)
                {
                    NavMeshAgent.speed = 10;
                    aux = false;
                    tr.emitting = true;
                }
                else{
                NavMeshAgent.speed = BaseVel;
                }
            }
        }


        void Enrage()
        {
            //discurso.Stop();
            //chatBox.StopText();
            //enojo.Play();
            chatBox.StopText();
            chatBox.NewText(EnrageSpeach,0.5f);
            Enojado = true;
            m_Health.MaxHealth = m_Health.MaxHealth *2.0f;
            m_Health.CurrentHealth = m_Health.CurrentHealth * 2.0f;
            
            if (weaponlist.Length!=0)
            {
                foreach(WeaponController WP in weaponlist)
                {
                    WP.dmgAmount = WP.dmgAmount*1.3f;
                }
            }
        }

        void EnsureIsWithinLevelBounds()
        {
            // at every frame, this tests for conditions to kill the enemy
            if (transform.position.y < SelfDestructYHeight)
            {
                Destroy(gameObject);
                return;
            }
        }

        void OnLostTarget()
        {
            onLostTarget.Invoke();

            // Set the eye attack color and property block if the eye renderer is set
            if (m_EyeRendererData.Renderer != null)
            {
                m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", DefaultEyeColor);
                m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock,
                    m_EyeRendererData.MaterialIndex);
            }
        }

        void OnDetectedTarget()
        {
            onDetectedTarget.Invoke();

            // Set the eye default color and property block if the eye renderer is set
            if (m_EyeRendererData.Renderer != null)
            {
                m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", AttackEyeColor);
                m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock,
                    m_EyeRendererData.MaterialIndex);
            }
        }

        public void OrientTowards(Vector3 lookPosition)
        {
            Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - transform.position, Vector3.up).normalized;
            if (lookDirection.sqrMagnitude != 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * OrientationSpeed);
            }
        }

        bool IsPathValid()
        {
            return PatrolPath && PatrolPath.PathNodes.Count > 0;
        }

        public void ResetPathDestination()
        {
            m_PathDestinationNodeIndex = 0;
        }

        public void SetPathDestinationToClosestNode()
        {
            if (IsPathValid())
            {
                int closestPathNodeIndex = 0;
                for (int i = 0; i < PatrolPath.PathNodes.Count; i++)
                {
                    float distanceToPathNode = PatrolPath.GetDistanceToNode(transform.position, i);
                    if (distanceToPathNode < PatrolPath.GetDistanceToNode(transform.position, closestPathNodeIndex))
                    {
                        closestPathNodeIndex = i;
                    }
                }

                m_PathDestinationNodeIndex = closestPathNodeIndex;
            }
            else
            {
                m_PathDestinationNodeIndex = 0;
            }
        }

        public Vector3 GetDestinationOnPath()
        {
            if (IsPathValid())
            {
                return PatrolPath.GetPositionOfPathNode(m_PathDestinationNodeIndex);
            }
            else
            {
                return transform.position;
            }
        }

        public void SetNavDestination(Vector3 destination)
        {
            if (NavMeshAgent)
            {
                NavMeshAgent.SetDestination(destination);
            }
        }

        public void UpdatePathDestination(bool inverseOrder = false)
        {
            if (IsPathValid())
            {
                // Check if reached the path destination
                if ((transform.position - GetDestinationOnPath()).magnitude <= PathReachingRadius)
                {
                    // increment path destination index
                    m_PathDestinationNodeIndex =
                        inverseOrder ? (m_PathDestinationNodeIndex - 1) : (m_PathDestinationNodeIndex + 1);
                    if (m_PathDestinationNodeIndex < 0)
                    {
                        m_PathDestinationNodeIndex += PatrolPath.PathNodes.Count;
                    }

                    if (m_PathDestinationNodeIndex >= PatrolPath.PathNodes.Count)
                    {
                        m_PathDestinationNodeIndex -= PatrolPath.PathNodes.Count;
                    }
                }
            }
        }

        void OnDamaged(float damage, GameObject damageSource)
        {
            // test if the damage source is the player
            if (damageSource && !damageSource.GetComponent<EnemyController>())
            {
                // pursue the player
                DetectionModule.OnDamaged(damageSource);
                dmgSource = damageSource;
                
                onDamaged?.Invoke();
                m_LastTimeDamaged = Time.time;
            
                // play the damage tick sound
                if (DamageTick && !m_WasDamagedThisFrame)
                    AudioUtility.CreateSFX(DamageTick, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);
            
                m_WasDamagedThisFrame = true;

                //if((TiempoHablando<TiempoDiscurso)&&(hablando))
                if((chatBox)&&(hablando))
                {
                    Enrage();
                    hablando = false;
                }
                
            }

        }

        void OnDie()
        {
            // spawn a particle system when dying
            var vfx = Instantiate(DeathVfx, DeathVfxSpawnPoint.position, Quaternion.identity);
            Destroy(vfx, 5f);

            // tells the game flow manager to handle the enemy destuction
            m_EnemyManager.UnregisterEnemy(this);

            // loot an object
            if (TryDropItem())
            {
                Instantiate(LootPrefab, transform.position, Quaternion.identity);
            }
            if(dmgSource){
                Money m_PlayerMoney = dmgSource.GetComponent<Money>();
                m_PlayerMoney.AddMoney(Random.Range(minMoney,maxMoney));
                //Debug.Log("Player money: "+m_PlayerMoney.CurrentMoney);
            }
            // this will call the OnDestroy function
            onDefeated?.Invoke();
            Destroy(gameObject, DeathDuration);
        }

        void OnDrawGizmosSelected()
        {
            // Path reaching range
            Gizmos.color = PathReachingRangeColor;
            Gizmos.DrawWireSphere(transform.position, PathReachingRadius);

            if (DetectionModule != null)
            {
                // Detection range
                Gizmos.color = DetectionRangeColor;
                Gizmos.DrawWireSphere(transform.position, DetectionModule.DetectionRange);

                // Attack range
                Gizmos.color = AttackRangeColor;
                Gizmos.DrawWireSphere(transform.position, DetectionModule.AttackRange);
            }
        }

        public void OrientWeaponsTowards(Vector3 lookPosition)
        {
            for (int i = 0; i < m_Weapons.Length; i++)
            {
                
                //orient weapon towards player
                Vector3 weaponForward = (lookPosition - m_Weapons[i].WeaponRoot.transform.position).normalized;
                m_Weapons[i].transform.forward = weaponForward;
                
                // orient weapon towards player
                //Vector3 weaponForward = (lookPosition - m_Weapons[i].WeaponRoot.transform.position).normalized;
                //m_Weapons[i].transform.forward = weaponForward;
            }
        }
        private IEnumerator Dash()
        {
            canDash = false;
            NavMeshAgent.speed = NavMeshAgent.speed * dashingPower;
            tr.emitting = true;
            yield return new WaitForSeconds(dashingTime);
            tr.emitting = false;
            NavMeshAgent.speed = NavMeshAgent.speed/ dashingPower;
            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }
        IEnumerator Kami()
        {
            yield return new WaitForSeconds(0.2f);
            OnDie();
        }

        public bool TryAtack(Vector3 enemyPosition)
        {   
            if (m_GameFlowManager.GameIsEnding)
                return false;

            OrientWeaponsTowards(enemyPosition);

            if ((m_LastTimeWeaponSwapped + DelayAfterWeaponSwap) >= Time.time)
                return false;

            if (hablando)
            {
                auxiliar=false;
            }
            bool didFire = false;
            if (auxiliar){
                didFire = GetCurrentWeapon().HandleShootInputs(false, true, false);
            }
            // Shoot the weapon
            //bool didFire = GetCurrentWeapon().HandleShootInputs(false, true, false);
            
            
            if (!hablando)
            {
                CountCooldownMeleeAtack += 1*Time.deltaTime;
            }
            
            if(this.tag=="Snake"){//check
                if (canDash && ((enemyPosition - this.transform.position).magnitude <=8f))
                    {
                    StartCoroutine(Dash());
                    }
                if ((enemyPosition - this.transform.position).magnitude <=3f)
                {
                    
                    if (CountCooldownMeleeAtack < CooldownMeleeAtack)//ataco hace poco
                    {
                        auxiliar = false;
                        return false;
                    }
                    else if (CountCooldownMeleeAtack >= CooldownMeleeAtack)
                    {
                        SetCurrentWeapon(0);
                        auxiliar = true;
                        CountCooldownMeleeAtack = 0.0f;
                        return didFire;
                    } 
                }
            }
            if(this.tag=="SnakeBoss"){
                if (hablando)
                {
                    auxiliar = false;
                    return false;
                }
                if ((enemyPosition - this.transform.position).magnitude <=4f)
                {
                    
                    if (CountCooldownMeleeAtack < CooldownMeleeAtack)//ataco hace poco
                    {
                        auxiliar = false;
                        return false;
                    }
                    else if (CountCooldownMeleeAtack >= CooldownMeleeAtack)
                    {
                        SetCurrentWeapon(0);
                        auxiliar = true;
                        CountCooldownMeleeAtack = 0.0f;
                        return didFire;
                    }
                }
            }
            if(this.tag=="Kamikace"){//check
                if (ContShoots<=500){
                    Debug.Log(ContShoots);
                    auxiliar = true;
                    if (aux2)
                        {
                            SetCurrentWeapon(1);//izq 
                            aux2 = false;
                            ContShoots=ContShoots+1;
                            if (ContShoots>500)
                            {
                                //auxiliar = false;
                                SetCurrentWeapon(0);
                            }
                            return didFire;
                        }
                        else
                        {
                            SetCurrentWeapon(2);//der 
                            aux2 = true;
                            ContShoots=ContShoots+1;
                            if (ContShoots>500)
                            {
                                //auxiliar = false;
                                SetCurrentWeapon(0);
                            }
                            return didFire;
                        }
                }
                else{
                    NavMeshAgent.speed=10;
                    
                    if ((enemyPosition - this.transform.position).magnitude < 3f)
                    {
                        auxiliar = true;
                        StartCoroutine(Kami());
                        return didFire;
                    }
                    else{
                        auxiliar = false;
                    }
                    return false;
                }   
            }
            if(this.tag=="TortugaBoss")//tortuga check, cambiar velocidad de proyectil
            {
                if (hablando)
                {
                    auxiliar = false;
                    return false;
                }
                else if (!hablando)
                {
                    if ((ContShoots<=800)&&(ContShoots>200))
                    {
                        Debug.Log(ContShoots);
                        auxiliar = true;
                        if (aux2)
                        {
                            SetCurrentWeapon(2);//ojo izq 
                            aux2 = false;
                            ContShoots=ContShoots+1;
                            if (ContShoots>800)
                            {
                                auxiliar = false;
                            }
                            return didFire;
                        }
                        else
                        {
                            SetCurrentWeapon(0);//ojo derecho
                            aux2 = true;
                            ContShoots=ContShoots+1;
                            if (ContShoots>800)
                            {
                                auxiliar = false;
                            }
                            return didFire;
                        }
                    }
                    else
                    {
                        Debug.Log(ContShoots);
                        ContShoots=ContShoots+1;
                        if ((ContShoots>800)&& aux)
                        {
                            aux = false;
                            auxiliar = false;
                            SonidoCargandoDisparo.Play();//sonido de carga
                        }
                        else if (ContShoots>1150)
                        {
                            ContShoots = 0;
                            aux = true;
                            auxiliar = true;
                            SetCurrentWeapon(1);//poder cargado
                            return didFire;
                            
                        }
                        else
                        {
                            auxiliar = false;
                        }

                    }
                }
                 
            }
            if(this.tag=="EnemyShelf")
                {
                    SetCurrentWeapon(0);
                    auxiliar = true;
                    return didFire;
                }
            if(this.tag=="CangrejoBoss")
            {
                if (hablando)
                {
                    auxiliar = false;
                    return false;
                }
                else if (!hablando)
                {
                    if ((ContShoots<=800)&&(ContShoots>200))
                    {
                        Debug.Log(ContShoots);
                        auxiliar = true;
                        if (aux2)
                        {
                            SetCurrentWeapon(1);//ojo izq 
                            aux2 = false;
                            ContShoots=ContShoots+1;
                            if (ContShoots>800)
                            {
                                auxiliar = false;
                            }
                            return didFire;
                        }
                        else
                        {
                            SetCurrentWeapon(0);//ojo derecho
                            aux2 = true;
                            ContShoots=ContShoots+1;
                            if (ContShoots>800)
                            {
                                auxiliar = false;
                            }
                            return didFire;
                        }
                    }
                    else
                    {
                        Debug.Log(ContShoots);
                        ContShoots=ContShoots+1;
                        if ((ContShoots>800)&& aux)
                        {
                            aux = false;
                            auxiliar = false;
                            SonidoCargandoDisparo.Play();//sonido de toma la wa
                        }
                        else if (ContShoots>1150)
                        {
                            if (aux3==0)
                            {
                                ContShoots = 0;
                                aux3 = 1;
                                aux = true;
                                auxiliar = true;
                                SetCurrentWeapon(2);//ClawIzq
                                return didFire;
                            }
                            else if(aux3==1)
                            {
                                ContShoots = 0;
                                aux3 = 2;
                                aux = true;
                                auxiliar = true;
                                SetCurrentWeapon(3);//ClawDer
                                return didFire;
                            }
                            else if(aux3==2)
                            {
                                ContShoots = 0;
                                aux3 = 0;
                                aux = true;
                                auxiliar = true;
                                SetCurrentWeapon(4);//Spit
                                return didFire;
                            }
                        }
                        else
                        {
                            auxiliar = false;
                        }

                    }
                }
            }
            
            if (didFire && onAttack != null)
            {
                
                
                /*{
                    SetCurrentWeapon(1);
                    if ((enemyPosition - this.transform.position).magnitude >5f)
                    {
                        if (CountCooldownMeleeAtack < CooldownRangeAtack)
                        {
                            return false;
                        }
                        else
                        {
                            SetCurrentWeapon(1);//ARMA DE LEJOS
                        }
                        
                    }
                    
                    if ((enemyPosition - this.transform.position).magnitude <=2f)
                    {
                        SetCurrentWeapon(0);//ARMA DE CERCA SI ESTA A MENOS DE 2F
                    }
                    if ((enemyPosition - this.transform.position).magnitude <=5f && (enemyPosition - this.transform.position).magnitude > 2f)
                    {
                        return false;//ARMA SIN DANO SI ESTA ENTRE 5F Y 2F
                    }
                    if (GetCurrentWeapon() == m_Weapons[0])
                    {
                        if (CountCooldownMeleeAtack < CooldownMeleeAtack)//ataco hace poco
                        {
                            return false;
                        }
                        else if (CountCooldownMeleeAtack >= CooldownMeleeAtack)
                        {
                            CountCooldownMeleeAtack = 0.0f;
                            return didFire;
                        } 
                    }
                }*/
                
                
            }
            onAttack.Invoke();
        return didFire;
        }
        
        

        public bool TryDropItem()
        {
            if (DropRate == 0 || LootPrefab == null)
                return false;
            else if (DropRate == 1)
                return true;
            else
                return (Random.value <= DropRate);
        }

        void FindAndInitializeAllWeapons()
        {
            // Check if we already found and initialized the weapons
            if (m_Weapons == null)
            {
                m_Weapons = GetComponentsInChildren<WeaponController>();
                DebugUtility.HandleErrorIfNoComponentFound<WeaponController, EnemyController>(m_Weapons.Length, this,
                    gameObject);

                for (int i = 0; i < m_Weapons.Length; i++)
                {
                    m_Weapons[i].Owner = gameObject;
                }
            }
        }

        public WeaponController GetCurrentWeapon()
        {
            FindAndInitializeAllWeapons();
            // Check if no weapon is currently selected
            if (m_CurrentWeapon == null)
            {
                // Set the first weapon of the weapons list as the current weapon
                SetCurrentWeapon(0);
            }

            DebugUtility.HandleErrorIfNullGetComponent<WeaponController, EnemyController>(m_CurrentWeapon, this,
                gameObject);

            return m_CurrentWeapon;
        }

        void SetCurrentWeapon(int index)
        {
            m_CurrentWeaponIndex = index;
            m_CurrentWeapon = m_Weapons[m_CurrentWeaponIndex];
            if (SwapToNextWeapon)
            {
                m_LastTimeWeaponSwapped = Time.time;
            }
            else
            {
                m_LastTimeWeaponSwapped = Mathf.NegativeInfinity;
            }
        }
    }
}