using Unity.Entities;
using UnityEngine;

// A component to store physics state.
public struct Body : IComponentData
{
    public Vector3 Velocity;
    public Vector3 Acceleration;
    public float Damping;

    public float AngularVelocity;
    public float AngularAcceleration;
    public float AngularDamping;
}