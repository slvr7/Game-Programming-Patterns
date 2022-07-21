using System.Collections;
using UnityEngine;

public class Main : MonoBehaviour
{
    public int numWanderers = 30;
    private readonly WanderManager _manager = new WanderManager();

    private void Start()
    {
        StartCoroutine(CheckWandererPopulation());
    }

    private void Update()
    {
        // Here's a boring but simple example of querying the manager...
        
        // Destroy all "old" wanderers when pressing the left mouse
        if (Input.GetMouseButtonDown(0))
        {
            _manager
                .FindAll(wanderer => wanderer.timeToLive < 5f)
                .ForEach(wanderer => _manager.Destroy(wanderer));
        }
    }

    // Manage the population
    private IEnumerator CheckWandererPopulation()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (_manager.Population < numWanderers)
            {
                var wanderer = _manager.Create();
                wanderer.transform.position = RandomVisiblePoint(Camera.main);
            }   
        }
    }

    private static Vector3 RandomVisiblePoint(Camera cam)
    {
        var point = cam.ViewportToWorldPoint(new Vector3(Random.value, Random.value));
        point.z = 0;
        return point;
    }
}