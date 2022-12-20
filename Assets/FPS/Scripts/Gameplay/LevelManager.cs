using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    
    public GameObject Level1;
    public GameObject Level2;
    public GameObject InstanceOfLevel1;
    // Start is called before the first frame update
    void Start()
    {
        InstanceOfLevel1 = Instantiate(Level1,new Vector3(-55.59f,0f,-25.60f), (Quaternion.identity));
        
    }
}
