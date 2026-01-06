using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made by Rajendra Abhinaya, 2023

namespace DiabolicalGames{
    public class Despawn : MonoBehaviour
    {
        private int despawnPercentage;
        private float despawnTime;
        private float distanceFromPlayer;
        private GameObject player;

        private Vector3[] _initLocalPos;
        private Quaternion[] _initLocalRot;
        private Rigidbody[] _rbs;

        private void Awake()
        {
            int n = transform.childCount;
            _initLocalPos = new Vector3[n];
            _initLocalRot = new Quaternion[n];
            _rbs = new Rigidbody[n];

            for (int i = 0; i < n; i++)
            {
                Transform t = transform.GetChild(i);
                _initLocalPos[i] = t.localPosition;
                _initLocalRot[i] = t.localRotation;
                _rbs[i] = t.GetComponent<Rigidbody>();
            }
        }

        private void OnEnable()
        {
            // Quand on réactive le debris, on remet tout propre
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform t = transform.GetChild(i);

                // Réactiver les morceaux (si certains ont été désactivés)
                t.gameObject.SetActive(true);

                // Reset transform
                t.localPosition = _initLocalPos[i];
                t.localRotation = _initLocalRot[i];

                // Reset rigidbody
                var rb = _rbs[i];
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;   // ou rb.velocity selon version
                    rb.angularVelocity = Vector3.zero;
                    rb.Sleep();
                    rb.WakeUp();
                }
            }
        }

        //Used to receive the variables' values from the parent object
        public void SetVariables(int despawnPercentage, float despawnTime, float distanceFromPlayer, GameObject player){
        this.despawnPercentage = despawnPercentage;
        this.despawnTime = despawnTime;
        this.distanceFromPlayer = distanceFromPlayer;
        this.player = player;
    }

        //Starts the selected despawn mode's coroutine function
        public void BeginCoroutine(string coroutine){
            switch(coroutine){
                case "Timed":
                    StartCoroutine(DespawnCoroutine());
                    break;
                case "Distance from Player":
                    StartCoroutine(CheckDistance());
                    break;
            }
        }

        //Despawns the debris based on the despawn percentage
        public void DespawnDebris(){
            int despawnCount = transform.childCount * despawnPercentage/100;
            for(int i = transform.childCount-1; i >= transform.childCount-despawnCount; i--){
                    var child = transform.GetChild(i).gameObject;
                    child.SetActive(false);
                }
        }

        //Checks the distance between the debris and the player every 0.5 seconds after a 5 second delay
        public IEnumerator CheckDistance(){
            yield return new WaitForSeconds(5f);
            while(true){
                Vector3 distance = transform.position - player.transform.position;
                if(distance.magnitude > distanceFromPlayer){
                    DespawnDebris();
                    yield break;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        //Despawns the debris after a set amount of time
        public IEnumerator DespawnCoroutine(){
            yield return new WaitForSeconds(despawnTime);
            DespawnDebris();
        }
    }
}