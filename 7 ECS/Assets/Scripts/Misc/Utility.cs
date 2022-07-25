using System;
using UnityEngine;

public static class Utility
{
    public static Bounds GetScreenBoundsInWorld()
    {
        var cam = Camera.main;
        if (cam == null) throw new Exception("To get screen bounds a main camera must be defined.");
        var bounds = new Bounds();
        var min = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        var max = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
        bounds.SetMinMax(min, max);
        return bounds;
    }
    
    public static Vector3 RandomPointInScreenBounds()
    {
        var cam = Camera.main;
        if (cam == null) throw new Exception("To get a random point in screen bounds a main camera must be defined.");
        var randomX = UnityEngine.Random.value;
        var randomY = UnityEngine.Random.value;
        var p = cam.ViewportToWorldPoint(new Vector3(randomX, randomY, 0));
        p.y = 0;
        return p;
    }
}