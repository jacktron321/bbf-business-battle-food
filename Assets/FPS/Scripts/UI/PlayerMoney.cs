using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Unity.FPS.UI
{
    public class PlayerMoney : MonoBehaviour
    {
        public TextMeshProUGUI money;

        Money m_PlayerMoney;

        void Start()
        {
            PlayerCharacterController playerCharacterController =
                GameObject.FindObjectOfType<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, PlayerMoney>(
                playerCharacterController, this);

            m_PlayerMoney = playerCharacterController.GetComponent<Money>();
            DebugUtility.HandleErrorIfNullGetComponent<Money, PlayerMoney>(m_PlayerMoney, this,
                playerCharacterController.gameObject);
        }

        void Update()
        {
            // update health bar value
            money.text = "$ "+m_PlayerMoney.CurrentMoney.ToString();
        }
    }
}