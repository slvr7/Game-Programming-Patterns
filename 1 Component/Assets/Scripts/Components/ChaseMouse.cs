using UnityEngine;

[RequireComponent(typeof(Target), typeof(MoveToTarget))]
public class ChaseMouse : MonoBehaviour
{
    private void Update()
    {
        // Get the target 
        var target = GetComponent<Target>();
        // This component only works with
        // objects that have targets.
        // Throw an error if one isn't available.
        Debug.Assert(target != null);
        target.position = Utility.GetMouseInWorldCoordinates();
        target.position.z = transform.position.z;
    }
}