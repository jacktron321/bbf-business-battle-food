using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class WorldspaceHealthBar : MonoBehaviour
    {
        [Tooltip("Health component to track")] public Health Health;

        [Tooltip("Image component displaying health left")]
        public Image HealthBarImage;
        [Tooltip("Image component displaying shield left")]
        public Image ShieldBarImage;

        [Tooltip("The floating healthbar pivot transform")]
        public Transform HealthBarPivot;

        [Tooltip("Whether the health bar is visible when at full health or not")]
        public bool HideFullHealthBar = true;

        void Update()
        {
            // update health bar value
            HealthBarImage.fillAmount = Health.CurrentHealth / Health.MaxHealth;
            //if(Health.CurrentShield != 0){
                //ShieldBarImage.gameObject.SetActive(true);
            ShieldBarImage.fillAmount = Health.CurrentShield / Health.MaxShield;
            //}
            //HealthBarPivot.gameObject.SetActive(ShieldBarImage.fillAmount != 1);

            // rotate health bar to face the camera/player
            HealthBarPivot.LookAt(Camera.main.transform.position);

            // hide health bar if needed
            if (HideFullHealthBar)
                HealthBarPivot.gameObject.SetActive(HealthBarImage.fillAmount != 1 || (Health.CurrentShield != Health.InitShield));
                //HealthBarPivot.gameObject.SetActive(Health.CurrentShield != Health.InitShield);
        }
    }
}