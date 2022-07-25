using Unity.Entities;
using UnityEngine;

public class RenderSystem : ComponentSystem
{
    private EntityQuery _renderables;
    
    protected override void OnStartRunning()
    {
        _renderables = GetEntityQuery(
            ComponentType.ReadOnly<RenderData>(),
            ComponentType.ReadOnly<Position>(),
            ComponentType.ReadOnly<Rotation>());
    }

    protected override void OnUpdate()
    {
        Entities.With(_renderables).ForEach((RenderData renderData, ref Position position, ref Rotation rotation) =>
        {
            var mesh = renderData.Mesh;
            var mat = renderData.Material;
            var pos = position.Value;
            var rot = Quaternion.Euler(0, rotation.Angle, 0);
            Graphics.DrawMesh(mesh, pos, rot, mat,0);
        });
    }
}