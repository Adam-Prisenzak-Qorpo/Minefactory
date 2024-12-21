using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Minefactory.World.Tiles.Behaviour;

namespace Minefactory.Factories.Mining
{
    public class MinerOutputUI : MonoBehaviour
    {
        [Header("Production Display")]
        public TextMeshProUGUI totalIronProductionText; 
        public TextMeshProUGUI totalIronOutputText; 
        public TextMeshProUGUI totalGoldProductionText;
        public TextMeshProUGUI totalGoldOutputText;

        [Header("Output Configuration")]
        public Slider ironOutputSlider;
        public Slider goldOutputSlider;
        public TextMeshProUGUI thisIronOutputText;
        public TextMeshProUGUI thisGoldOutputText;

        [Header("UI Elements")]
        public Button closeButton;
        
        private MinerOutputBehaviour minerOutput;
        private bool isInitializing = false;

        public void Initialize(MinerOutputBehaviour output)
        {
            minerOutput = output;
            SetupUI(true);
        }

        private void OnEnable()
        {
            Debug.Log("MinerOutputUI OnEnable called");
            SetupUI(false);
        }

        private void SetupUI(bool isFirstTime)
        {
            isInitializing = true;

            // Find and setup references
            totalIronProductionText = transform.Find("Production Panel/Total Iron Production").GetComponent<TextMeshProUGUI>();
            totalIronOutputText = transform.Find("Production Panel/Total Iron Output").GetComponent<TextMeshProUGUI>();
            totalGoldProductionText = transform.Find("Production Panel/Total Gold Production").GetComponent<TextMeshProUGUI>();
            totalGoldOutputText = transform.Find("Production Panel/Total Gold Output").GetComponent<TextMeshProUGUI>();
            closeButton = transform.Find("CloseButton").GetComponent<Button>();
            
            if (ironOutputSlider == null)
                ironOutputSlider = transform.Find("Output Panel/Iron Slider").GetComponent<Slider>();
            if (goldOutputSlider == null)
                goldOutputSlider = transform.Find("Output Panel/Gold Slider").GetComponent<Slider>();
            
            thisIronOutputText = transform.Find("Output Panel/Iron Slider/Iron Output Text").GetComponent<TextMeshProUGUI>();
            thisGoldOutputText = transform.Find("Output Panel/Gold Slider/Gold Output Text").GetComponent<TextMeshProUGUI>();

            // Setup button
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);

            // Remove old listeners before adding new ones
            ironOutputSlider.onValueChanged.RemoveAllListeners();
            goldOutputSlider.onValueChanged.RemoveAllListeners();

            var manager = MiningProductionManager.Instance;
            if (minerOutput != null)
            {
                float currentIronRate = minerOutput.GetIronOutputRate();
                float currentGoldRate = minerOutput.GetGoldOutputRate();

                // Set max values first
                ironOutputSlider.maxValue = manager.GetAvailableOutputRate("iron") + currentIronRate;
                goldOutputSlider.maxValue = manager.GetAvailableOutputRate("gold") + currentGoldRate;

                // Set values without triggering events (because isInitializing is true)
                ironOutputSlider.value = currentIronRate;
                goldOutputSlider.value = currentGoldRate;

                // Update text displays
                thisIronOutputText.text = $"Iron Output: {currentIronRate:F1}/min";
                thisGoldOutputText.text = $"Gold Output: {currentGoldRate:F1}/min";
            }

            // Add listeners after setting initial values
            ironOutputSlider.onValueChanged.AddListener(OnIronSliderChanged);
            goldOutputSlider.onValueChanged.AddListener(OnGoldSliderChanged);

            // Update the rest of the UI
            UpdateUI();

            isInitializing = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }

        public void UpdateUI()
        {
            var manager = MiningProductionManager.Instance;
            
            if (manager != null)
            {
                float ironProduction = manager.GetTotalProductionRate("iron");
                float goldProduction = manager.GetTotalProductionRate("gold");
                
                totalIronProductionText.text = $"Total Iron Production: {ironProduction:F1}/min";
                totalGoldProductionText.text = $"Total Gold Production: {goldProduction:F1}/min";
                totalIronOutputText.text = $"Total Iron Output: {manager.GetCurrentOutputRate("iron"):F1}/min";
                totalGoldOutputText.text = $"Total Gold Output: {manager.GetCurrentOutputRate("gold"):F1}/min";
            }
        }

        private void OnIronSliderChanged(float value)
        {
            if (!isInitializing)
            {
                minerOutput.SetOutputRate("iron", value);
                thisIronOutputText.text = $"Iron Output: {value:F1}/min";
                totalIronOutputText.text = $"Total Iron Output: {MiningProductionManager.Instance.GetCurrentOutputRate("iron"):F1}/min";
            }
        }

        private void OnGoldSliderChanged(float value)
        {
            if (!isInitializing)
            {
                minerOutput.SetOutputRate("gold", value);
                thisGoldOutputText.text = $"Gold Output: {value:F1}/min";
                totalGoldOutputText.text = $"Total Gold Output: {MiningProductionManager.Instance.GetCurrentOutputRate("gold"):F1}/min";
            }
        }

        public void Close()
        {
            if (minerOutput != null)
                minerOutput.CloseConfigUI();
        }
    }
}