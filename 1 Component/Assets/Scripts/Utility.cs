using System;
using UnityEngine;

public static class Utility
{
    public static Vector3 GetMouseInWorldCoordinates()
    {
        var mainCamera = Camera.main;
        Debug.Assert(mainCamera != null);
        var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return pos;
    }

    public static T GetRandomElement<T>(T[] array)
    {
        Debug.Assert(array.Length > 0);
        var i = UnityEngine.Random.Range(0, array.Length);
        return array[i];
    }
    
    public static Vector2 GetXYOffset(Vector3 from, Vector3 to)
    {
        from.z = 0;
        to.z = 0;
        return to - from;
    }    
}