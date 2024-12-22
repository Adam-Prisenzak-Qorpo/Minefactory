using System.Collections;
using System.Collections.Generic;
using Minefactory.Player;
using UnityEngine;
using UnityEngine.UI;
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public event System.Action OnPopulationChanged;
    public event System.Action<int> OnOxygenChanged;
    

    private Dictionary<string, object> sharedState = new Dictionary<string, object>();
    [SerializeField] private int population;
    public Text populationText; // Reference to UI text
    public int Population
    {
        get { return population; }
        set 
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            Debug.Log($"Population changed from {population} to {value} at time {Time.time}\nCalled from:\n{stackTrace}"); 
            population = value;
            OnPopulationChanged?.Invoke();
            UpdatePopulationText();
        }
    }

    private void Start()
    {
        Debug.Log($"Population at the start: {population}");
        UpdatePopulationText();  
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void UpdatePopulationText()
    {
        if (populationText != null)
        {
            populationText.text = "Population: " + population;
        }
    }

    // Set shared state for skills or other changes
    public void SetSharedState(string key, object value)
    {
        sharedState[key] = value;
    }

    // Get shared state when a world activates
    public T GetSharedState<T>(string key, T defaultValue = default)
    {
        if (sharedState.TryGetValue(key, out object value))
        {
            return (T)value;
        }
        return defaultValue;
    }
}