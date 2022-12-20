using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Unity.FPS.UI
{
    public class ShopHUD : MonoBehaviour
    {
        //public RectTransform ShopPanel;
        public RectTransform ShopPanel;
        [Tooltip("UI panel containing the layoutGroup for displaying notifications")]
        public GameObject MoneyMenu;
        public ShopToast Item;
        //[Tooltip("UI panel containing the layoutGroup for displaying notifications")]
        //public GameObject shopInBox;
        [Tooltip("Text for state of shop")]
        public TMPro.TextMeshProUGUI ShopStateText;
        public Image ShopFillImage;
        public GameObject shopInBox;

        Shop shop;
        float openpos;
        float closepos;

        //WeaponController current_weapon;

        void Start()
        {
            shop = FindObjectOfType<Shop>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, Shop>(shop,
                this);
            ShopFillImage.fillAmount = 0.0f;

            openpos = ShopPanel.anchoredPosition.y;
            closepos = ShopPanel.anchoredPosition.y-147.75f;
        }

        void Update()
        {
            if(shop.CanBeUsed) {
                ShopPanel.gameObject.SetActive(true);
                MoneyMenu.SetActive(true);
                }
            else {
                ShopPanel.gameObject.SetActive(false);
                MoneyMenu.SetActive(false);
            }
            if(shop.CanBeUsed){
                if(shop.isOpen){
                    if(!shop.inBox) ShopStateText.text = "Cerrar Tienda [C]";
                    ShopPanel.anchoredPosition = new Vector2(ShopPanel.anchoredPosition.x,openpos);
                    //ShopPanel.transform.position.Set(ShopPanel.transform.position.x,ShopPanel.transform.position.y+172f,ShopPanel.transform.position.z);
                    //ShopPanel.SetActive(true);
                    WeaponController WeaponinMenu = shop.WeaponList[shop.s_index];
                    Item.WeaponPriceText.text = "$ " + WeaponinMenu.weaponPrice;
                    Item.imagen.GetComponent<Image>().sprite = WeaponinMenu.WeaponIcon;
                    // (WeaponinMenu.WeaponName, WeaponinMenu.weaponPrice, WeaponinMenu.WeaponIcon);
                        
                    //WeaponController wantedWeapon = shop.GetComponent<WeaponController>();
                    //weapon.Sprite = wantedWeapon.WeaponIcon;
                }else {
                    if(!shop.isDelivering) ShopStateText.text = "Abrir Tienda [C]";
                    ShopPanel.anchoredPosition = new Vector2(ShopPanel.anchoredPosition.x,closepos);
                    //ShopPanel.transform.position.Set(ShopPanel.transform.position.x,ShopPanel.transform.position.y-172f,ShopPanel.transform.position.z);
                    //ShopPanel.SetActive(false);
                }

                if(shop.isDelivering){
                    ShopStateText.text = "En camino ...";
                    ShopFillImage.fillAmount = shop.currentTime / shop.deliveryTime;
                }
                else{
                    if(!shop.inBox){
                        ShopFillImage.fillAmount = 0.0f;
                        shopInBox.SetActive(false);
                    }
                    else {
                        shopInBox.SetActive(true);
                        ShopStateText.text = "Come un arma";
                    }
                }
            }
            
        }
    }
}