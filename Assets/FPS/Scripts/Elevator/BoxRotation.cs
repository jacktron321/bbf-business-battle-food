using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxRotation : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 aux;
    Transform player;
    float initialRotation;
    public float yRotation;
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj) player = playerObj.transform;
    }
    void Awake() {
        initialRotation = this.transform.rotation.y;
    }

    /*void Update(){
         Vector3 targetPostition = new Vector3( player.position.x, 
                                        this.transform.position.y, 
                                        player.position.z ) ;
        this.transform.LookAt( targetPostition ) ;
        //this.transform.rotation.y = initialRotation;
        //yRotation = this.transform.rotation.y;
        //if(this.transform.rotation.y < )
    }   
    /*void Update()
    {   //277 264
        //if(player.rotation.y <= maxRotation && player.rotation.y >= -maxRotation) transform.rotation = player.rotation;
        Vector3 distToPlayer = player.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(distToPlayer), Time.deltaTime*10f);
        float zAngle = transform.eulerAngles.y;
        //if( zAngle > 277 && zAngle <= 180){
        if( zAngle > 277){
            zAngle = 277;
        }
        //else if (zAngle < 260 && zAngle > 180){
        else if (zAngle < 264){
            zAngle = 264;
        }
        Debug.Log(transform.eulerAngles);
        transform.eulerAngles = new Vector3(0,zAngle,0);
        //transform.rotation = player.rotation;
    }*/
}
