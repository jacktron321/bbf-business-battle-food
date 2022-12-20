using TMPro;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    [RequireComponent(typeof(FillBarColorChange))]
    public class AmmoCounter : MonoBehaviour
    {
        [Tooltip("CanvasGroup to fade the ammo UI")]
        public CanvasGroup CanvasGroup;

        //[Tooltip("Image for the weapon icon")] public Image WeaponImage;

        [Tooltip("Image component for the background")]
        public Image AmmoBackgroundImage;

        [Tooltip("Image component to display fill ratio")]
        public Image AmmoFillImage;

        [Tooltip("Text for Weapon index")] 
        public TextMeshProUGUI WeaponIndexText;

        //[Tooltip("Text for Bullet Counter")] 
        //public TextMeshProUGUI BulletCounter;

        [Tooltip("consumeIcon Text for Weapons with physical bullets")]
        public RectTransform consumeIcon;
        public TextMeshProUGUI consumeText;

        [Header("Selection")] [Range(0, 1)] [Tooltip("Opacity when weapon not selected")]
        public float UnselectedOpacity = 0.5f;

        [Tooltip("Scale when weapon not selected")]
        public Vector3 UnselectedScale = Vector3.one * 0.8f;

        [Tooltip("Root for the control keys")] public GameObject ControlKeysRoot;

        [Header("Feedback")] [Tooltip("Component to animate the color when empty or full")]
        public FillBarColorChange FillBarColorChange;

        [Tooltip("Sharpness for the fill ratio movements")]
        public float AmmoFillMovementSharpness = 20f;

        public int WeaponCounterIndex { get; set; }

        PlayerWeaponsManager m_PlayerWeaponsManager;
        public WeaponController m_Weapon;
        public float lastFill;
        public float FillAmmount;


        /*void Awake()
        {
            EventManager.AddListener<AmmoPickupEvent>(OnAmmoPickup);
        }

        /*void OnAmmoPickup(AmmoPickupEvent evt)
        {
            if (evt.Weapon == m_Weapon)
            {
                BulletCounter.text = m_Weapon.GetCarriedPhysicalBullets().ToString();
            }
        }*/

        public void Initialize(WeaponController weapon, int weaponIndex)
        {
            m_Weapon = weapon;
            WeaponCounterIndex = weaponIndex;
            //WeaponImage.sprite = weapon.WeaponIcon;

            AmmoFillImage.sprite = weapon.WeaponIcon2;
            AmmoBackgroundImage.sprite = weapon.WeaponIcon2;

            /*if (!weapon.HasPhysicalBullets)
                BulletCounter.transform.parent.gameObject.SetActive(false);
            else
                BulletCounter.text = weapon.GetCarriedPhysicalBullets().ToString();*/

            consumeIcon.gameObject.SetActive(false);
            consumeText.gameObject.SetActive(false);
            m_PlayerWeaponsManager = FindObjectOfType<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerWeaponsManager, AmmoCounter>(m_PlayerWeaponsManager, this);

            WeaponIndexText.text = (WeaponCounterIndex + 1).ToString();

            FillBarColorChange.Initialize(1f, m_Weapon.GetAmmoNeededToShoot());
        }
        void Update()
        {
            if(m_Weapon){
                float currenFillRatio = m_Weapon.CurrentAmmoRatio;
                if(!m_PlayerWeaponsManager.isConsuming){
                    AmmoFillImage.fillAmount = Mathf.Lerp(AmmoFillImage.fillAmount, currenFillRatio,
                    Time.deltaTime * AmmoFillMovementSharpness);
                    FillAmmount = AmmoFillImage.fillAmount;

                    WeaponController activeWeapon =m_PlayerWeaponsManager.GetActiveWeapon();
                    if(m_Weapon == activeWeapon) lastFill = AmmoFillImage.fillAmount;
                    
                }else{
                    WeaponController activeWeapon =m_PlayerWeaponsManager.GetActiveWeapon();
                    if(m_Weapon == activeWeapon){
                        float consumeRatio;
                        if(activeWeapon.CurrentAmmoRatio != 0) consumeRatio = 1-(m_PlayerWeaponsManager.consumeTimer/(activeWeapon.timeToConsume*activeWeapon.CurrentAmmoRatio));
                        else consumeRatio = 1f;
                        AmmoFillImage.fillAmount = lastFill*consumeRatio;
                        FillAmmount = AmmoFillImage.fillAmount;
                        //Debug.Log(m_PlayerWeaponsManager.consumeTimer);
                        //Debug.Log(consumeRatio);
                        //Debug.Log(AmmoFillImage.fillAmount);
                        //Debug.Log(AmmoFillImage.fillAmount*consumeRatio);
                        //Debug.Log(consumeRatio);
                        //float aux = AmmoFillImage.fillAmount;
                        //AmmoFillImage.fillAmount = aux*consumeRatio;
                    }
                }

                //BulletCounter.text = m_Weapon.GetCarriedPhysicalBullets().ToString();

                bool isActiveWeapon = m_Weapon == m_PlayerWeaponsManager.GetActiveWeapon();

                CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, isActiveWeapon ? 1f : UnselectedOpacity,
                    Time.deltaTime * 10);
                transform.localScale = Vector3.Lerp(transform.localScale, isActiveWeapon ? Vector3.one : UnselectedScale,
                    Time.deltaTime * 10);
                ControlKeysRoot.SetActive(!isActiveWeapon);

                FillBarColorChange.UpdateVisual(currenFillRatio);

                //consumeIcon.gameObject.SetActive(m_Weapon.GetCarriedPhysicalBullets() > 0 && m_Weapon.GetCurrentAmmo() == 0 && m_Weapon.IsWeaponActive);
                //consumeIcon.gameObject.SetActive(m_Weapon.GetCurrentAmmo() == 0 && m_Weapon.IsWeaponActive);
                consumeIcon.gameObject.SetActive(m_Weapon.GetCurrentAmmo() == 0);
                consumeText.gameObject.SetActive(m_Weapon.GetCurrentAmmo() == 0);
            }
        }

        /*void Destroy()
        {
            EventManager.RemoveListener<AmmoPickupEvent>(OnAmmoPickup);
        }*/
    }
}