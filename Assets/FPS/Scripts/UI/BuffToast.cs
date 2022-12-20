using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace Unity.FPS.UI
{
    public class BuffToast : MonoBehaviour
    {
        [Tooltip("Time Left")]
        public TMPro.TextMeshProUGUI BuffText;
        [Tooltip("Will display icon")]
        public GameObject Buffimage;
        [Tooltip("Canvas used to fade in and out the content")]
        public CanvasGroup CanvasGroup;
        [Tooltip("How long it will stay visible")]

        public float TimeRemaining;
        public bool alternate = true;
        public float TickAlternate = 0.5f;
        public float MinTime = 7.0f;
        [Range(0.0f,1.0f)]
        public float minalpha = 0.5f;
        float newalpha = 1.0f;

        public bool Initialized { get; private set; }

        //float m_InitTime;
        //float m_Duration;

        public void Initialize(Sprite sprite, float buffTime, string bufftype)
        //public void Initialize(Sprite sprite, float buffTime)
        {
            TimeRemaining = buffTime;
            //this.transform.Rotate(0.0f,0.0f,90.0f, Space.Self);
            //TimeText.text = ""+buffTime;
            //TimeText.text = ""+buffTime;
            BuffText.text = bufftype;
            Buffimage.GetComponent<Image>().sprite = sprite;
            //weaponSprite.sprite = sprite;
            //m_InitTime = Time.time;
            //m_Duration = buffTime;
            

            // start the fade out
            Initialized = true;
        }
        public void UpdateToast(string str, float time){
            BuffText.text = str;
            TimeRemaining = time;
            if(TimeRemaining>MinTime) newalpha = 1f;
            //TimeText.text = ""+Math.Ceiling(time);
        }
        public void Destroy()
        {
            Destroy(gameObject);
        }

        void Update(){
            if(TimeRemaining<=MinTime && alternate){
                alternate = false;
                if(newalpha == 1.0f) newalpha = minalpha;
                else if(newalpha == minalpha) newalpha = 1.0f;
                Buffimage.GetComponent<Image>().color = new Color(255.0f,255.0f,255.0f,newalpha);
                StartCoroutine(RestartTime(TickAlternate));
            }else if(TimeRemaining > MinTime) {
                newalpha = 1.0f;
                Buffimage.GetComponent<Image>().color = new Color(255.0f,255.0f,255.0f,newalpha);
            }
        }
        IEnumerator RestartTime(float time){
            yield return new WaitForSeconds(time);
            alternate = true;
        }

        /*void Update()
        {
            if (Initialized)
            {
                float timeSinceInit = Time.time - m_InitTime;
                if (timeSinceInit > m_Duration){
                    Destroy(gameObject);
                }
                
                // fade in
                CanvasGroup.alpha = 1f;

                TimeText.text = ""+Math.Ceiling(m_Duration-timeSinceInit);
            }
            //CanvasGroup.alpha = 0f;
            //Destroy(gameObject);
        }*/
    }
}