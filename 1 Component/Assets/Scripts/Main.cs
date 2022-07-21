using System;
using UnityEngine;

// This is the "main" script
public class Main : MonoBehaviour
{
    private void Update()
    {
        // Create an en
        if (Input.GetMouseButtonUp(0))
        {
            CreateEntity();
        }
    }

    private static void CreateEntity()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var pos = Utility.GetMouseInWorldCoordinates();
        pos.z = -0.5f;
        go.transform.position = pos;
        
        go.AddComponent<Lifetime>();
        AddRandomMovementComponent(go);
    }

    private static void AddRandomMovementComponent(GameObject go)
    {
        Type[] movementComponents = {typeof(ChaseMouse), typeof(AvoidMouse), typeof(RandomWalk)};
        go.AddComponent(Utility.GetRandomElement(movementComponents));
    }
}
