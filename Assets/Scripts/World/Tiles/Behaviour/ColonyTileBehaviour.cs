using Minefactory.Common;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;


namespace Minefactory.World.Tiles.Behaviour
{
    public class ColonyTileBehaviour : BreakableTileBehaviour
    {
        public GameObject oxygenZoneObject;


        public void OnTilePlaced()
        {
            Debug.Log("tu som");
            if (GameStateManager.Instance != null)
            {
                Debug.Log("jak to je");
                GameStateManager.Instance.Population += 100;
            }
            else
            {
                Debug.Log("korutinka");
                StartCoroutine(RetryAwardPopulation());
            }
        }

        private IEnumerator RetryAwardPopulation()
        {
            Debug.Log("teraz tu");
            yield return new WaitForEndOfFrame();
            if (GameStateManager.Instance != null)
            {
                Debug.Log("tada");
                GameStateManager.Instance.Population += 100;
            }
        }

        // Override OnMouseDown to handle population reduction before destroying
        void OnMouseDown()
        {
            // Reduce population first
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.Population -= 100;
            }
            else
            {
                StartCoroutine(RetryReducePopulation());
            }

            // Call the base class OnMouseDown to handle the rest of the destruction logic
            base.OnMouseDown();
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