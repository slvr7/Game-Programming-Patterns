using Unity.Entities;
using UnityEngine;

// Lifecycle
class PlayerCreateSystem : ComponentSystem
{

    private FloatValue _damping, _angularDamping;
    private RenderData _playerRenderData;
    private EntityArchetype _playerArchetype;
    private EntityQuery _players;

    protected override void OnStartRunning()
    {
        _damping = Resources.Load<FloatValue>("PlayerDamping");
        _angularDamping = Resources.Load<FloatValue>("PlayerAngularDamping");
        
        var meshData = Resources.Load<MeshData>("PlayerMesh");
        _players = GetEntityQuery(ComponentType.ReadWrite<Player>());
        _playerRenderData = new RenderData(meshData.Mesh, meshData.Material);
        _playerArchetype = World.EntityManager.CreateArchetype(
            ComponentType.ReadWrite<Player>(),
            ComponentType.ReadOnly<RenderData>(),
            ComponentType.ReadWrite<Body>(),
            ComponentType.ReadWrite<Position>(),
            ComponentType.ReadWrite<Rotation>());
    }

    protected override void OnUpdate()
    {
        // If there is no player, create one
        if (_players.CalculateEntityCount() == 0)
        {
            CreatePlayer();
        }
    }

    private void CreatePlayer()
    {
        var em = World.EntityManager;

        var e = em.CreateEntity(_playerArchetype);
        
        em.SetSharedComponentData(e, _playerRenderData);
        em.SetComponentData(e, new Body
        {
            Damping = _damping.Value,
            AngularDamping = _angularDamping.Value
        });
        em.SetComponentData(e, new Position());
        em.SetComponentData(e, new Rotation());
    }
}

// 
class PlayerRotateSystem : ComponentSystem
{
    private FloatValue _rotation;

    private EntityQuery _players;
    
    protected override void OnStartRunning()
    {
        _rotation = Resources.Load<FloatValue>("PlayerRotation");
        _players = GetEntityQuery(ComponentType.ReadWrite<Body>(), ComponentType.ReadWrite<Player>());
    }

    protected override void OnUpdate()
    {
        var isLeftPressed = Input.GetKey(KeyCode.A);
        var isRightPressed = Input.GetKey(KeyCode.D);
        
        if (!(isLeftPressed||isRightPressed)) return;

        Entities.With(_players)
            .ForEach((ref Body body) =>
            {
                if (isLeftPressed) body.AngularAcceleration += -_rotation.Value;
                else body.AngularAcceleration += _rotation.Value;
            });
    }
}

class PlayerThrustSystem : ComponentSystem
{
    private FloatValue _thrust;
    private EntityQuery _players; 
    
    protected override void OnStartRunning()
    {
        _thrust = Resources.Load<FloatValue>("PlayerThrust");
        _players = GetEntityQuery(ComponentType.ReadWrite<Player>(), ComponentType.ReadWrite<Body>(), ComponentType.ReadWrite<Rotation>());
    }

    protected override void OnUpdate()
    {
        if (!Input.GetKey(KeyCode.W)) return;
        Entities.With(_players)
            .ForEach((ref Body body, ref Rotation rotation) =>
            {
                body.Acceleration += rotation.Direction() * _thrust.Value;
            });
    }
}

// 