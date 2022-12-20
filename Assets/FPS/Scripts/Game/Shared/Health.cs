using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Unity.FPS.Game
{
    public class Health : MonoBehaviour
    {
        [Tooltip("Maximum amount of health")] public float MaxHealth = 10f;
        [Tooltip("Maximum amount of shield")] public float MaxShield = 200f;
        [Tooltip("Initial Ammount of Shield (default = 0)")] public float InitShield = 0f;

        [Tooltip("Health ratio at which the critical health vignette starts appearing")]
        public float CriticalHealthRatio = 0.3f;

        public UnityAction<float, GameObject> OnDamaged;
        public UnityAction<float> OnHealed;
        public UnityAction<float> OnShielded;
        public UnityAction OnDie;

        public float CurrentHealth { get; set; }
        public float CurrentShield { get; set; }
        public bool Invincible { get; set; }
        public bool CanPickup() => CurrentHealth < MaxHealth;

        public float GetRatio() => CurrentHealth / MaxHealth;
        public bool IsCritical() => GetRatio() <= CriticalHealthRatio;

        bool m_IsDead;
        //bool isHealing = false;
        Coroutine co = null;
        double h_ratio = 0.05;

        public bool canBeHit = true;
       
        [Tooltip("Invulnerability time (sec) after hit (Only for Player)")]
        public float InvTime = 0f;

        void Start()
        {
            CurrentShield = InitShield;
            CurrentHealth = MaxHealth;
        }

        public void Heal(float healAmount)
        {
            float healthBefore = CurrentHealth;
            CurrentHealth += healAmount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            // call OnHeal action
            float trueHealAmount = CurrentHealth - healthBefore;
            if (trueHealAmount > 0f)
            {
                OnHealed?.Invoke(trueHealAmount);
            }
        }

        public void Shield(float shieldAmount)
        {
            float shieldBefore = CurrentShield;
            CurrentShield += shieldAmount;
            CurrentShield = Mathf.Clamp(CurrentShield, 0f, MaxShield);

            // call OnHeal action
            float trueShieldAmount = CurrentShield - shieldBefore;
            if (trueShieldAmount > 0f)
            {
                OnShielded?.Invoke(trueShieldAmount);
            }
        }

        public void RegenHealth(float healAmount, float healTime)
        {
            if(co == null){
                co = StartCoroutine(StartRegen(healAmount,healTime));
            }else{
                StopCoroutine(co);
                co = StartCoroutine(StartRegen(healAmount,healTime));
            }
            //InvokeRepeating("Heal",0.0f,1.0f);

        }

        private IEnumerator StartRegen(float healamount, float healtime){
            double init_time = 0;
            float healthBefore = 0;
            //Debug.Log("Ratio: "+((healamount/healtime)));
            while((float)init_time <= healtime){
                //Debug.Log("Curando");
                //Debug.Log(init_time);
                //Debug.Log(healtime);
                //init_time += Time.deltaTime;
                //yield return new WaitForSeconds(Time.deltaTime);
                
                healthBefore = CurrentHealth;
                //CurrentHealth += (healamount/healtime)*(float)h_ratio;
                CurrentHealth += Mathf.Round((healamount/healtime)*(float)h_ratio*1000f) / 1000f;;
                CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
                //Debug.Log(CurrentHealth);

                // call OnHeal action
                

                init_time += h_ratio;
                yield return new WaitForSeconds((float)h_ratio);
            }
            float trueHealAmount = CurrentHealth - healthBefore;
            //Debug.Log("Pre"+ CurrentHealth);
            CurrentHealth = (int)Mathf.Round(CurrentHealth);
            //Debug.Log("Post"+ CurrentHealth);
            if (trueHealAmount > 0f)
                {
                    OnHealed?.Invoke(trueHealAmount);
                }
            co = null;
        }

        public void TakeDamage(float damage, GameObject damageSource)
        {
            if (Invincible)
                return;
            if(CurrentShield == 0){
                if(canBeHit){
                    float healthBefore = CurrentHealth;
                    CurrentHealth -= damage;
                    CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

                    // call OnDamage action
                    float trueDamageAmount = healthBefore - CurrentHealth;
                    if (trueDamageAmount > 0f)
                    {
                        OnDamaged?.Invoke(trueDamageAmount, damageSource);
                    }

                    if(this.tag == "Player"){
                        canBeHit = false;
                        StartCoroutine(ResetHit());
                    }
                }
                
            } else if (CurrentShield != 0){
                if(CurrentShield >= damage){
                    float shieldBefore = CurrentShield;
                    CurrentShield -= damage;
                    CurrentShield = Mathf.Clamp(CurrentShield, 0f, MaxShield);

                    // call OnDamage action
                    float trueDamageAmount = shieldBefore - CurrentShield;
                    if (trueDamageAmount > 0f)
                    {
                        OnDamaged?.Invoke(trueDamageAmount, damageSource);
                    }
                }else{
                    damage = damage - CurrentShield;
                    CurrentShield = 0;
                    TakeDamage(damage, damageSource);
                }
            }
            HandleDeath();
        }
        IEnumerator ResetHit(){
            yield return new WaitForSeconds(InvTime);
            canBeHit = true;
        }

        public void Kill()
        {
            CurrentHealth = 0f;

            // call OnDamage action
            OnDamaged?.Invoke(MaxHealth, null);

            HandleDeath();
        }

        void HandleDeath()
        {
            if (m_IsDead)
                return;

            // call OnDie action
            if (CurrentHealth <= 0f)
            {
                m_IsDead = true;
                OnDie?.Invoke();
            }
        }
    }
}