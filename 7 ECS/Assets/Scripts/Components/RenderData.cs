using System;
using Unity.Entities;
using UnityEngine;

// A shared component to store rendering information such as mesh and material
public struct RenderData : ISharedComponentData, IEquatable<RenderData>
{
    public readonly Mesh Mesh;
    public readonly Material Material;

    public RenderData(Mesh mesh, Material material)
    {
        Mesh = mesh;
        Material = material;
    }

    public bool Equals(RenderData other)
    {
        return Equals(Mesh, other.Mesh) && Equals(Material, other.Material);
    }

    public override bool Equals(object obj)
    {
        return obj is RenderData other && Equals(other);
    }

    
    public override int GetHashCode()
    {
        unchecked
        {
            return ((Mesh != null ? Mesh.GetHashCode() : 0) * 397) ^ (Material != null ? Material.GetHashCode() : 0);
        }
    }
}

// Shared components are used when you need to reuse the same data
// in multiple entities.

// Since render data is identical for each entity of a given type (e.g.
// every enemy has the same mesh and material) it makes sense to only have  
// one instance of that data shared among all the entities that need it.

// Also shared components are the only ones that are allowed to store
// references (as opposed to blittable(1) values like int or float)
// so if we need to use a managed(2) object like a mesh we need to put it
// in a shared component.

// 1) Blittable types are ones that can be copied just by copying their
// memory bit-for-bit. Blittable types basically correspond to value types
// but you can get a full list here:
// https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types 

// 2) Managed objects are ones whose memory is managed by C#'s garbage
// collector. Basically anything that isn't a primitive like int or byte
// or a struct. 