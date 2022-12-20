using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace Unity.FPS.UI
{
    public class BuffHUD : MonoBehaviour
    {
        /*public class BuffInScreen{
            public GameObject BuffInHud;
            public string BuffString;
            public float TimeRemaining;
            //public BuffManager.Buff buff;
        }*/


        [Tooltip("UI panel containing the layoutGroup for displaying buffs")]
        public RectTransform BuffPanel;
        [Tooltip("UI panel containing the layoutGroup for displaying debuffs")]
        public RectTransform DebuffPanel;

        [Tooltip("Prefab for the notifications")]
        public GameObject BuffPrefab;
        public Sprite[] buff_sprites;

        public TextMeshProUGUI DmgText;
        public TextMeshProUGUI ATSText;
        public TextMeshProUGUI SpeedText;
        BuffManager buff_mang;
        
        //[Tooltip("Lista con instanciaciones")]
        //List<BuffInScreen> InHud = new List<BuffInScreen>();
        /*Dictionary<string,List<BuffInScreen>> InHudBuff = new Dictionary<string,List<BuffInScreen>>(){
            {"Regeneration",new List<BuffInScreen>()},
            {"Damage",new List<BuffInScreen>()},
            {"AttackSpeed",new List<BuffInScreen>()},
            {"Speed",new List<BuffInScreen>()},
        };*/
        Dictionary<string,Dictionary<string,(GameObject,float)>> InHudBuff = new Dictionary<string,Dictionary<string,(GameObject,float)>>(){
            {"Regeneration",new Dictionary<string,(GameObject,float)>()},
            {"Damage",new Dictionary<string,(GameObject,float)>()},
            {"AttackSpeed",new Dictionary<string,(GameObject,float)>()},
            {"Speed",new Dictionary<string,(GameObject,float)>()},
            {"FastDash",new Dictionary<string,(GameObject,float)>()},
        };
        /*Dictionary<string,List<BuffInScreen>> InHudDebuff = new Dictionary<string,List<BuffInScreen>>(){
            {"Damage",new List<BuffInScreen>()},
            {"AttackSpeed",new List<BuffInScreen>()},
            {"Speed",new List<BuffInScreen>()},
        };*/
        Dictionary<string,Dictionary<string,(GameObject,float)>> InHudDebuff = new Dictionary<string,Dictionary<string,(GameObject,float)>>(){
            {"Damage",new Dictionary<string,(GameObject,float)>()},
            {"AttackSpeed",new Dictionary<string,(GameObject,float)>()},
            {"Speed",new Dictionary<string,(GameObject,float)>()},
            {"Slippery",new Dictionary<string,(GameObject,float)>()},
            {"CSMoving",new Dictionary<string,(GameObject,float)>()},
        };

        

        void Start()
        {
            PlayerCharacterController playerCharacterController =
                GameObject.FindObjectOfType<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, BuffHUD>(
                playerCharacterController, this);

            buff_mang = playerCharacterController.GetComponent<BuffManager>();
            DebugUtility.HandleErrorIfNullGetComponent<BuffManager, BuffHUD>(buff_mang, this,
                playerCharacterController.gameObject);

            //buff_mang.OnActiveBuff += OnActiveBuff;
        }


        void Update()
        {
            if(buff_mang){
                /*CheckDict(buff_mang.BuffDict,InHudBuff,"Buff");
                CheckDict(buff_mang.DebuffDict,InHudDebuff,"Debuff");*/
                CheckDict(buff_mang.BuffDict,InHudBuff,BuffPanel,"◄");
                CheckDict(buff_mang.DebuffDict,InHudDebuff,DebuffPanel,"►");
                /*if(canshow){
                    canshow = false;
                    StartCoroutine(WaitShow());
                }*/
                
                //CheckBuffDict();
                /*BDMGList = buff_mang.BDMGList;
                if(BDMGList.Count != 0){
                    foreach (BuffManager.Buff buff in BDMGList){
                        if(InHud.Count == 0){
                            GameObject buffInstance = Instantiate(BuffPrefab, BuffPanel);
                            buffInstance.transform.SetSiblingIndex(0);

                            BuffToast toast = buffInstance.GetComponent<BuffToast>();
                            if (toast)
                            {
                                toast.Initialize(buff_sprites[1], buff.BTime);
                            }
                            Aux aux = new Aux();
                            aux.buffHud = buffInstance;
                            aux.Activebuff = buff;
                            InHud.Add(aux);
                        }
                        Debug.Log(buff.BMult+" "+buff.BTime);
                    }
                }*/
                //DmgText.text = "DMG\nx"+buff_mang.DmgMult;

                // Text displayed below the buffs and debuffs
                DmgText.text = "Daño\n";
                if(buff_mang.DmgMult > 1f){
                    DmgText.text += "<color=green>x"+buff_mang.DmgMult;
                }else if(buff_mang.DmgMult < 1f){
                    DmgText.text += "<color=red>x"+buff_mang.DmgMult;
                }else DmgText.text += "<color=white>x"+buff_mang.DmgMult;

                //ATSText.text = "ATS\nx"+buff_mang.ATSMult;
                ATSText.text = "Vel.Atq\n";
                if(buff_mang.ATSMult > 1f){
                    ATSText.text += "<color=green>x"+buff_mang.ATSMult;
                }else if(buff_mang.ATSMult < 1f){
                    ATSText.text += "<color=red>x"+buff_mang.ATSMult;
                }else ATSText.text += "<color=white>x"+buff_mang.ATSMult;

                //SpeedText.text = "Speed\nx"+buff_mang.SpeedMult;
                SpeedText.text = "Vel.\n";
                if(buff_mang.SpeedMult > 1f){
                    SpeedText.text += "<color=green>x"+buff_mang.SpeedMult;
                }else if(buff_mang.SpeedMult < 1f){
                    SpeedText.text += "<color=red>x"+buff_mang.SpeedMult;
                }else SpeedText.text += "<color=white>x"+buff_mang.SpeedMult;
            }
            //if(InHud.Count != 0) ShowList();
        }
        //void CheckDict(Dictionary<string,List<BuffManager.Buff>> dict, Dictionary<string,List<BuffInScreen>> hud, string bufftype){
        void CheckDict(Dictionary<string,List<BuffManager.Buff>> dict, Dictionary<string,Dictionary<string,(GameObject,float)>> hud,RectTransform Panel, string symbol){
            foreach (var (type,list) in dict){
                //if((list.Count != 0) && canshow){
                if((list.Count != 0)){ // Si es que hay elementos en la lista
                    //Debug.Log("Aqui 1");
                    //Debug.Log(type);
                    //if(!hud[type].BuffInHud){
                    if(hud[type].Count == 0){ // No hay ningun buff en pantalla
                        //Debug.Log("Aqui");
                        GameObject buffInstance;
                        string newbuffstring;
                        //Debug.Log("Aqui 2");
                        buffInstance = Instantiate(BuffPrefab, Panel);
                        if(type == "Regeneration") newbuffstring = GetNewBuffString(list[0].BMult/list[0].BTime,symbol,type);
                        else if(type == "CSMoving") newbuffstring = symbol;
                        else newbuffstring = GetNewBuffString(list[0].BMult,symbol,type);
                      
                        buffInstance.transform.SetSiblingIndex(0);

                        BuffToast toast = buffInstance.GetComponent<BuffToast>();


                        if (toast)
                        {   
                            //list.Sort((a,b) => a.BMult.CompareTo(b.BMult));
                            if(type == "Regeneration") toast.Initialize(buff_sprites[0],list[0].BTime,newbuffstring);
                            if(type == "Damage") toast.Initialize(buff_sprites[1],list[0].BTime,newbuffstring);
                            if(type == "AttackSpeed") toast.Initialize(buff_sprites[2],list[0].BTime,newbuffstring);
                            if(type == "Speed") toast.Initialize(buff_sprites[3],list[0].BTime,newbuffstring);
                            if(type == "Slippery") toast.Initialize(buff_sprites[4],list[0].BTime,newbuffstring);
                            if(type == "CSMoving") toast.Initialize(buff_sprites[5],list[0].BTime,newbuffstring);
                            if(type == "FastDash") toast.Initialize(buff_sprites[6],list[0].BTime,newbuffstring);
                        }
                        hud[type].Add(newbuffstring,(buffInstance,list[0].TimeRemaining));

                    }else { /// Si es que ya hay buff en pantalla y hay buffs en en la lista del tipo
                        //Debug.Log("Aqui 3");
                        Dictionary<string,(float,float)> bufftime = new Dictionary<string,(float,float)>(); // string ◄/► , (multiplicator,timeremaining)
                        foreach(BuffManager.Buff buff in list){
                            string newbuffstring;
                            //newbuffstring = GetNewBuffString(buff.BMult,bufftype,type);
                            if(type == "Regeneration") newbuffstring = GetNewBuffString(buff.BMult/buff.BTime,symbol,type);
                            else if(type == "CSMoving") newbuffstring = symbol;
                            else newbuffstring = GetNewBuffString(buff.BMult,symbol,type);
                            
                            if(symbol == "◄") { // Buff
                                if(bufftime.ContainsKey(newbuffstring)){
                                    if(bufftime[newbuffstring].Item1 < buff.BMult) bufftime[newbuffstring] = (buff.BMult,buff.TimeRemaining);
                                    else if ((bufftime[newbuffstring].Item1 == buff.BMult) && (bufftime[newbuffstring].Item1 < buff.TimeRemaining)) bufftime[newbuffstring] = (buff.BMult,buff.TimeRemaining);
                                }else{
                                    bufftime[newbuffstring] = (buff.BMult,buff.TimeRemaining);
                                }
                            }else{ // Debuff
                                if(bufftime.ContainsKey(newbuffstring)){
                                    if(bufftime[newbuffstring].Item1 > buff.BMult) bufftime[newbuffstring] = (buff.BMult,buff.TimeRemaining);
                                    else if ((bufftime[newbuffstring].Item1 == buff.BMult) && (bufftime[newbuffstring].Item1 < buff.TimeRemaining)) bufftime[newbuffstring] = (buff.BMult,buff.TimeRemaining);
                                }else{
                                    bufftime[newbuffstring] = (buff.BMult,buff.TimeRemaining);
                                }
                            }
                        }

                        foreach (var (buffstr,buffdata) in bufftime){ //(buffdata.Item1,buffdata.Item2) === (multiplicador,timeremaining)
                            if(hud[type].ContainsKey(buffstr)){
                                hud[type][buffstr] = (hud[type][buffstr].Item1,buffdata.Item2);
                                GameObject buffInstance = hud[type][buffstr].Item1;
                                if(buffInstance != null){
                                    BuffToast toast = buffInstance.GetComponent<BuffToast>();
                                    toast.UpdateToast(buffstr,buffdata.Item2);
                                }
                            }else{
                                GameObject buffInstance;
                                buffInstance = Instantiate(BuffPrefab, Panel);
                                buffInstance.transform.SetSiblingIndex(0);
                                BuffToast toast = buffInstance.GetComponent<BuffToast>();
                                if (toast)
                                {   
                                    if(type == "Regeneration") toast.Initialize(buff_sprites[0],buffdata.Item2,buffstr);
                                    if(type == "Damage") toast.Initialize(buff_sprites[1],buffdata.Item2,buffstr);
                                    if(type == "AttackSpeed") toast.Initialize(buff_sprites[2],buffdata.Item2,buffstr);
                                    if(type == "Speed") toast.Initialize(buff_sprites[3],buffdata.Item2,buffstr);
                                    if(type == "Slippery") toast.Initialize(buff_sprites[4],list[0].BTime,buffstr);
                                    if(type == "CSMoving") toast.Initialize(buff_sprites[5],list[0].BTime,buffstr);
                                    if(type == "FastDash") toast.Initialize(buff_sprites[6],list[0].BTime,buffstr);
                                }
                                hud[type].Add(buffstr,(buffInstance,buffdata.Item2));
                            }
                        }
                        //// Update HUD
                        if(!bufftime.ContainsKey(symbol)){
                            if(hud[type].ContainsKey(symbol)){
                                if(hud[type][symbol].Item1){
                                    BuffToast toast = hud[type][symbol].Item1.GetComponent<BuffToast>();
                                    if(toast) Destroy(toast);
                                    Destroy(hud[type][symbol].Item1);
                                }
                            }
                        }
                        if(!bufftime.ContainsKey(symbol+symbol)){
                            if(hud[type].ContainsKey(symbol+symbol)){
                                if(hud[type][symbol+symbol].Item1){
                                    BuffToast toast = hud[type][symbol+symbol].Item1.GetComponent<BuffToast>();
                                    if(toast) Destroy(toast);
                                    Destroy(hud[type][symbol+symbol].Item1);
                                }
                            }
                        }
                        if(!bufftime.ContainsKey(symbol+symbol+symbol)){
                            if(hud[type].ContainsKey(symbol+symbol+symbol)){
                                if(hud[type][symbol+symbol+symbol].Item1){
                                    BuffToast toast = hud[type][symbol+symbol+symbol].Item1.GetComponent<BuffToast>();
                                    if(toast) Destroy(toast);
                                    Destroy(hud[type][symbol+symbol+symbol].Item1);
                                }
                            }
                        }
                    }
                    //Debug.Log(string.Join(", ",list.ForEach(aux => ));
                    //list.ForEach(buff => Debug.Log(string.Join(", ",buff.BMult)));
                    //list.ForEach(buff => Debug.Log(string.Join(", ",buff.TimeRemaining)));
                //}else if((list.Count == 0) && canshow){
                }else if((list.Count == 0)){
                    //Debug.Log("Aqui 4");
                    //Debug.Log(type);
                    //Debug.Log("Aqui3");
                    //Debug.Log(InHud[type].BuffInHud);
                    if(hud[type].Count != 0){
                        //Debug.Log("Aqui 4");
                        foreach(var (buffstr,buffdata) in hud[type]){
                            if(buffdata.Item1){
                                BuffToast toast = buffdata.Item1.GetComponent<BuffToast>();
                                if(toast) Destroy(toast);
                                Destroy(buffdata.Item1);
                            }
                        }
                        hud[type].Clear();
                        //Debug.Log("Aqui4");
                    }
                }
            }            
        }

        string GetNewBuffString(float mult, string symbol, string bufftype){
            if(symbol == "◄"){// BUFFS
                if(bufftype != "Regeneration"){
                    if(mult <= 1.3f) return symbol;
                    else if (mult <= 1.6f) return (string)(symbol+symbol);
                    else if (mult <= 2.0f) return (string)(symbol+symbol+symbol);
                }else{
                    if(mult < 1f) return symbol;
                    else if (mult == 1f) return (string)(symbol+symbol);
                    else if (mult > 1f) return (string)(symbol+symbol+symbol);
                }
            }else{ // DEBUFFS
                if(bufftype != "Slippery"){
                    if(mult <= 0.5f) return (string)(symbol+symbol+symbol);
                    else if (mult <= 0.75f) return (string)(symbol+symbol);
                    else if (mult <= 0.9f) return symbol;
                }else{ /// Slippery
                    if(mult < 1.5f) return (string)(symbol+symbol+symbol);
                    else if (mult < 2.5f) return (string)(symbol+symbol);
                    else if (mult <= 15f) return symbol;
                }
            }
            return symbol;
        }

        /*IEnumerator WaitShow(){
            yield return new WaitForSeconds(1.0f);
            canshow = true;
        }*/
        /*
        IEnumerator WaitForDestroy(Aux aux){
            yield return new WaitForSeconds(2f);
            if(aux.buffHud!=null){
                BuffToast toast = aux.buffHud.GetComponent<BuffToast>();
                if(toast){
                    Debug.Log("Aqui");
                } else Debug.Log("Aqui no");
                toast.Destroy();
                InHud.Remove(aux);
            }
        }
        void ShowList(){
            foreach(Aux aux in InHud){
                Debug.Log(aux.Activebuff.BMult +" "+ aux.Activebuff.BTime);
                StartCoroutine(WaitForDestroy(aux));
            }
        }*/
        /*void OnActiveBuff(string buff_type, float time)
        {
            if (time != 0){
                if (buff_type == "Regen"){
                    CreateNotification(buff_sprites[0] , time);
                }
                else if (buff_type == "Dmg"){
                    CreateNotification(buff_sprites[1] , time);
                }
                else if (buff_type == "ATS"){
                    CreateNotification(buff_sprites[2] , time);
                }else if (buff_type == "Speed"){
                    CreateNotification(buff_sprites[3] , time);
                }
            }
        }*/

        public void CreateNotification(Sprite sprite, float time)
        {
            GameObject buffInstance = Instantiate(BuffPrefab, BuffPanel);
            buffInstance.transform.SetSiblingIndex(0);

            BuffToast toast = buffInstance.GetComponent<BuffToast>();
            if (toast)
            {
                //toast.Initialize(sprite, time);
            }
        }
    }
}