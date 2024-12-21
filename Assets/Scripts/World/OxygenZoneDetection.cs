using UnityEngine;
using Minefactory.Player;
using System.Collections;

public class OxygenZone : MonoBehaviour
{
    public int oxygenReplenishAmount = 5;  // Amount of oxygen to add (in seconds)
    public float refillTimePerSegment = 1f; // Time (in seconds) to refill one segment, adjustable in Inspector
    private Coroutine refillCoroutine;  // Keep track of the coroutine
    private bool isInZone = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInZone = true;
            OxygenManager.Instance?.SetOxygenZoneState(true);
            if (refillCoroutine != null)
            {
                StopCoroutine(refillCoroutine);
            }
            refillCoroutine = StartCoroutine(GradualOxygenRefill(OxygenManager.Instance));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInZone = false;
            if (refillCoroutine != null)
            {
                StopCoroutine(refillCoroutine);
                refillCoroutine = null;
            }
            OxygenManager.Instance?.SetOxygenZoneState(false);
        }
    }

    private IEnumerator GradualOxygenRefill(OxygenManager oxygenManager)
    {
        if (oxygenManager == null) yield break;

        while (isInZone && oxygenManager.CurrentOxygen < oxygenManager.totalOxygenSegments)
        {
            yield return new WaitForSeconds(refillTimePerSegment);
            if (isInZone) // Double check we're still in zone
            {
                oxygenManager.ReplenishOxygen(1);
            }
        }
        
        refillCoroutine = null;
    }
}
