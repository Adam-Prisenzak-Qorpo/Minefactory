using System.Collections;
using System.Collections.Generic;
using Minefactory.Player;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public event System.Action OnPopulationChanged;
    public event System.Action<int> OnOxygenChanged;

    [SerializeField] private float currentOxygenTime;
    [SerializeField] private int currentOxygenSegments;
    

    private Dictionary<string, object> sharedState = new Dictionary<string, object>();
    [SerializeField] private int population;
    public Text populationText; // Reference to UI text
    public int Population
    {
        get { return population; }
        set 
        {
            Debug.Log($"Population changed from {population} to {value} by {Time.time}."); 
            population = value;
            UpdatePopulationText();
            OnPopulationChanged?.Invoke();
        }
    }

    public void UpdateOxygenState(int segments, float timePerSegment)
    {
        Debug.Log($"[GameStateManager] Updating oxygen state: segments={segments}, time={timePerSegment}");
        currentOxygenSegments = segments;
        currentOxygenTime = timePerSegment;
        OnOxygenChanged?.Invoke(segments);
    }

    public (int segments, float time) GetOxygenState()
    {
        return (currentOxygenSegments, currentOxygenTime);
    }

    private void Start()
    {
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