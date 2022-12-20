using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Unity.FPS.UI
{
    public class PlayerHealthBar : MonoBehaviour
    {
        public TextMeshProUGUI health_text;
        public TextMeshProUGUI shield_text;
        public GameObject Shield;
        public GameObject ShieldAux;
        Slider ShieldSlider;

        [Tooltip("Image component dispplaying current health")]
        public Image HealthFillImage;
        public Image StaminaFillImage;

        PlayerCharacterController playerCharacterController;
        Health m_PlayerHealth;
        private bool alternate = true;
        private float TickAlternate = 0.1f;
        int color = 1;

        //float bv = 0.16f;

        void Start()
        {
            playerCharacterController =
                GameObject.FindObjectOfType<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, PlayerHealthBar>(
                playerCharacterController, this);

            m_PlayerHealth = playerCharacterController.GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerHealthBar>(m_PlayerHealth, this,
                playerCharacterController.gameObject);
            
            ShieldSlider = Shield.GetComponent<Slider>();
            DebugUtility.HandleErrorIfNullFindObject<Slider, PlayerHealthBar>(
                ShieldSlider, this);
        }

        void Update()
        {
            // update health bar value
            if(m_PlayerHealth.CurrentShield != 0){
                Shield.SetActive(true);
                ShieldAux.SetActive(true);
                shield_text.text =  ((int)m_PlayerHealth.CurrentShield).ToString();
                ShieldSlider.value = m_PlayerHealth.CurrentShield/m_PlayerHealth.MaxShield;
                //ShieldSlider.value = m_PlayerHealth.CurrentShield/m_PlayerHealth.CurrentShield;

                //shield.text =  "+"+m_PlayerHealth.CurrentShield;
                //health.text =  (m_PlayerHealth.CurrentHealth).ToString() +"/"+m_PlayerHealth.MaxHealth.ToString(); // Exact Health
            }else {
                Shield.SetActive(false);
                ShieldAux.SetActive(false);
                //shield.text =  "";
                 // Aprox Health
                //health.text =  (m_PlayerHealth.CurrentHealth).ToString() +"/"+m_PlayerHealth.MaxHealth.ToString(); // Exact Health   
            }
            health_text.text =  ((int)m_PlayerHealth.CurrentHealth).ToString();
            HealthFillImage.fillAmount = m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth;
            if (playerCharacterController.dashCDTimer < 0)StaminaFillImage.fillAmount = 1f;
            else StaminaFillImage.fillAmount = (playerCharacterController.dashCD - playerCharacterController.dashCDTimer) / playerCharacterController.dashCD;
            
            if(!m_PlayerHealth.canBeHit){
                if(alternate){
                    alternate = false;
                    Color32 col = new Color32();
                    if(color == 1){
                        col = new Color32(255,170,180,255);
                        color = 0;
                    }else if(color == 0){
                        col = new Color32(255,35,56,255);
                        color = 1;
                    }
                    HealthFillImage.color = col;
                    StartCoroutine(RestartTime(TickAlternate));
                }
            }else HealthFillImage.color = new Color32(255,35,56,255);
        }
        IEnumerator RestartTime(float time){
            yield return new WaitForSeconds(time);
            alternate = true;
        }
    }
}