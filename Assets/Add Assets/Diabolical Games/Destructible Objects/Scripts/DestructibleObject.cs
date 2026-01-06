using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made by Rajendra Abhinaya, 2023

namespace DiabolicalGames{
    public class DestructibleObject : MonoBehaviour
    {
        enum DebrisAmount{
            Low,
            Medium,
            High,
            Random
        }

        enum DespawnType{
            None,
            Timed,
            DistanceFromPlayer
        }

        [System.Serializable]
        struct DebrisPrefab{
            public string name;
            public GameObject prefab;
        }

        [Header("Debris")]
        [SerializeField, Tooltip("List of debris prefabs that will be spawned when the object is destroyed. Editing the list is not recommended")]
        private List<DebrisPrefab> debrisPrefabs = new List<DebrisPrefab>();

        [SerializeField, Tooltip("Amount of debris(Gameobjects) that will be spawned when the object breaks")]
        private DebrisAmount debrisAmount = new DebrisAmount();

        [SerializeField, Tooltip("Force required to break the object")]
        private float forceRequired;

        [Header("Despawning")]
        [SerializeField, Tooltip("Despawn mode used for despawning the debris created by destroying the object\nNone: debris will not be despawned\nTimed: debris will despawned after a certain time\nDistance from Player: debris will despawn when the player moves a certain distance away from the debris")]
        private DespawnType despawnType = new DespawnType();

        [SerializeField, Range(0, 100), Tooltip("Percentage of debris objects that will be despawned")]
        private int despawnPercentage;

        [SerializeField, Tooltip("Time in seconds before debris will despawn when using the Timed despawn mode")]
        private float despawnTime;

        [SerializeField, Tooltip("Player gameobject for when using the Distance from Player despawn mode")]
        private GameObject player;

        [SerializeField, Tooltip("Distance between debris and player before debris despawns when using the Distance from Player despawn mode")]
        private float distanceFromPlayer;
    
        private GameObject debris;
        private new Rigidbody rigidbody;

        [SerializeField] private DebrisAmount pcvrDebris = DebrisAmount.High;
        [SerializeField] private DebrisAmount questDebris = DebrisAmount.Low;



        public void Break(){
            float velocityMagnitude = rigidbody.linearVelocity.magnitude;

            //Activates the debris object and sets its position and rotation to match the object's
            debris.transform.position = transform.position;
            debris.transform.rotation = transform.rotation;
            debris.transform.localScale = transform.localScale;
            debris.SetActive(true);

            //Applies force to the debris based on the velocity of the object
            for(int i = 0; i < debris.transform.childCount; i++){
                Rigidbody debrisRigidbody = debris.transform.GetChild(i).GetComponent<Rigidbody>();
                Vector3 randomise = new Vector3(Random.Range(0f, velocityMagnitude), Random.Range(0f, velocityMagnitude), Random.Range(0f, velocityMagnitude)) / 2;
                debrisRigidbody.linearVelocity = rigidbody.linearVelocity + randomise;
            }

            //Sends variable values to the debris
            debris.GetComponent<Despawn>().SetVariables(despawnPercentage, despawnTime, distanceFromPlayer, player);

            //Activates the despawning mechanism of the debris
            switch(despawnType){
                case DespawnType.Timed:
                    debris.GetComponent<Despawn>().BeginCoroutine("Timed");
                    break;
                case DespawnType.DistanceFromPlayer:
                    debris.GetComponent<Despawn>().BeginCoroutine("Distance from Player");
                    break;
            }

            var pooled = GetComponent<PooledTarget>();
            if (pooled != null)
            {
                pooled.ReturnToPool();
            }
            else
            {
                Destroy(gameObject);
            }

        }

        void Awake()
        {
            if (Application.platform == RuntimePlatform.Android)
                debrisAmount = questDebris;
            else
                debrisAmount = pcvrDebris;
        }

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();

            switch(debrisAmount){
                case DebrisAmount.Low:
                    debris = Instantiate(debrisPrefabs[0].prefab, transform.position, Quaternion.identity);
                    break;
                case DebrisAmount.Medium:
                    debris = Instantiate(debrisPrefabs[1].prefab, transform.position, Quaternion.identity);
                    break;
                case DebrisAmount.High:
                    debris = Instantiate(debrisPrefabs[2].prefab, transform.position, Quaternion.identity);
                    break;
                case DebrisAmount.Random:
                    debris = Instantiate(debrisPrefabs[Random.Range(0,3)].prefab, transform.position, Quaternion.identity);
                    break;
                default:
                    debris = Instantiate(debrisPrefabs[0].prefab, transform.position, Quaternion.identity);
                    break;
            }
            debris.SetActive(false);
        }

        void OnCollisionEnter(Collision collision){
            if(collision.relativeVelocity.magnitude > forceRequired){
                Break();
            }
        }
    }
}