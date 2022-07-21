using UnityEngine;

[RequireComponent(typeof(Speed), typeof(Target))]
public class MoveToTarget : MonoBehaviour
{
    private void Update()
    {
        var speed = GetComponent<Speed>();
        Debug.Assert(speed != null);
        
        var target = GetComponent<Target>();
        Debug.Assert(target != null);
        
        var pos = transform.position;
        var offsetToTarget = Utility.GetXYOffset(pos, target.position);
        Vector3 offset = offsetToTarget.normalized * Time.deltaTime * speed.value;
        
        // We don't want to overshoot so we clamp the distance traveled to be 
        // no more than the distance to the target
        var distanceToTarget = offsetToTarget.magnitude;
        offset = Vector3.ClampMagnitude(offset, distanceToTarget);
        
        transform.position += offset;
    }
}