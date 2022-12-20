using System.Collections.Generic;
using System.Collections;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace Unity.FPS.Gameplay
{
    public class BuffManager : MonoBehaviour
    {
        public class Buff{
            public float BMult;
            public float BTime;
            public float BInit;
            public float TimeRemaining;
        }
        //public UnityAction<string, float> OnActiveBuff;
        PlayerCharacterController m_PlayerCharacterController;
        Health p_health;
        public Dictionary<string,List<Buff>> BuffDict = new Dictionary<string, List<Buff>>(){
            {"Regeneration",new List<Buff>()},
            {"Damage",new List<Buff>()},
            {"AttackSpeed",new List<Buff>()},
            {"Speed",new List<Buff>()},
            {"FastDash",new List<Buff>()},
        };
        public Dictionary<string,List<Buff>> DebuffDict = new Dictionary<string, List<Buff>>(){
            {"Damage",new List<Buff>()},
            {"AttackSpeed",new List<Buff>()},
            {"Speed",new List<Buff>()},
            {"Slippery",new List<Buff>()},
            {"CSMoving",new List<Buff>()},
        };
        ////// Damage Control        
        public float DmgMult = 1.0f;
        public float BDmg = 1.0f;
        public float DDmg = 1.0f;

        //// Atack Speed Control
        public float ATSMult = 1.0f;
        public float BATS = 1.0f;
        public float DATS = 1.0f;

        ///// Speed Control
        public float SpeedMult = 1.0f;
        public float BSpeed = 1.0f;
        public float DSpeed = 1.0f;

        ///////////// Special Control
        public float fastDash;
        public float BaseSlippery;
        public float Slippery;
        //List<Buff> SlipperyList = new List<Buff>();
        /////////////

        public float PlayerVel;

        //public bool isHealing = false;
        //public bool hasHealed = false;
        //public bool hasShielded = false;
        public Vector3 m_PlayerVel;

        public bool auxiliar = false;
        public float auxiliartime = 0;
        //Coroutine coDMG;


        void Start() {
            DmgMult = BDmg * DDmg;
            ATSMult = BATS * DATS;
            SpeedMult = BSpeed * DSpeed;

            m_PlayerCharacterController = GetComponent<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerCharacterController, BuffManager>(
                m_PlayerCharacterController, this, gameObject);
            

            BaseSlippery = m_PlayerCharacterController.MovementSharpnessOnGround;
            Slippery = m_PlayerCharacterController.MovementSharpnessOnGround;
            fastDash = 1f;
            
            p_health = m_PlayerCharacterController.GetComponent<Health>();

            //PSpeed = m_PlayerCharacterController.MaxSpeedOnGround;
            m_PlayerVel = m_PlayerCharacterController.CharacterVelocity;
            PlayerVel = m_PlayerVel.magnitude;
        }

        void Update(){
            DmgMult = BDmg * DDmg; // Damage update
            ATSMult = BATS * DATS; // Attack Speed update
            SpeedMult = BSpeed * DSpeed; // Speed update
            /// Show
            m_PlayerVel = m_PlayerCharacterController.CharacterVelocity;
            if(m_PlayerVel != Vector3.zero) PlayerVel = m_PlayerVel.magnitude;
            else PlayerVel = 0;
            ///
            UpdateAllLists(BuffDict,"Buff");
            UpdateAllLists(DebuffDict,"Debuff");

            m_PlayerCharacterController.SpeedMult = SpeedMult;// Speed update 2
            m_PlayerCharacterController.MovementSharpnessOnGround = Slippery; // Slippery update
            m_PlayerCharacterController.fastDash = fastDash; // Special Fast Dash

            if(auxiliar){
                if(m_PlayerCharacterController.IsGrounded){
                    StartCoroutine(StartCantStopMoving(0,auxiliartime));
                }
            }

        }
        void UpdateAllLists(Dictionary<string,List<Buff>> dict, string type){
            //foreach(KeyValuePair<string,List<Buff>> item in dict){
            foreach(var (bufftype,list) in dict){
                if(list.Count != 0){
                    //int i = 0;
                    if(type == "Buff"){
                        float maxi = 0;
                        foreach(Buff aux in new List<Buff>(list)){
                            if(aux.BMult >= maxi) maxi = aux.BMult;
                            float tr = Time.time - aux.BInit;
                            aux.TimeRemaining = aux.BTime - tr;
                            if(tr > aux.BTime){
                                //Debug.Log("Finished buff mult of "+aux.BMult);
                                list.Remove(aux);
                            }
                            if(bufftype == "Damage") BDmg = maxi;
                            else if (bufftype == "AttackSpeed") BATS = maxi;
                            else if (bufftype == "Speed") BSpeed = maxi;
                            else if (bufftype == "FastDash") fastDash = maxi;
                        }
                    }else if(type == "Debuff"){
                        float mini = 9999.0f;
                        foreach(Buff aux in new List<Buff>(list)){
                            if(aux.BMult <= mini) mini = aux.BMult;
                            float tr = Time.time - aux.BInit;
                            aux.TimeRemaining = aux.BTime - tr;
                            if(tr > aux.BTime){
                                //Debug.Log("Finished buff mult of "+aux.BMult);
                                list.Remove(aux);
                            }
                            if(bufftype == "Damage") DDmg = mini;
                            else if (bufftype == "AttackSpeed") DATS = mini;
                            else if (bufftype == "Speed") DSpeed = mini;
                            else if (bufftype == "Slippery") Slippery = mini;
                            else if (bufftype == "CSMoving") m_PlayerCharacterController.CantStopMoving = true;
                        }
                    }
                } else if ( list.Count == 0){
                    if(type == "Buff"){
                        if(bufftype == "Damage") BDmg = 1f;
                        else if(bufftype == "AttackSpeed") BATS = 1f;
                        else if(bufftype == "Speed") BSpeed = 1f;
                        else if(bufftype == "FastDash") fastDash = 1f;
                    }else if (type == "Debuff"){
                        if(bufftype == "Damage") DDmg = 1f;
                        else if(bufftype == "AttackSpeed") DATS = 1f;
                        else if(bufftype == "Speed") DSpeed = 1f;
                        else if(bufftype == "Slippery") Slippery = BaseSlippery;
                        else if(bufftype == "CSMoving") m_PlayerCharacterController.CantStopMoving = false;
                    }
                }
            }
        }

        /*void UpdateSlipperyList(){
            if(SlipperyList.Count != 0){
                //int i = 0;
                float mini = 999;
                foreach (Buff aux in new List<Buff>(SlipperyList)){
                    if(aux.BMult <= mini) mini = aux.BMult;
                    if(Time.time - aux.BInit > aux.BTime) {
                        Debug.Log("Finished buff mult of "+aux.BMult);
                        SlipperyList.Remove(aux);
                    }
                }
                Slippery = mini;
                //Debug.Log("Max "+maxi);
            } else if (SlipperyList.Count == 0) Slippery = BaseSlippery;
        }*/
        public void ActivateBuff(WeaponController weapon){
            Buffs buff = weapon.Buff;
            //Health p_health = m_PlayerCharacterController.GetComponent<Health>();
            if(buff.HealthAmmount != 0){ // Healed fix ammount of health
                var (buffmult, _) = GetNewBuffData(weapon.CurrentAmmoRatio,buff.HealthAmmount,0f,"Regeneration");
                p_health.Heal(Mathf.Round(buffmult));
                //p_health.Heal(buff.HealthAmmount);
                //hasHealed = true;
                //if (OnActiveBuff != null) OnActiveBuff.Invoke("Heal", 3);
                //StartCoroutine(RestartHealed());
            } 
            if(buff.HealthRegen != 0) { // Regenerate big/little ammount of health in fast/slow time
                //Debug.Log(buff.HealthRegen/buff.RegenTime);
                var (buffmult, time) = GetNewBuffData(weapon.CurrentAmmoRatio,buff.HealthRegen,buff.RegenTime,"Regeneration");
                AddToList(BuffDict["Regeneration"], buffmult,time);
                p_health.RegenHealth(buffmult,time);
                //isHealing = true;
                //if (OnActiveBuff != null) OnActiveBuff.Invoke("Regen", buff.RegenTime);
                //StartCoroutine(RestartHealing(buff.RegenTime));
            }
            if(buff.ShieldAmmount != 0){ // Shield fix ammount
                var (buffmult, _) = GetNewBuffData(weapon.CurrentAmmoRatio,buff.ShieldAmmount,0f,"Regeneration");
                p_health.Shield(Mathf.Round(buffmult));
                //p_health.Shield(buff.ShieldAmmount);
                //hasShielded = true;
                //StartCoroutine(RestartShielded());
            } 
            if(buff.Dmg != 0){
                var (buffmult, time) = GetNewBuffData(weapon.CurrentAmmoRatio,buff.Dmg,buff.DmgTime,"Buff");
                //AddToList(BuffDict["Damage"], buff.Dmg,buff.DmgTime);
                AddToList(BuffDict["Damage"], buffmult,time);
            }
            if(buff.AtkSpeed != 0){
                var (buffmult, time) = GetNewBuffData(weapon.CurrentAmmoRatio,buff.AtkSpeed,buff.ASTime,"Buff");
                //AddToList(BuffDict["AttackSpeed"], buff.AtkSpeed,buff.ASTime);
                AddToList(BuffDict["AttackSpeed"], buffmult,time);
            }
            if(buff.Speed != 0){
                var (buffmult, time) = GetNewBuffData(weapon.CurrentAmmoRatio,buff.Speed,buff.SpeedTime,"Buff");
                //AddToList(BuffDict["Speed"], buff.Speed,buff.SpeedTime);
                AddToList(BuffDict["Speed"], buffmult,time);
            }
            if(buff.Special.Length != 0){
                foreach(Buffs.BSpecial SpecialBuff in buff.Special){
                    float BIndex = SpecialBuff.BIndex;
                    float Bmult = SpecialBuff.Bmult;
                    float BTime = SpecialBuff.BTime;
                    if(BIndex == 0){ // Clear Debuffs
                        HandleClearDebuff(weapon);
                    }else if (BIndex == 1){
                        var (buffmult, time) = GetNewBuffData(weapon.CurrentAmmoRatio,Bmult,BTime,"Buff");
                        //Debug.Log("Doesn't Delay");
                        AddToList(BuffDict["FastDash"], buffmult,time);
                    }
                }
            }
        }
        void HandleClearDebuff(WeaponController weapon){
            if(weapon.CurrentAmmoRatio == 1f){ /// Clear all debuffs
                foreach(var(_,dlist) in DebuffDict){
                    dlist.Clear();
                }
            }else if(weapon.CurrentAmmoRatio >= 0.5f){ /// Clear half debuffs
                List<string> arr = new List<string>();
                foreach(var(dtype,dlist) in DebuffDict){
                    arr.Add(dtype);
                }
                System.Random rand = new System.Random();
                arr = arr.OrderBy(_ => rand.Next()).ToList();
                foreach(string str in arr.GetRange(0,(int)(arr.Count/2))){
                    DebuffDict[str].Clear();
                }
            }else{ /// Clear only 1 debuff
                List<string> arr = new List<string>();
                foreach(var(dtype,dlist) in DebuffDict){
                    arr.Add(dtype);
                }
                System.Random rand = new System.Random();
                arr = arr.OrderBy(_ => rand.Next()).ToList();
                DebuffDict[arr[0]].Clear();
            }
        }
        //(float,float) GetNewBuffData(WeaponController weapon, string type){
        public void ActivateDebuff(WeaponController weapon){
            Debuffs debuff = weapon.Debuff;
            //Health p_health = m_PlayerCharacterController.GetComponent<Health>();
            if(debuff.Dmg != 0){
                var (buffmult, time) = GetNewBuffData(weapon.CurrentAmmoRatio,debuff.Dmg,debuff.DmgTime,"Debuff");
                if(debuff.DmgDelay == 0) {
                    //AddToList(DDMGList, debuff.Dmg,debuff.DmgTime);
                    AddToList(DebuffDict["Damage"], buffmult,time);
                    //DebuffDamage(debuff);
                } else {
                    var (_, delaytime) = GetNewBuffData(weapon.CurrentAmmoRatio,1.0f,debuff.DmgDelay,"Debuff");
                    StartCoroutine(WaitDebuffMult(delaytime,"Damage",buffmult,time));
                    //StartCoroutine(WaitDDmgMult(debuff));
                }
            }
            if(debuff.AtkSpeed != 0){
                var (buffmult, time) = GetNewBuffData(weapon.CurrentAmmoRatio,debuff.AtkSpeed,debuff.ASTime,"Debuff");
                if(debuff.ASDelay == 0) {
                    //AddToList(DATSList, debuff.AtkSpeed,debuff.ASTime);
                    AddToList(DebuffDict["AttackSpeed"], buffmult,time);
                    //DebuffDamage(debuff);
                } else {
                    var (_, delaytime) = GetNewBuffData(weapon.CurrentAmmoRatio,1.0f,debuff.ASDelay,"Debuff");
                    StartCoroutine(WaitDebuffMult(delaytime,"AttackSpeed",buffmult,time));
                    //StartCoroutine(WaitDATSMult(debuff));
                }
            }
            if(debuff.Speed != 0){
                var (buffmult, time) = GetNewBuffData(weapon.CurrentAmmoRatio,debuff.Speed,debuff.SpeedTime,"Debuff");
                if(debuff.SpeedDelay == 0) {
                    //AddToList(DSpeedList, debuff.Speed,debuff.SpeedTime);
                    AddToList(DebuffDict["Speed"], buffmult,time);
                    //DebuffDamage(debuff);
                } else {
                    var (_, delaytime) = GetNewBuffData(weapon.CurrentAmmoRatio,1.0f,debuff.SpeedDelay,"Debuff");
                    StartCoroutine(WaitDebuffMult(delaytime,"Speed",buffmult,time));
                    //StartCoroutine(WaitDSpeedMult(debuff));
                }
            }
            if(debuff.Special.Length != 0){
                foreach(Debuffs.DSpecial SpecialDebuff in debuff.Special){
                    float DIndex = SpecialDebuff.DIndex;
                    float DDelay = SpecialDebuff.DDelay;
                    float DTime = SpecialDebuff.DTime;
                    if(DIndex == 0){ // Special Slippery
                        var (buffmult, time) = GetNewBuffData(weapon.CurrentAmmoRatio,0.5f,DTime,"Slippery");
                        if(DDelay == 0){
                            AddToList(DebuffDict["Slippery"], buffmult,time);
                        }else {
                            var (_, delaytime) = GetNewBuffData(weapon.CurrentAmmoRatio,0.5f,DDelay,"Slippery");
                            StartCoroutine(WaitDebuffMult(delaytime,"Slippery",buffmult,time));
                        }
                    }else if(DIndex == 1){ /// Special Can't Stop Moving
                        var (buffmult, time) = GetNewBuffData(weapon.CurrentAmmoRatio,1f,DTime,"CSMoving");
                        if(DDelay == 0){
                            //Debug.Log("Doesn't Delay");
                            AddToList(DebuffDict["CSMoving"], buffmult,time);
                            //StartCoroutine(StartCantStopMoving(0,DTime));
                        }else {
                            var (_, delaytime) = GetNewBuffData(weapon.CurrentAmmoRatio,1f,DDelay,"CSMoving");
                            //Debug.Log("Has Delay");
                            StartCoroutine(WaitDebuffMult(delaytime,"CSMoving",buffmult,time));
                            //StartCoroutine(StartCantStopMoving(DDelay,DTime));
                        }
                    }
                }
            }
        }

        (float,float) GetNewBuffData(float wratio,float bmult, float btime, string type){
            float mult = 1.0f;
            float time = 0f;
            mult = bmult;
            time = btime;
            
            float ratio = wratio;
            //Debug.Log(ratio);
            if(type != "Regeneration") mult = Mathf.Abs(mult - 1.0f);
            if(type == "Slippery"){
                if(ratio != 1.0f){
                    if(ratio > 0.7f){
                        mult = 1f;
                        time = Mathf.Round(time*0.7f* 100f) / 100f;
                    }else if(ratio > 0.3){
                        mult = 1.75f;
                        time = Mathf.Round(time*0.5f* 100f) / 100f;
                    }else{
                        mult = 2.5f;
                        time = Mathf.Round(time*0.3f* 100f) / 100f;
                    }
                }
            }else if(type == "CSMoving"){
                if(ratio != 1.0f){
                    if(ratio > 0.7f){
                        time = Mathf.Round(time*0.7f* 100f) / 100f;
                    }else if(ratio > 0.3){
                        time = Mathf.Round(time*0.5f* 100f) / 100f;
                    }else{
                        time = Mathf.Round(time*0.3f* 100f) / 100f;
                    }
                }
            }else if(type == "Regeneration"){
                if(ratio != 1.0f){
                    if(ratio > 0.7f){
                        mult = Mathf.Round(mult*0.85f* 100f) / 100f;
                        time = Mathf.Round(time*0.85f* 100f) / 100f;
                    }else if(ratio > 0.3){
                        mult = Mathf.Round(mult*0.65f* 100f) / 100f;
                        time = Mathf.Round(time*0.65f* 100f) / 100f;
                    }else{
                        mult = Mathf.Round(mult*0.5f* 100f) / 100f;
                        time = Mathf.Round(time*0.5f* 100f) / 100f;
                    }
                }
            }else{
                if(ratio != 1.0f){
                    if(ratio > 0.7f){
                        mult = Mathf.Round(mult*0.7f* 100f) / 100f;
                        time = Mathf.Round(time*0.7f* 100f) / 100f;
                    }else if(ratio > 0.3){
                        mult = Mathf.Round(mult*0.5f* 100f) / 100f;
                        time = Mathf.Round(time*0.5f* 100f) / 100f;
                    }else{
                        mult = Mathf.Round(mult*0.3f* 100f) / 100f;
                        time = Mathf.Round(time*0.3f* 100f) / 100f;
                    }
                }
            }
            
            if(type == "Buff") mult = Mathf.Abs(1.0f + mult);
            else if(type == "Regeneration") return (mult,time);
            else mult = Mathf.Abs(1.0f - mult);
            return (mult,time);
        }
        IEnumerator WaitDebuffMult(float delay, string type, float mult, float time){
            yield return new WaitForSeconds(delay);
            //AddToList(DDMGList, debuff.Dmg,debuff.DmgTime);
            AddToList(DebuffDict[type], mult,time);
            //coDMG = null;
        }
        /*IEnumerator WaitDDmgMult(Debuffs debuff){
            yield return new WaitForSeconds(debuff.DmgDelay);
            //AddToList(DDMGList, debuff.Dmg,debuff.DmgTime);
            AddToList(DebuffDict["Damage"], debuff.Dmg,debuff.DmgTime);
            //coDMG = null;
        }
        IEnumerator WaitDATSMult(Debuffs debuff){
            yield return new WaitForSeconds(debuff.ASDelay);
            //AddToList(DATSList, debuff.AtkSpeed,debuff.ASTime);
            AddToList(DebuffDict["AttackSpeed"], debuff.AtkSpeed,debuff.ASTime);
            //coDMG = null;
        }
        IEnumerator WaitDSpeedMult(Debuffs debuff){
            yield return new WaitForSeconds(debuff.SpeedDelay);
            //AddToList(DSpeedList, debuff.Speed,debuff.SpeedTime);
            AddToList(DebuffDict["Speed"], debuff.Speed,debuff.SpeedTime);
            //coDMG = null;
        }
        IEnumerator WaitSlippery(float slippery, float time, float delay){
            yield return new WaitForSeconds(delay);
            AddToList(DebuffDict["Slippery"], slippery,time);
            //AddToList(SlipperyList, slippery,time);
            //coDMG = null;
        }*/
        IEnumerator RestartCantStopMoving(float time){
            yield return new WaitForSeconds(time);
            m_PlayerCharacterController.CantStopMoving = false;
            //coDMG = null;
        }
        IEnumerator StartCantStopMoving(float delay, float time){
            yield return new WaitForSeconds(delay);
            if(m_PlayerCharacterController.IsGrounded){
                m_PlayerCharacterController.CantStopMoving = true;
                auxiliar = false;
                StartCoroutine(RestartCantStopMoving(time));
            }
            else {
                auxiliar = true;
                auxiliartime = time;
            } 
        }

        public void AddToList(List<Buff> list,float mult, float time){
            if(list.Count == 0) list.Add(new Buff() {BMult = mult, BTime = time, BInit = Time.time}); // If list is empty add element
            else {
                bool found = false;
                foreach (Buff aux in list){ // if already multiplier exist in list, update time
                    if(aux.BMult == mult) {
                        aux.BTime = time;
                        aux.BInit = Time.time;
                        found = true;
                    }
                }
                if(!found) list.Add(new Buff() {BMult = mult, BTime = time, BInit = Time.time}); // if it wasn't found in the list, then adds the element to the list
            }
        }

        /*IEnumerator RestartHealed(float time = 2){
            yield return new WaitForSeconds(time);
            hasHealed = false;
        }
        IEnumerator RestartShielded(float time = 2){
            yield return new WaitForSeconds(time);
            hasShielded = false;
        }
        /*IEnumerator RestartHealing(float time){
            yield return new WaitForSeconds(time);
            isHealing = false;
        }*/
        
    }
}