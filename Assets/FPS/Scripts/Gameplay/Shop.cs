using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Unity.FPS.Gameplay
{
    public class Shop : MonoBehaviour
    {
        [Tooltip("Time to deliver the item")]
        public float deliveryTime = 5.0f;
        public float currentTime = 0.0f;
        public bool CanBeUsed = false;

        public bool isDelivering = false;
        int[] w_shop = new int[2];
        public bool isOpen = false;
        public bool inBox = false;
        //public PlayerCharacterController m_PlayerCharacterController;
        PlayerInputHandler m_InputHandler;
        PlayerWeaponsManager m_WeaponsManager;
        Money m_Money;
        public int s_index;
        public WeaponController[] WeaponList;
        public static WeaponController wantedWeapon;

        void Awake()
        {
            PlayerCharacterController m_PlayerCharacterController = FindObjectOfType<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, Shop>(m_PlayerCharacterController,
                this);
            
            m_InputHandler = m_PlayerCharacterController.GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, Shop>(m_InputHandler, this, gameObject);

            m_Money = m_PlayerCharacterController.GetComponent<Money>();
            DebugUtility.HandleErrorIfNullGetComponent<Money, Shop>(m_Money, this, gameObject);

            m_WeaponsManager = m_PlayerCharacterController.GetComponent<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerWeaponsManager, Shop>(m_WeaponsManager, this, gameObject);
            
        }
        void Start() {
            s_index = 0;
            
            //Debug.Log(m_PlayerCharacterController.IsDead);
        }
        void Update() {
            if(CanBeUsed){
                if(inBox){
                    //bool confirm = m_InputHandler.GetConfirmButtonDown();
                    bool confirm = m_WeaponsManager.CheckWeaponList();
                    if(confirm){
                        HandleBuyWeapon(wantedWeapon);
                    }
                }
                // Open and close Shop with same Key
                if(m_InputHandler.GetShopButtonDown() && !isDelivering && !inBox){
                //if(!isOpen) Debug.Log("Tienda abierta"); else Debug.Log("Tienda cerrada");
                    isOpen = !isOpen; // Alter Shop state
                    //Debug.Log(WeaponList[s_index].WeaponName);
                }
                
                if(isOpen){
                    //Debug.Log(m_Money.CurrentMoney);
                    //Debug.Log(m_Weapons.m_WeaponSlots[0].WeaponName);
                    HandleIndex();
                    HandleShopping();

                } else {
                    s_index = 0;
                }
                
                if(isDelivering){
                    currentTime += Time.deltaTime;
                    //Debug.Log(currentTime);
                    if(currentTime >= deliveryTime){
                        isDelivering = false;
                        currentTime = 0;
                        HandleBuyWeapon(wantedWeapon);
                    }
                }
            }
        }
        
        public bool HandleIndex(){
            int dir = m_InputHandler.GetSwitchWeaponInput();
            // Handle Scrollwheel to move in shop
            if(dir != 0){
                //WeaponController weapon_i = m_WeaponsManager.GetWeaponAtSlotIndex(1);
                //if(weapon_i != null) Debug.Log(weapon_i.WeaponName);
                bool ford = dir > 0;
                //if (ford) Debug.Log("Abajo"); else Debug.Log("Arriba");
                s_index += dir; //Abajo +1 , Arriba -1

                // Handle index between range
                if(s_index >= WeaponList.Length) {
                    s_index = 0; 
                }else if (s_index < 0) s_index = (WeaponList.Length-1);

                //Debug.Log(WeaponList[s_index].WeaponName);
                return true;
            }
            return false;
        }
        public int CorrectIndex(int indx){

            // Handle index between range
            if(indx >= WeaponList.Length) {
                return 0; 
            }else if (indx < 0) {
                return (WeaponList.Length-1);
            }
            return indx;
        }

        void HandleShopping(){
            bool confirm = m_InputHandler.GetConfirmButtonDown();// Check confirm button
            if(confirm){
                wantedWeapon = WeaponList[s_index];
                if(m_Money.CurrentMoney >= wantedWeapon.weaponPrice){
                    isDelivering = true;
                    m_Money.Spend(wantedWeapon.weaponPrice);
                    //Debug.Log(wantedWeapon.WeaponName+" is in the way");
                    isOpen = false;
                    s_index = 0;
                }
            }
        }
        
        void HandleBuyWeapon(WeaponController wantedWeapon){
            if(wantedWeapon){
                if(m_WeaponsManager.AddWeapon(wantedWeapon)){
                    //Debug.Log("You recieved "+wantedWeapon.WeaponName);
                    // Handle auto-switching to weapon if no weapons currently
                    if (m_WeaponsManager.GetActiveWeapon() == null)
                    {
                        m_WeaponsManager.SwitchWeapon(true);
                    }
                    inBox = false;
                } else {
                    //Debug.Log("You need space in your inventory");
                    inBox = true;
                }
            }
        }
    }
}