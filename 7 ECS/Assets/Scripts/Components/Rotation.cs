using Unity.Entities;
using UnityEngine;

public struct Rotation : IComponentData
{
    public float Angle;
    
    public Vector3 Direction()
    {
        return Quaternion.Euler(0, Angle, 0) * Vector3.forward;
    }
}