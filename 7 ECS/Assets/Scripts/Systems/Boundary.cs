
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(PhysicsSystem))]
public class Boundary : ComponentSystem
{
    private EntityQuery _positions;
    private Bounds _screenBounds;
    
    protected override void OnStartRunning()
    {
        _positions = GetEntityQuery(ComponentType.ReadOnly<Position>());
        _screenBounds = Utility.GetScreenBoundsInWorld();
    }

    protected override void OnUpdate()
    {
        Entities.With(_positions).ForEach((ref Position pos) =>
        {
            var val = pos.Value;
            pos.Value.x = Mathf.Clamp(val.x, _screenBounds.min.x, _screenBounds.max.x);
            pos.Value.z = Mathf.Clamp(val.z, _screenBounds.min.z, _screenBounds.max.z);
        });
    }
}