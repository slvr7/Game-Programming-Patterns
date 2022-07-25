
using Unity.Entities;
using UnityEngine;
using static UnityEngine.Mathf;

public class PhysicsSystem : ComponentSystem
{

    private const float AngularEpsilon = 0.0001f;
    private const float VelocityEpsilon = 0.0001f;
    
    private EntityQuery _physicsObjects;

    protected override void OnStartRunning()
    {
        _physicsObjects = GetEntityQuery(
            ComponentType.ReadWrite<Body>(), 
            ComponentType.ReadWrite<Position>(), 
            ComponentType.ReadWrite<Rotation>());
    }

    protected override void OnUpdate()
    {
        var dt = Time.fixedDeltaTime;
        Entities.With(_physicsObjects).ForEach((ref Body body, ref Position position, ref Rotation rotation) =>
        {
            // Handle rotation
            body.AngularVelocity += body.AngularAcceleration * dt;
            body.AngularAcceleration = 0;
            body.AngularVelocity *= 1 - body.AngularDamping;
            if (Abs(body.AngularVelocity) < AngularEpsilon) body.AngularVelocity = 0;
            rotation.Angle += body.AngularVelocity * dt;

            // Handle position
            body.Velocity += body.Acceleration * dt;
            body.Acceleration = Vector3.zero;
            body.Velocity *= 1 - body.Damping;
            if (Abs(body.Velocity.sqrMagnitude) < VelocityEpsilon) body.Velocity = Vector3.zero;
            position.Value += body.Velocity * dt;
        });
    }
}
