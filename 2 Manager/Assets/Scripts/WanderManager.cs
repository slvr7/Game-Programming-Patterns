using UnityEngine;

public class WanderManager : Manager<Wanderer>
{
    protected override Wanderer CreateObject()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var wanderer = go.AddComponent<Wanderer>();
        // When we create a Wanderer we let it know that this is its manager
        wanderer.Manager = this;
        return wanderer;
    }

    protected override void DestroyObject(Wanderer w)
    {
        GameObject.Destroy(w.gameObject);
    }
}