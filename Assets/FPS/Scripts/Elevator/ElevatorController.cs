using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;

public class ElevatorController : MonoBehaviour
{
    // Start is called before the first frame update
    GameFlowManager GM;
    GameObjective GO;
    public float TimeScale;
    public float DIFF;
    public float MaxDiff = 2.2f;
    public bool StartClosed = false;
    public bool isIntro = false;
    public AudioSource ElevatorSource;
    public AudioSource[] ElevatorLifts;
    public AudioSource ElevatorSourceBell;
    public AudioClip IntroSong;
    public List<AudioClip> ElevatorSongs = new List<AudioClip>();
    public GameObject Door1;
    public GameObject Door2;
    public Collider InvisibleWall;
    public string State;
    public bool Opening = false;
    public bool Closing = false;
    bool hasFinished = false;

    Vector3 DefaultDoor1Init = new Vector3();
    Vector3 DefaultDoor2Init = new Vector3();
    ChatBox chatBox;
    bool hasInteracted = false;
    float DelayToShowText = 3f;
    //int startIndex = 0;
    public List<string> frases = new List<string>();

    
    float OpenSpeed = 1f;
    float CloseSpeed = 3f;
    [Tooltip("Time to open the doors with 'Elevator music'")]
    public float TimeToOpenDoor = 20f; // 5f

    void Awake() {
        if(StartClosed){
            State = "Closed";
            DoStartClosed();
        }
        else {
            State = "Open";
            InvisibleWall.enabled = false;
            DefaultDoor1Init = Door1.transform.position;
            DefaultDoor2Init = Door2.transform.position;
        }
        GM = FindObjectOfType<GameFlowManager>();
        DebugUtility.HandleErrorIfNullGetComponent<GameFlowManager, ElevatorController>(GM, this,
            gameObject);
        GM.CurrentState = "InElevator";
        GO = FindObjectOfType<GameObjective>();
        DebugUtility.HandleErrorIfNullGetComponent<GameObjective, ElevatorController>(GO, this,
            gameObject);

        //InvisibleWall.enabled = false;
        
        //Debug.Log(DefaultDoor1Init.x);
        chatBox = GetComponent<ChatBox>();
        if(!chatBox){
            chatBox = GetComponentInChildren<ChatBox>();
        }
        chatBox.DoneWriting += DoneWriting;
    }
    void DoneWriting(){
        if(GM.CurrentState != "Complete"){
            OpenDoors();
            ElevatorSource.volume = Mathf.Lerp(ElevatorSource.volume,0.1f,1f);
            //ElevatorSource.Stop();
            InvisibleWall.enabled =false;
        }
    }
    void DoStartClosed(){
        InvisibleWall.enabled = true;
        Door1.transform.Translate (Vector3.back*MaxDiff);
        Door2.transform.Translate (Vector3.forward *MaxDiff);
        DefaultDoor1Init = Door1.transform.position;
        DefaultDoor2Init = Door2.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        TimeScale = Time.timeScale;
        if(Time.timeScale == 0f){
            ElevatorSource.Pause();
            foreach(AudioSource elevLift in ElevatorLifts){
                elevLift.Pause();
            }
        }else{
            ElevatorSource.UnPause();
            foreach(AudioSource elevLift in ElevatorLifts){
                elevLift.UnPause();
            }
        }
        //if(State == "Open" && hasInteracted) CheckToClose();
        //if(Opening){
        if(Closing){
            Door1.transform.Translate (Vector3.back * Time.deltaTime * CloseSpeed);
            Door2.transform.Translate (Vector3.forward * Time.deltaTime * CloseSpeed);
        }
        //if(Closing){
        if(Opening){
            Door1.transform.Translate (Vector3.forward * Time.deltaTime * OpenSpeed);
            Door2.transform.Translate (Vector3.back * Time.deltaTime * OpenSpeed);
        }

        //float diff = Mathf.Abs(Door1.transform.position.x - DefaultDoor1Init.x);
        float diff = Mathf.Abs(Vector3.Distance(Door1.transform.position,DefaultDoor1Init));
        DIFF = diff;
        //if(diff != 0 ) Debug.Log(diff);
        
        if(StartClosed){ // Start with Closed Doors
            if(Opening && diff >MaxDiff){
                State = "Open";
                Opening = false;
                InvisibleWall.enabled =false;
            }else if(Closing && diff < 0.1f){
                State = "Closed";
                Closing = false;
                Door1.transform.position = DefaultDoor1Init;
                Door2.transform.position = DefaultDoor2Init;
            }
        }else{ // Start with Open Doors
            if(Closing && diff >MaxDiff){
                State = "Closed";
                Closing = false;
            }else if(Opening && diff < 0.1f){
                State = "Open";
                Opening = false;
                InvisibleWall.enabled =false;
                Door1.transform.position = DefaultDoor1Init;
                Door2.transform.position = DefaultDoor2Init;
            }
        }

        /*if((Opening || Closing) && diff >2.2f){ // 2.1f para Door1
            Debug.Log("Dif maxima");
            if(State == "Open") State = "Closed";
            else {
                State = "Open";
            }
            Opening = false;
            Closing = false;
            //State = "Open";
        }

        if((Closing || Opening) && (diff < 0f)){
            Debug.Log("Muy Cerca");
            if(State == "Open") State = "Closed";
            else State = "Open";
            Closing = false;
            Opening = false;
            Door1.transform.position = DefaultDoor1Init;
            Door2.transform.position = DefaultDoor2Init;
        }*/
        /*if((!Closing || !Opening) && diff == 0f){
            Door1.transform.position = DefaultDoor1Init;
            Door2.transform.position = DefaultDoor2Init;
        }*/
    }
    /*void CheckToClose(){
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player) {
            float dist = Vector3.Distance(InvisibleWall.transform.position, player.transform.position);
            //Debug.Log(dist);
            if(dist >= 10f){
                InvisibleWall.enabled = true;
                CloseDoors();
                ElevatorSource.Stop();
            }
        }

    }*/

