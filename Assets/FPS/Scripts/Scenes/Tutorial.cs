using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using Unity.FPS.AI;

public class Tutorial : MonoBehaviour
{
    public bool Typing = false;
    public Collider InvWall;
    GameFlowManager GM;
    GameObjective GO;
    Collider Detector;
    PlayerWeaponsManager weaponMang;
    Shop shop;
    public ChatBox chatBox;
    public DoorController door;
    public WeaponPickup weaponPickup;
    public EnemyController Enemy;
    int chatBoxState = 0; // 0: None 1:TalkingOfWeapons 2:TalkAboutEnemy 3:TalkAboutShop 4:EnemyKill 5:Done
    bool canType = true;
    bool finishShopText = false;
    bool defeatedEnemy = false;
    bool auxShop = true;
    public List<string> forWeapon = new List<string>(){
        {"En este sueño va a pelear contra conceptualizaciones de los problemas que lo rodea."},
        {"Por lo que va a necesitar un arma como la que tiene en el rincon a su derecha. "},
    };
    public List<string> forEnemy = new List<string>(){
        {"Al parecer ve la comida como un apoyo que lo mantiene día a día, por eso las transformó en armas."},
        {".   .   ."},
        {"Por lo que veo sus compañeros de trabajo dejan mucho que desear si los imagina como sus enemigos."},
        //{"Por lo que para completar esta simulacion tendrá que derrotarlos a todos."},
        {"Como por ejemplo, el que aparecerá en el rincon de su izquierda."},

    };
    public List<string> forShop = new List<string>(){
        {"Cuando se quede sin municion, puede consumir las armas."},
        {"Consumirlas otorga vida y distintas mejoras."},
        {"Si se queda sin arma puede pedirlas con la aplicación celular rappidin gastando el dinero obtenido al derrotar enemigos."},
        {"Siempre puedes comprar Maíz, ya que es gratis."},
    };
    public List<string> forFinishRoom = new List<string>(){
        {"Cuando derrotas a todos los enemigos, se abre la siguiente sala."},
        {"Para terminar la simulacion, debes derrotar a todos los... jefes?? *pfff* JAJA-"},
    };

    void Start()
    {
        Detector = this.GetComponent<Collider>();
        Detector.enabled = false;
        GM = FindObjectOfType<GameFlowManager>();
        DebugUtility.HandleErrorIfNullGetComponent<GameFlowManager, Tutorial>(GM, this,
            gameObject);
        GO = FindObjectOfType<GameObjective>();
        DebugUtility.HandleErrorIfNullGetComponent<GameObjective, Tutorial>(GO, this,
            gameObject);
        weaponMang = FindObjectOfType<PlayerWeaponsManager>();
        DebugUtility.HandleErrorIfNullGetComponent<PlayerWeaponsManager, Tutorial>(weaponMang, this,
            gameObject);
        shop = FindObjectOfType<Shop>();
        DebugUtility.HandleErrorIfNullFindObject<Shop, Tutorial>(shop,
            this);
        chatBox.DoneWriting += DoneWriting;
        weaponPickup.OnDest += GotWeapon;
        Enemy.onDefeated += EnemyDefeated; 
    }
    void Update(){
        if(finishShopText && defeatedEnemy) {
            defeatedEnemy = false;
            //Debug.Log("Terminado shop y Derrotado Enemigo");
            chatBoxState = 4;
            canType = true;
        } 
        WeaponController mainWeap = weaponMang.GetActiveWeapon();
        if(chatBoxState == 3){
            if(mainWeap){
                mainWeap.noAmmo += NoAmmo;
            }else NoAmmo();
        }
        if(GM.CurrentState == "Tutorial" && canType){
            Detector.enabled = true;
            //Debug.Log("Aqui");
            if(chatBoxState == 4 && finishShopText){
                chatBox.NewText(forFinishRoom,0.2f);
                canType = false;
            }else if(chatBoxState == 3 && auxShop){
                auxShop = false;
                shop.CanBeUsed = true;
                chatBox.NewText(forShop,0.2f);
                canType = false;
            }else if(chatBoxState == 2){
                chatBox.NewText(forEnemy,0.2f);
                canType = false;
            }else if(chatBoxState == 1){
                chatBox.NewText(forWeapon,0.2f);
                Typing = true;
                canType = false;
            }else if(chatBoxState == 0) chatBoxState = 1;
            
            
            
            
        }
    }
    void DoneWriting(){
        if(chatBoxState == 4){
            door.Open();
            chatBoxState = 5;
        }else if(chatBoxState == 3){
            //Debug.Log("Finished Shop Text");
            finishShopText = true;
        }else if(chatBoxState == 2){
            Enemy.gameObject.SetActive(true);
            chatBoxState = 3;
        }else if(chatBoxState == 1){ // Talkin of Weapon
            weaponPickup.gameObject.SetActive(true);
        }
    }   
    void GotWeapon(){
        chatBoxState = 2;
        canType = true;
    }
    void NoAmmo(){
        canType = true;
    }
    void EnemyDefeated(){
        //Debug.Log("Derrotado Enemigo");
        //chatBoxState = 4;
        defeatedEnemy = true;
        //canType = true;
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player"){
            InvWall.enabled = true;
            Detector.enabled = false;
            Debug.Log(other.name);
            Debug.Log("Salido Sala Tutorial");
            door.Close();
            GM.CurrentState = "Rooms";
            GO.CompleteRooms +=1;
        }
    }
}
