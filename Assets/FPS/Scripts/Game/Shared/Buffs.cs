using Unity.FPS.Game;
using UnityEngine;
using System.Collections;

namespace Unity.FPS.Game
{
    [CreateAssetMenu(fileName = "New Buff", menuName = "Buff")]
    public class Buffs : ScriptableObject
    {
        [System.Serializable]
        public class BSpecial{
            [Tooltip("0: Cleans debuffs\n1: Fast Dash")] [Range(0, 1)]
            public int BIndex;
            public float Bmult;
            public float BTime;
        }
        public float HealthAmmount;
        public float ShieldAmmount;
        public float HealthRegen;
        public float RegenTime;
        public float Dmg;
        public float DmgTime;
        public float AtkSpeed;
        public float ASTime;
        public float Speed;
        public float SpeedTime;
        [Tooltip("Ammount of special buffs with Index and Delay Time")]
        public BSpecial[] Special;
    }
}