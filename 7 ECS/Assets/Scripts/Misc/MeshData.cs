using UnityEngine;

[CreateAssetMenu(fileName = "MeshData")]
public class MeshData : ScriptableObject
{
    public Mesh Mesh;
    public Material Material;

    public RenderData GenerateRenderData()
    {
        return new RenderData(Mesh, Material);
    }
}