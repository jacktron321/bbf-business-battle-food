using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Unity.FPS.Gameplay
{
    public class GameObjective : Objective
    {
        GameFlowManager GM;
        public UnityAction<int> onChangeRoom;
        public bool test = false;
        public LevelManager levelMang;
        
        [Tooltip("Ammount of rooms to finish the game")]
        public int RoomsToCompleteObjective;
        public int CompleteRooms = 0;
        public int BossesToCompleteGame;
        public int DefeatedBosses = 0;
        int lastCompleteRooms;
        void Awake() {
            lastCompleteRooms = CompleteRooms;
            GM = FindObjectOfType<GameFlowManager>();
            DebugUtility.HandleErrorIfNullGetComponent<GameFlowManager, GameObjective>(GM, this,
                gameObject);
        }
        
        void Update(){
            if(lastCompleteRooms != CompleteRooms){
                lastCompleteRooms = CompleteRooms;
                onChangeRoom?.Invoke(lastCompleteRooms);
            }
            //if(CompleteRooms == RoomsToCompleteObjective) CompleteObjective(string.Empty, string.Empty, string.Empty);
            if(DefeatedBosses == BossesToCompleteGame) GM.CurrentState = "Complete";
            if(test){
                CompleteObjective(string.Empty, string.Empty, string.Empty);
            }
        }

        public void Finish(){
            CompleteObjective(string.Empty, string.Empty, string.Empty);
        }
    }
}