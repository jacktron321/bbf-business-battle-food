using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.UI;
namespace Unity.FPS.Gameplay
{
    public class ObjectiveKillEnemies1 : Objective
    {
        public LevelManager levelMang;
        LevelController InstanceofLevel;

        [Header("Animators")] [Tooltip("Animators ran after each room")]
        
        //[SerializeField] private 
        //public Animator myDoor = null;
        //[SerializeField] private Animator myDoor2 = null;
        //[SerializeField] private Animator myDoor3 = null;
         [SerializeField] private Animator elevatorDoor = null;
         [SerializeField] private Animator elevatorDoor2 = null;


        [Header("Notifications")] [Tooltip("notifications to add history")]
        
        //[SerializeField] private Text notifText;
         private string NotificationText1 = "Succes, now go to the next room, you pissed off some new colleagues...";
         private string NotificationText2 = "More are coming..hurry to the next room!!";

        
        [Header("Objective & Ennemies")] [Tooltip("Info about objective and Ennemies")]
        public bool MustKillAllEnemies = true;

        [Tooltip("If MustKillAllEnemies is false, this is the amount of enemy kills required")]
        public int KillsToCompleteObjective = 5;

        [Tooltip("Start sending notification about remaining enemies when this amount of enemies is left")]
        public int NotificationEnemiesRemainingThreshold = 3;

        int m_KillTotal;
        
        [Header("Number of ennemies per room")] [Tooltip("notifications to add history")]
        public int[] ennemiesList;
        [Header("Ennemies Prefabs")] [Tooltip("")]
        public GameObject BossPrefab;
        public GameObject SecondWavePrefab;

        private int actualRoom = 0;
        private int ennemiesForNextRoom = 0;

        protected override void Start()
        {
            base.Start();
            ennemiesForNextRoom = ennemiesList[actualRoom];
            EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);

            // set a title and description specific for this type of objective, if it hasn't one
            if (string.IsNullOrEmpty(Title))
                Title = "Eliminate " + (MustKillAllEnemies ? "all the" : KillsToCompleteObjective.ToString()) +
                        " enemies";

            if (string.IsNullOrEmpty(Description))
                Description = GetUpdatedCounterAmount();
        }
        void OnEnemyKilled(EnemyKillEvent evt)
        {
            print("ENNEMY KILLED");
            if (IsCompleted)
                return;

            m_KillTotal++;

            if (MustKillAllEnemies)
                KillsToCompleteObjective = evt.RemainingEnemyCount + m_KillTotal;

            int targetRemaining = MustKillAllEnemies ? evt.RemainingEnemyCount : KillsToCompleteObjective - m_KillTotal;
            // update the objective text according to how many enemies remain to kill
            if(levelMang.InstanceOfLevel1){
               InstanceofLevel = levelMang.InstanceOfLevel1.GetComponent<LevelController>(); 
            }
            if ((actualRoom==0)&&(m_KillTotal == ennemiesForNextRoom))
            {        
                Debug.Log("Aqui");
                InstanceofLevel.OpenDoor(0);   
                //Debug.Log(myDoor.name);  
                //myDoor.Play("OpenDoor",0,0.0f);
                //notifText.text=NotificationText1;
                gameObject.SetActive(false);
                actualRoom++;
                ennemiesForNextRoom +=ennemiesList[actualRoom];
                //CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
            }
            else if ((actualRoom==1)&&(m_KillTotal == ennemiesForNextRoom))
            {                
                //myDoor2.Play("OpenDoor2",0,0.0f);
                InstanceofLevel.OpenDoor(1);   
                gameObject.SetActive(false);
                actualRoom++;
                ennemiesForNextRoom +=ennemiesList[actualRoom];
                //CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
            }
            else if ((actualRoom==2)&&(m_KillTotal == ennemiesForNextRoom))
            {                
                //myDoor3.Play("OpenDoor3",0,0.0f);
                InstanceofLevel.OpenDoor(2);   
                gameObject.SetActive(false);
                actualRoom++;
                ennemiesForNextRoom +=ennemiesList[actualRoom];
                //CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
            }
            else if ((actualRoom==3)&&(m_KillTotal == ennemiesForNextRoom))
            {                
                elevatorDoor.Play("ElevatorDoor",0,0.0f);
                gameObject.SetActive(false);
                //notifText.text=NotificationText1;
                actualRoom++;
                ennemiesForNextRoom +=ennemiesList[actualRoom];
                //CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
            }else if ((actualRoom==4)&&(m_KillTotal == ennemiesForNextRoom))
            {                
                elevatorDoor2.Play("ElevatorDoor2",0,0.0f);
                gameObject.SetActive(false);
                //notifText.text=NotificationText1;
                actualRoom++;
                ennemiesForNextRoom +=ennemiesList[actualRoom];
                //CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
            }else if ((actualRoom==5)&&(m_KillTotal == ennemiesForNextRoom))
            {
                CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
            }
            /*else if (targetRemaining == 1)
            {
                //GameObject newEnemy = Instantiate(BossPrefab,new Vector3(-35.5f,0f,52.0f), Quaternion.identity);
                string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                    ? "The Boss Has Appeared!"
                    : string.Empty;
                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }
            else if (targetRemaining == 5)
            {
                GameObject newEnemy = Instantiate(SecondWavePrefab,new Vector3(-22.6f,0f,57f), Quaternion.identity);
                GameObject newEnemy1 = Instantiate(SecondWavePrefab,new Vector3(-31.2f,0f,57f), Quaternion.identity);
                GameObject newEnemy2 = Instantiate(SecondWavePrefab,new Vector3(-39.8f,0f,57f), Quaternion.identity);
                GameObject newEnemy3 = Instantiate(SecondWavePrefab,new Vector3(-48.4f,0f,57f), Quaternion.identity);
                string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                    ? "A new wave of enemies Has Appeared!"
                    : string.Empty;
                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }*/
            else
            {
                // create a notification text if needed, if it stays empty, the notification will not be created
                string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                    ? targetRemaining + " enemies to kill left"
                    : string.Empty;

                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }
        }

        string GetUpdatedCounterAmount()
        {
            return m_KillTotal + " / " + KillsToCompleteObjective;
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
        }
    }
}