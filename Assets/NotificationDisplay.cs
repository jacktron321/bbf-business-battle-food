using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationDisplay : MonoBehaviour
{
    public Text ts;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clickButton(){
        StartCoroutine(sendNotification("You clicked the button!",3));
    }
    IEnumerator sendNotification(string text, int time){
        ts.text = text;
        yield return new WaitForSeconds(time);
        ts.text = "";
    }
}
