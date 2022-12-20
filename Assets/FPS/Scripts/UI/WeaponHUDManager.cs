using System.Collections.Generic;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class WeaponHUDManager : MonoBehaviour
    {
        [Tooltip("UI panel containing the layoutGroup for displaying weapon ammo")]
        public RectTransform AmmoPanel;

        //[Tooltip("Prefab for displaying weapon ammo")]
        //public GameObject AmmoCounterPrefab;
        [Tooltip("Prefab for displaying mainn weapon ammo")]
        public GameObject MainWeapon;
        [Tooltip("Prefab for displaying secondary ammo")]
        public GameObject SecondaryWeapon;

        PlayerWeaponsManager m_PlayerWeaponsManager;
        List<AmmoCounter> m_AmmoCounters = new List<AmmoCounter>();

        void Start()
        {
            m_PlayerWeaponsManager = FindObjectOfType<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerWeaponsManager, WeaponHUDManager>(m_PlayerWeaponsManager,
                this);

            WeaponController activeWeapon = m_PlayerWeaponsManager.GetActiveWeapon();
            if (activeWeapon)
            {
                AddWeapon(activeWeapon, m_PlayerWeaponsManager.ActiveWeaponIndex);
                ChangeWeapon(activeWeapon);
            }

            if(m_AmmoCounters.Count == 1){
                SecondaryWeapon.SetActive(false);
            }else if(m_AmmoCounters.Count == 0){
                MainWeapon.SetActive(false);
                SecondaryWeapon.SetActive(false);
            }

            m_PlayerWeaponsManager.OnAddedWeapon += AddWeapon;
            m_PlayerWeaponsManager.OnRemovedWeapon += RemoveWeapon;
            m_PlayerWeaponsManager.OnSwitchedToWeapon += ChangeWeapon;
        }

        void AddWeapon(WeaponController newWeapon, int weaponIndex)
        {
            WeaponController activeWeapon = m_PlayerWeaponsManager.GetActiveWeapon();
            if(activeWeapon){
                if(newWeapon == activeWeapon){ // Is Active Weapon
                    AmmoCounter newAmmoCounter = MainWeapon.GetComponent<AmmoCounter>();
                    DebugUtility.HandleErrorIfNullGetComponent<AmmoCounter, WeaponHUDManager>(newAmmoCounter, this,
                        MainWeapon.gameObject);

                    newAmmoCounter.Initialize(newWeapon, weaponIndex);

                    m_AmmoCounters.Add(newAmmoCounter);
                    MainWeapon.SetActive(true);
                }else{
                    AmmoCounter newAmmoCounter = SecondaryWeapon.GetComponent<AmmoCounter>();
                    DebugUtility.HandleErrorIfNullGetComponent<AmmoCounter, WeaponHUDManager>(newAmmoCounter, this,
                        SecondaryWeapon.gameObject);

                    newAmmoCounter.Initialize(newWeapon, weaponIndex);

                    m_AmmoCounters.Add(newAmmoCounter);
                }
            }else{
                AmmoCounter newAmmoCounter = MainWeapon.GetComponent<AmmoCounter>();
                DebugUtility.HandleErrorIfNullGetComponent<AmmoCounter, WeaponHUDManager>(newAmmoCounter, this,
                    MainWeapon.gameObject);

                newAmmoCounter.Initialize(newWeapon, weaponIndex);

                m_AmmoCounters.Add(newAmmoCounter);
                MainWeapon.SetActive(true);
            }
            if(m_AmmoCounters.Count == 1){
                SecondaryWeapon.SetActive(false);
            }else if(m_AmmoCounters.Count == 2){
                SecondaryWeapon.SetActive(true);
            }
            /*
            GameObject ammoCounterInstance = Instantiate(AmmoCounterPrefab, AmmoPanel);
            AmmoCounter newAmmoCounter = ammoCounterInstance.GetComponent<AmmoCounter>();
            DebugUtility.HandleErrorIfNullGetComponent<AmmoCounter, WeaponHUDManager>(newAmmoCounter, this,
                ammoCounterInstance.gameObject);

            newAmmoCounter.Initialize(newWeapon, weaponIndex);

            m_AmmoCounters.Add(newAmmoCounter);*/
        }

        void RemoveWeapon(WeaponController newWeapon, int weaponIndex)
        {
            //int foundCounterIndex = -1;
            /*for (int i = 0; i < m_AmmoCounters.Count; i++)
            {
                if (m_AmmoCounters[i].WeaponCounterIndex == weaponIndex)
                {
                    foundCounterIndex = i;
                    //Destroy(m_AmmoCounters[i].gameObject);
                }
            }*/
            if(m_AmmoCounters.Count == 2){
                
                AmmoCounter MainCounter = MainWeapon.GetComponent<AmmoCounter>();
                DebugUtility.HandleErrorIfNullGetComponent<AmmoCounter, WeaponHUDManager>(MainCounter, this,
                    MainWeapon.gameObject);
                AmmoCounter SeconCounter = SecondaryWeapon.GetComponent<AmmoCounter>();
                DebugUtility.HandleErrorIfNullGetComponent<AmmoCounter, WeaponHUDManager>(SeconCounter, this,
                    MainWeapon.gameObject);

                WeaponController auxwp = MainCounter.m_Weapon;
                int auxIndex = MainCounter.WeaponCounterIndex;
                //Debug.Log(""+auxwp.WeaponName+" with index "+auxIndex);
                MainCounter.Initialize(SeconCounter.m_Weapon, SeconCounter.WeaponCounterIndex);
                //MainCounter = SeconCounter;
                //Debug.Log(""+auxwp.WeaponName+" with index "+auxIndex);
                SeconCounter.Initialize(auxwp, auxIndex);
                SecondaryWeapon.SetActive(false);
                //SeconCounter = aux;
                
            } else if(m_AmmoCounters.Count == 1){
                MainWeapon.SetActive(false);
            }

            /*if (foundCounterIndex >= 0)
            {
                m_AmmoCounters.RemoveAt(foundCounterIndex);
            }*/
            if(m_AmmoCounters.Count == 1) m_AmmoCounters.RemoveAt(0);
            else m_AmmoCounters.RemoveAt(weaponIndex);
        }

        void ChangeWeapon(WeaponController weapon)
        {
            //Debug.Log(weapon.WeaponName);
            if(m_AmmoCounters.Count == 2){
                
                AmmoCounter MainCounter = MainWeapon.GetComponent<AmmoCounter>();
                DebugUtility.HandleErrorIfNullGetComponent<AmmoCounter, WeaponHUDManager>(MainCounter, this,
                    MainWeapon.gameObject);
                AmmoCounter SeconCounter = SecondaryWeapon.GetComponent<AmmoCounter>();
                DebugUtility.HandleErrorIfNullGetComponent<AmmoCounter, WeaponHUDManager>(SeconCounter, this,
                    MainWeapon.gameObject);

                WeaponController auxwp = MainCounter.m_Weapon;
                int auxIndex = MainCounter.WeaponCounterIndex;
                //Debug.Log(""+auxwp.WeaponName+" with index "+auxIndex);
                MainCounter.Initialize(SeconCounter.m_Weapon, SeconCounter.WeaponCounterIndex);
                //MainCounter = SeconCounter;
                //Debug.Log(""+auxwp.WeaponName+" with index "+auxIndex);
                SeconCounter.Initialize(auxwp, auxIndex);
                //SeconCounter = aux;
                
            }
            /*GameObject aux = SecondaryWeapon;
            SecondaryWeapon = MainWeapon;
            MainWeapon = aux;*/
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(AmmoPanel);
        }
    }
}