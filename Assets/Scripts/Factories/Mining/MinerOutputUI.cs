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
        public Button closeButton; // Reference to close button
        
        private MinerOutputBehaviour minerOutput;

        public void Initialize(MinerOutputBehaviour output)
        {
            minerOutput = output;
            
            // Set initial slider values to match current output rates
            ironOutputSlider.value = minerOutput.GetIronOutputRate();
            goldOutputSlider.value = minerOutput.GetGoldOutputRate();
            
            UpdateUI();
        }

        private void OnEnable()
        {
            Debug.Log("MinerOutputUI OnEnable called");
            totalIronProductionText = transform.Find("Production Panel/Total Iron Production").GetComponent<TextMeshProUGUI>();
            totalIronOutputText = transform.Find("Production Panel/Total Iron Output").GetComponent<TextMeshProUGUI>();
            totalGoldProductionText = transform.Find("Production Panel/Total Gold Production").GetComponent<TextMeshProUGUI>();
            totalGoldOutputText = transform.Find("Production Panel/Total Gold Output").GetComponent<TextMeshProUGUI>();
            closeButton = transform.Find("CloseButton").GetComponent<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(Close);
            }
            if (ironOutputSlider == null)
                ironOutputSlider = transform.Find("Output Panel/Iron Slider").GetComponent<Slider>();
            if (goldOutputSlider == null)
                goldOutputSlider = transform.Find("Output Panel/Gold Slider").GetComponent<Slider>();
            thisIronOutputText = transform.Find("Output Panel/Iron Slider/Iron Output Text").GetComponent<TextMeshProUGUI>();
            thisGoldOutputText = transform.Find("Output Panel/Gold Slider/Gold Output Text").GetComponent<TextMeshProUGUI>();
            
            ironOutputSlider.onValueChanged.AddListener((value) => OnIronSliderChanged(value));
            goldOutputSlider.onValueChanged.AddListener((value) => OnGoldSliderChanged(value));
            var manager = MiningProductionManager.Instance;
            ironOutputSlider.maxValue = ((manager.GetAvailableOutputRate("iron"))+ironOutputSlider.value);
            goldOutputSlider.maxValue = ((manager.GetAvailableOutputRate("gold"))+goldOutputSlider.value);
        }


        private void Update()
        {
            // Check for ESC key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }

        public void UpdateUI()
        {
            Debug.Log("UpdateUI called");
            var manager = MiningProductionManager.Instance;
            
            if (manager != null)
            {
                float ironProduction = manager.GetTotalProductionRate("iron");
                float goldProduction = manager.GetTotalProductionRate("gold");
                
                totalIronProductionText.text = $"Total Iron Production: {ironProduction:F1}/min";
                totalGoldProductionText.text = $"Total Gold Production: {goldProduction:F1}/min";
                totalIronOutputText.text = $"Total Iron Output: {manager.GetCurrentOutputRate("iron"):F1}/min";
                totalGoldOutputText.text = $"Total Gold Output: {manager.GetCurrentOutputRate("gold"):F1}/min";
                Debug.Log("Max output rates: " + manager.GetAvailableOutputRate("iron"));
                thisIronOutputText.text = $"Iron Output: {ironOutputSlider.value:F1}/min";
                thisGoldOutputText.text = $"Gold Output: {goldOutputSlider.value:F1}/min";
            }
        }

        private void OnIronSliderChanged(float value)
        {

            minerOutput.SetOutputRate("iron", value);
            this.thisIronOutputText.text = $"Iron Output: {value:F1}/min";
            this.totalIronOutputText.text = $"Total Iron Output: {MiningProductionManager.Instance.GetCurrentOutputRate("iron"):F1}/min";
        }

        private void OnGoldSliderChanged(float value)
        {
            minerOutput.SetOutputRate("gold", value);
            UpdateUI();
        }


        public void Close()
        {
            if (minerOutput != null)
                minerOutput.CloseConfigUI();
        }
    }
}