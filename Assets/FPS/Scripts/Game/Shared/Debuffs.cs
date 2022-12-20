using Unity.FPS.Game;
using UnityEngine;
using System.Collections;

namespace Unity.FPS.Game
{
    [CreateAssetMenu(fileName = "New Debuff", menuName = "Debuff")]
    public class Debuffs : ScriptableObject
    {
        [System.Serializable]
        public class DSpecial{
            [Tooltip("0: Slippery\n1: Can't Stop Moving")] [Range(0, 1)]
            public int DIndex;
            public float DDelay;
            public float DTime;
        }
        public float Dmg;
        public float DmgDelay;
        public float DmgTime;
        public float AtkSpeed;
        public float ASDelay;
        public float ASTime;
        public float Speed;
        public float SpeedDelay;
        public float SpeedTime;
        [Tooltip("Ammount of special debuffs with Index and Delay Time")]
        public DSpecial[] Special;
    }
}