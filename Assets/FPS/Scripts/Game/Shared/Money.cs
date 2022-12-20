using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    public class Money : MonoBehaviour
    {
        [Tooltip("Starting amount of money")] public float StatingMoney = 100f;
        public UnityAction<float> OnGetMoney;
        public UnityAction<float> OnSpend;
        public UnityAction OnDie;

        public float CurrentMoney { get; set; }
        public bool RichBoy { get; set; }

        bool m_IsDead;

        void Start()
        {
            CurrentMoney = StatingMoney;
        }

        public void AddMoney(float moneyAmmount)
        {
            float moneyBefore = CurrentMoney;
            CurrentMoney += moneyAmmount;
            // call OnGetMoney action
            float trueMoneyAmount = CurrentMoney - moneyBefore;
            if (trueMoneyAmount > 0f)
            {
                OnGetMoney?.Invoke(trueMoneyAmount);
            }
        }

        public void Spend(float ammount)
        {
            if (RichBoy)
                return;

            float moneyBefore = CurrentMoney;
            CurrentMoney -= ammount;
            //CurrentMoney = Mathf.Clamp(CurrentMoney, 0f, MaxHealth);

            // call OnDamage action
            float trueMoneyAmount = moneyBefore - CurrentMoney;
            if (trueMoneyAmount > 0f)
            {
                OnSpend?.Invoke(trueMoneyAmount);
            }

        }
    }
}