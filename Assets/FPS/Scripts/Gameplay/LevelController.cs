using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public DoorController[] Doors;
    public void OpenDoor(int index){
        Doors[index].Open();
    }
}
