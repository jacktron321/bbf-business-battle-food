using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)){
            float interactRange = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray){
                if(collider.tag == "Button"){
                    ElevatorController elevator = collider.GetComponentInParent<ElevatorController>();
                    if (elevator){
                        elevator.Interact();
                    }
                    /*if(collider.TryGetComponent(out ElevatorController button)){
                        button.Interact();
                    }*/
                }
            }
        }
    }
}
