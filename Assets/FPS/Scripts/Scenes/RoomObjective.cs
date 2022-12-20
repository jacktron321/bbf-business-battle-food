using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using Unity.FPS.AI;

public class RoomObjective : MonoBehaviour
{
    Collider Detector;
    public bool isBoosRoom = false;
    [Tooltip("If the trigger is inside the room, else it is in the exit")]
    public bool triggerInRoom = true;
    public int roomIndex;
    GameObjective GO;
    [Tooltip("You can draw all the enemies of the room or put the exact ammount in 'Ammount Of Enemies'")]
    public List<EnemyController> Enemies = new List<EnemyController>();
    public int AmmountOfEnemies = 0;

    public DoorController Door;
    public ElevatorController Elevator;
    public Collider InvWall;

    void Start(){
        //if(Elevator) Elevator.CloseDoors();
        Detector = this.GetComponent<Collider>();
        if(Detector) Detector.enabled = false;
        GO = FindObjectOfType<GameObjective>();
        DebugUtility.HandleErrorIfNullGetComponent<GameObjective, RoomObjective>(GO, this,
            gameObject);
        foreach(EnemyController enemy in Enemies){
            enemy.onDefeated += DefeatedEnemy;
        }
        GO.onChangeRoom += ActiveRoom;
        AmmountOfEnemies = Enemies.Count;
    }

    void DefeatedEnemy(){
        AmmountOfEnemies-=1;
        if(AmmountOfEnemies == 0){
            Debug.Log("Finished room of index "+roomIndex);
            if(Door) Door.Open();
            if(Elevator) Elevator.OpenDoors();
        }
        if((AmmountOfEnemies == 0) && isBoosRoom){
            GO.DefeatedBosses += 1;
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player" && triggerInRoom){
            GO.CompleteRooms +=1;
            if(Detector) Detector.enabled = false;
            if(Door) Door.Close();
            if(InvWall) InvWall.enabled = true;
        }
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player" && !triggerInRoom){
            GO.CompleteRooms +=1;
            if(Detector) Detector.enabled = false;
            if(Door) Door.Close();
            if(InvWall) InvWall.enabled = true;
        }
    }

    void ActiveRoom(int Index){
        if(roomIndex == Index){
            if(Detector) Detector.enabled = true;
            Debug.Log("Active Room of index "+roomIndex);
            foreach(EnemyController enemy in Enemies){
                enemy.gameObject.SetActive(true);
            }
            if(AmmountOfEnemies == 0){
                Debug.Log("Finished room of index "+roomIndex);
                if(Door) Door.Open();
            }
        }
    }
}