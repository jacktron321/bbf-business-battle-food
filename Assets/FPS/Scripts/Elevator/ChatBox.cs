using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ChatBox : MonoBehaviour
{
    // Start is called before the first frame update
    public UnityAction DoneWriting;
    public TextMeshProUGUI BubbleText;
    public GameObject bubble;
    public AudioSource typingSource;
    [Tooltip ("Ammount of letters per second")]
    public float speed = 20f;
    public bool isWriting = false;
    bool cantType = true;
    List<string> textToWrite;
    int textIndex = 0;
    int textIndex2 = 0;
    void Start() {
        BubbleText.text = "";
        bubble.SetActive(false);    
    }

    public void NewText(List<string> text,float delay){
        textIndex = 0;
        textIndex2 = 0;
        if(delay == 0f){
            BubbleText.text = "";
            isWriting = true;
            cantType = true;
            textToWrite = text;
            bubble.SetActive(true);
            typingSource.Play();
        }else StartCoroutine(WaitToStart(text,delay));
        /*foreach(char ch in text){
            BubbleText.text += ch;
            yield return new WaitForSeconds(1f/speed);
            StartCoroutine(WaitforNewLetter());
        }*/
    }
    public void StopText(){ // Only if want to interrupt
        BubbleText.text = "";
        isWriting = false;
        cantType = false;
        textToWrite = new List<string>();
        bubble.SetActive(false);
        textIndex = -1;
        textIndex2 = -1;
        typingSource.Stop();
        StopAllCoroutines();
    }
    
    void Update() {
        if(Time.timeScale == 0f) typingSource.Pause();
        else typingSource.UnPause();

        if(isWriting){
            if(textIndex == textToWrite.Count) {
                typingSource.Stop();
                isWriting = false;
                textIndex = 0;
                cantType = false;
                HideBubble();
                //StartCoroutine(WaitToHide());
            }
            
            if(cantType){
                if(textIndex != -1 && textIndex2 != -1){
                    BubbleText.text += textToWrite[textIndex][textIndex2];
                    textIndex2++;
                    if(textIndex2 == textToWrite[textIndex].Length){
                        textIndex2 = 0;
                        cantType = false;
                        typingSource.Stop();
                        StartCoroutine(WaitToContinue());
                    }else {
                        cantType = false;
                        StartCoroutine(WaitforNewLetter());
                    }
                }
            }
        }    
    }
    IEnumerator WaitToContinue(){
        yield return new WaitForSeconds(3f);
        BubbleText.text = "";
        textIndex++;
        cantType = true;
        typingSource.Play();
    }
    IEnumerator WaitToHide(){
        yield return new WaitForSeconds(2f);
        BubbleText.text = "";
        bubble.SetActive(false);
        DoneWriting?.Invoke();
    }
    void HideBubble(){
        BubbleText.text = "";
        bubble.SetActive(false);
        DoneWriting?.Invoke();
    }
    IEnumerator WaitToStart(List<string> text, float delay){
        yield return new WaitForSeconds(delay);
        BubbleText.text = "";
        isWriting = true;
        cantType = true;
        textToWrite = text;
        bubble.SetActive(true);
        typingSource.Play();
    }
    IEnumerator WaitforNewLetter(){
        yield return new WaitForSeconds(1f/speed);
        cantType = true;
    }
}
