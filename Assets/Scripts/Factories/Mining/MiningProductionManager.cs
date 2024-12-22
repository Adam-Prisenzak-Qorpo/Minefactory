using UnityEngine;
using System.Collections.Generic;
using Minefactory.World;

namespace Minefactory.Factories.Mining
{
    public class MiningProductionManager : MonoBehaviour
    {
        private static MiningProductionManager instance;
        public static MiningProductionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("MiningProductionManager");
                    instance = go.AddComponent<MiningProductionManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private Dictionary<string, float> totalProductionRates = new Dictionary<string, float>();
        private Dictionary<string, float> currentOutputRates = new Dictionary<string, float>();

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize supported ore types
            totalProductionRates["iron"] = 0f;
            totalProductionRates["gold"] = 0f;
            currentOutputRates["iron"] = 0f;
            currentOutputRates["gold"] = 0f;
        }

        public void AddMiningProduction(string oreType, float rate)
        {
            if (totalProductionRates.ContainsKey(oreType))
            {
                totalProductionRates[oreType] += rate;
                Debug.Log($"Added mining production for {oreType}: {rate}/min. Total: {totalProductionRates[oreType]}/min");
            }
        }

        public void UpgradeMiningProduction(float upgradedMiningRate)
        {
            totalProductionRates["iron"] = upgradedMiningRate;
            totalProductionRates["gold"] = upgradedMiningRate;
        }

        public void RemoveMiningProduction(string oreType, float rate)
        {
            if (totalProductionRates.ContainsKey(oreType))
            {
                totalProductionRates[oreType] = Mathf.Max(0, totalProductionRates[oreType] - rate);
            }
        }

        public void SetOutputRate(string oreType, float rate)
        {
            if (currentOutputRates.ContainsKey(oreType))
            {
                currentOutputRates[oreType] = rate;
            }
        }

        public float GetTotalProductionRate(string oreType)
        {
            var coef = GameStateManager.Instance.GetSharedState("Mining", 1f);
            return totalProductionRates.GetValueOrDefault(oreType, 0f) * coef;
        }

        public float GetCurrentOutputRate(string oreType)
        {
            return currentOutputRates.GetValueOrDefault(oreType, 0f);
        }

        public float GetAvailableOutputRate(string oreType)
        {
            return totalProductionRates.GetValueOrDefault(oreType, 0f) - currentOutputRates.GetValueOrDefault(oreType, 0f);
        }
    }
}