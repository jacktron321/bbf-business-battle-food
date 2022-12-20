using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    public class ColliderDetector : MonoBehaviour
    {
        bool iscolliding = false;
        Collider enemy;
        

        public void DetectHit(GameObject owner){
            //Debug.Log("Aqui");
            if(iscolliding){
                //Debug.Log("Chocando");
                if(enemy){
                    Debug.Log(enemy.name);
                    Damageable damageable = FindObjectOfType<Damageable>();
                    //Damageable damageable = enemy.GetComponent<Damageable>();
                    DebugUtility.HandleErrorIfNullFindObject<Damageable, ColliderDetector>(damageable,
                    this);
                    //Damageable damageable = other.GetComponent<Damageable>();
                    if (damageable)
                    {
                        Debug.Log(damageable.name);
                        //Debug.Log("Aqui");
                        damageable.InflictDamage(50.0f, false, owner);
                    }
                }
            }
        }
        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Enemy"){
                //Debug.Log("1");
                iscolliding = true;
                enemy = other;
            }
        }
        private void OnTriggerExit(Collider other) {
            if (other.tag == "Enemy"){
                //Debug.Log("2");
                iscolliding = false;
                enemy = null;
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