    public void OpenDoors(){
        Opening = true;
        Closing = false;
    }
    public void CloseDoors(){
        InvisibleWall.enabled =true;
        Closing = true;
        Opening = false;
    }
    public void Interact(){
        if(!hasInteracted){
            if(!isIntro){ // Intro Elevator
                hasInteracted = true;
                //chatBox.NewText(frases,DelayToShowText);
                CloseDoors();
                InvisibleWall.enabled = true;
                var random = new System.Random();
                int index = random.Next(ElevatorSongs.Count);
                ElevatorSource.clip = ElevatorSongs[index];
                ElevatorSource.Play();
                chatBox.NewText(frases,2f);
                StartCoroutine(WaitToLift());
                StartCoroutine(WaitTo("Open"));
            }else{ // Start with Closed Doors
                hasInteracted = true;
                chatBox.NewText(frases,DelayToShowText);
                if(State == "Open"){
                    CloseDoors();
                    InvisibleWall.enabled = true;
                }
                ElevatorSource.clip = IntroSong;
                ElevatorSource.Play();
            }
        }
    }
    IEnumerator WaitToLift(){
        yield return new WaitForSeconds(2f);
        foreach(AudioSource ElevatorLift in ElevatorLifts){
            ElevatorLift.Play();
        }
    }

    IEnumerator WaitTo(string action){
        Debug.Log("WaitTo");
        yield return new WaitForSeconds(TimeToOpenDoor);
        //Debug.Log(GM.CurrentState);
        //Debug.Log(GM.CurrentState == "Complete");
        if(GM.CurrentState == "Complete"){
            ElevatorSource.volume = Mathf.Lerp(ElevatorSource.volume,0.1f,1f);
            GO.Finish();
        }else{
            if(action == "Open") OpenDoors();
            else if(action == "Close") CloseDoors();
            foreach(AudioSource ElevatorLift in ElevatorLifts){
                ElevatorLift.Stop();
            }
            ElevatorSourceBell.Play();
            ElevatorSource.volume = Mathf.Lerp(ElevatorSource.volume,0.1f,1f);
            //ElevatorSource.Stop();
            InvisibleWall.enabled =false;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player" && !hasFinished){
            //if(hasInteracted && State =="Open"){
            if(hasInteracted){
                hasFinished = true;
                InvisibleWall.enabled = true;
                CloseDoors();
                ElevatorSource.Stop();
                GM.CurrentState = "Tutorial";
            }
        }
    }
}
