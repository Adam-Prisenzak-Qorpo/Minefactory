using Minefactory.Common;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;


namespace Minefactory.World.Tiles.Behaviour
{
    public class ColonyTileBehaviour : PersistentTileBehaviour
    {
        private bool isInitialized = false;

        
        protected override void Start()
        {
            base.Start();

            if(!isGhostTile){
                OnActivate();
            }
        }

        public override void OnActivate()
        {
            base.OnActivate();
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.Population += 100;
            }
            else
            {
                StartCoroutine(RetryAwardPopulation());
            }
            isInitialized = true; // Mark as initialized after placement is complete
        }

        private IEnumerator RetryAwardPopulation()
        {
            yield return new WaitForEndOfFrame();
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.Population += 100;
            }
        }

        public override void OnDeactivate()
        {
            Debug.Log("ondeactivate");
            if (!isInitialized) return; // Only handle destruction if the tile has been fully initialized
            
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.Population -= 100;
            }
            else
            {
                Debug.Log("tu som");
                StartCoroutine(RetryReducePopulation());
            }

            base.OnDeactivate();
        }

        private IEnumerator RetryReducePopulation()
        {
            yield return new WaitForEndOfFrame();
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.Population -= 100;
            }
        }
    }
}