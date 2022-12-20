using Unity.FPS.Game;
using UnityEngine;
using System.Collections.Generic;

namespace Unity.FPS.Gameplay
{
    public class WeaponGenerator : MonoBehaviour
    {
        public GameObject[] Pickups;
        List<GameObject> GeneratedPickups = new List<GameObject>(); 
        [Tooltip("The prefab for the weapon that will be added to the player on pickup")]
        public WeaponController WeaponPrefab;
        GameObject PickupToGenerate;
        public bool CanGenerate = true;
        float GenerateTime = 10f;
        public int ListLen;
        public float timer = 0f;

        void Awake(){
            ListLen = GeneratedPickups.Count;
            foreach(GameObject WpObj in Pickups){
                WeaponController Wp = WpObj.GetComponentInChildren<WeaponController>();
                if(Wp){
                    if(Wp.WeaponName == WeaponPrefab.WeaponName){
                        PickupToGenerate = WpObj;
                    }
                }
            }
        }

        void Update(){
            ListLen = GeneratedPickups.Count;
            if(CanGenerate){
                if(timer>= GenerateTime) timer = 0;
                if(timer == 0){
                    if(PickupToGenerate && (GeneratedPickups.Count == 0)){
                        GeneratedPickups.Add(Instantiate(PickupToGenerate,transform.position,Quaternion.identity));
                        WeaponPickup WpPick = GeneratedPickups[0].GetComponent<WeaponPickup>();
                        if(WpPick){
                            WpPick.OnDest += OnDest;
                        }
                        //timer+=Time.deltaTime;
                    }
                }else timer+=Time.deltaTime;
            }
        }
        void OnDest(){
            GeneratedPickups.Clear();
            timer+=Time.deltaTime;
        }
    }
}