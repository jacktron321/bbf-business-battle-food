using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;

namespace Unity.FPS.Gameplay
{
    public class ColliderDetector : MonoBehaviour
    {
        PlayerWeaponsManager m_WeaponsManager;
        PlayerCharacterController m_PlayerCharacterController;

        void Awake() {
            m_PlayerCharacterController = FindObjectOfType<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, ColliderDetector>(m_PlayerCharacterController,
                this);

            m_WeaponsManager = m_PlayerCharacterController.GetComponent<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerWeaponsManager, ColliderDetector>(m_WeaponsManager, this, gameObject);
        }
        private void OnTriggerEnter(Collider other) {
            //Debug.Log("Enemy");
            //Debug.Log(other.name +" and " + this.name);
            //Debug.Log(other.tag +" and " + this.tag);
            if ((other.tag == "Weapon" || other.tag == "Pineapple") || other.tag == "Enemy"){
                if(this.tag != "Player"){ // Damage dealt to enemies
                    //Debug.Log("Aqui 1");
                    Damageable damageable = GetComponent<Damageable>();
                    DebugUtility.HandleErrorIfNullFindObject<Damageable, ColliderDetector>(damageable,
                    this);
                    if (damageable)
                    {
                        //Debug.Log("Aqui");
                        WeaponController wp = m_WeaponsManager.GetActiveWeapon();
                        if(wp){
                            
                            //Debug.Log(wp.name);
                            //Debug.Log(wp.tag);
                            if(wp.tag == "Pineapple") {
                                Health enem_h = damageable.GetComponentInParent<Health>();
                                if(enem_h){
                                    if(enem_h.CurrentShield != 0){
                                        damageable.InflictDamage(wp.dmgAmount*1.5f, false, m_PlayerCharacterController.gameObject);
                                        return;
                                    }
                                }
                                damageable.InflictDamage(wp.dmgAmount, false, m_PlayerCharacterController.gameObject);
                                //damageable.InflictDamage(wp.dmgAmount, false, m_PlayerCharacterController.gameObject);
                            }else damageable.InflictDamage(wp.dmgAmount, false, m_PlayerCharacterController.gameObject);
                            //StartCoroutine(waithit());
                            //damageable.InflictDamage(1.0f, false, other.gameObject);
                        }
                    }
                }else if(other.tag == "Enemy" && this.tag == "Player"){ //// Dmg dealt to the player
                    //Debug.Log("Aqui 2");
                    Damageable damageable = GetComponent<Damageable>();
                    DebugUtility.HandleErrorIfNullFindObject<Damageable, ColliderDetector>(damageable,
                    this);
                    if (damageable)
                    {
                        //Debug.Log("Aqui");
                        WeaponController wp = other.GetComponent<WeaponController>();
                        if(!wp){
                            wp = other.GetComponentInChildren<WeaponController>();
                        }
                        DebugUtility.HandleErrorIfNullFindObject<Damageable, ColliderDetector>(damageable,
                        this);
                        if(wp){
                            damageable.InflictDamage(wp.dmgAmount, false, other.gameObject);
                            //StartCoroutine(waithit());
                            //damageable.InflictDamage(1.0f, false, other.gameObject);
                        }
                    }
                }
            }
        }

        /*private void OnControllerColliderHit(ControllerColliderHit hit) {
            Debug.Log(hit);
        }*/
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
