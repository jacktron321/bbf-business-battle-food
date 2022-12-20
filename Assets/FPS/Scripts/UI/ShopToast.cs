using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class ShopToast : MonoBehaviour
    {
        /*[Tooltip("Text for weapon name")]
        public TMPro.TextMeshProUGUI WeaponNameText;*/
        [Tooltip("Text for weapon price")]
        public TMPro.TextMeshProUGUI WeaponPriceText;
        [Tooltip("Will display icon")]
        public GameObject imagen;
        /*[Tooltip("Canvas used to fade in and out the content")]
        public CanvasGroup CanvasGroup;*/
        [Tooltip("How long it will stay visible")]

        public bool Initialized { get; private set; }

        /*public void Initialize(string name, int price, Sprite sprite)
        {
            WeaponNameText.text = name;
            WeaponPriceText.text = "$ "+price;
            imagen.GetComponent<Image>().sprite = sprite;
            //weaponSprite.sprite = sprite;

            // start the fade out
            Initialized = true;
        }*/
        public void Destroy()
        {
            Destroy(gameObject);
        }

        /*void Update()
        {
            if (Initialized)
            {
                
                // fade in
                CanvasGroup.alpha = 1f;
            }
            //CanvasGroup.alpha = 0f;
            //Destroy(gameObject);
        }*/
    }
}