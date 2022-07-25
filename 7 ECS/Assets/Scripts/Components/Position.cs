using Unity.Entities;
using UnityEngine;

// A component to store position
public struct Position : IComponentData
{
    public Vector3 Value;
}