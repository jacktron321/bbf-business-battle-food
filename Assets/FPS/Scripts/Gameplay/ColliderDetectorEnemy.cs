using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;

namespace Unity.FPS.Gameplay
{
    public class ColliderDetectorEnemy : MonoBehaviour
    {
        PlayerWeaponsManager m_WeaponsManager;
        PlayerCharacterController m_PlayerCharacterController;

        void Awake() {
            m_PlayerCharacterController = FindObjectOfType<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, Shop>(m_PlayerCharacterController,
                this);

            m_WeaponsManager = m_PlayerCharacterController.GetComponent<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerWeaponsManager, ColliderDetector>(m_WeaponsManager, this, gameObject);
        }
        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Weapon_Carrot"){
                //Debug.Log(this.name);
                //Debug.Log("Hit");
                Damageable damageable = GetComponent<Damageable>();
                DebugUtility.HandleErrorIfNullFindObject<Damageable, ColliderDetector>(damageable,
                this);
                //Damageable damageable = other.GetComponent<Damageable>();
                if (damageable)
                {
                    //Debug.Log("Aqui");
                    WeaponController wp = m_WeaponsManager.GetActiveWeapon();
                    if(wp){
                        damageable.InflictDamage(wp.dmgAmount/2, false, m_PlayerCharacterController.gameObject);
                        //StartCoroutine(waithit());
                        //damageable.InflictDamage(1.0f, false, other.gameObject);
                    }
                }
            }
        }
        //public WeaponController wp;
        // Start is called before the first frame update
        /*PlayerWeaponsManager m_WeaponsManager;
        PlayerCharacterController m_PlayerCharacterController;
        void Awake(){
            m_PlayerCharacterController = FindObjectOfType<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, Shop>(m_PlayerCharacterController,
                this);

            m_WeaponsManager = m_PlayerCharacterController.GetComponent<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerWeaponsManager, ColliderDetector>(m_WeaponsManager, this, gameObject);
        }
        */
        //private void OnTriggerEnter(Collider other) {
        /*private void OnTriggerStay(Collider other) {
            WeaponController wp = m_WeaponsManager.GetActiveWeapon();
            if(other.tag == "Enemy" && wp.isAttacking){
                if(other){
                    Damageable damageable = FindObjectOfType<Damageable>();
                    DebugUtility.HandleErrorIfNullFindObject<Damageable, ColliderDetector>(damageable,
                    this);
                    //Damageable damageable = other.GetComponent<Damageable>();
                    if (damageable)
                    {
                        Debug.Log("Aqui");
                        damageable.InflictDamage(1.0f, false, wp.Owner);
                    }
                    Debug.Log(wp.WeaponName);
                    Debug.Log(other.name);
                    //Health o_h = other.GetComponent<Health>();
                    //Debug.Log(o_h.CurrentHealth);
                }
            }
        }
        */
    }
}
