using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.TestTools;

public class DoorController : MonoBehaviour
{
    Vector3 InitialPosition = new Vector3();
    public float maxDistance = 0f;
    public float OpenSpeed = 1f;
    public bool Opening = false;
    public bool Closing = false;
    void Awake(){
        InitialPosition = this.transform.position;
    }
    void Update(){
        if(Opening){
            this.transform.Translate (Vector3.forward * Time.deltaTime * OpenSpeed);
        }
        if(Closing){
            this.transform.Translate (Vector3.back * Time.deltaTime * OpenSpeed);
        }
        float diff = Mathf.Abs(Vector3.Distance(this.transform.position,InitialPosition));
        if(diff > maxDistance){
            Opening = false;
        }else if(0f <= diff && diff <= 0.1f){
            if(Closing){
                Closing = false;
                this.transform.position = InitialPosition;
            }
        }
    }

    public void Open(){
        Opening = true;
        Closing = false;
    }
    public void Close(){
        Closing = true;
        Opening = false;
    }
}
