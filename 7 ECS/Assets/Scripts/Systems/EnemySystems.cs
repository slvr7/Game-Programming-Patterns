using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class EnemyCreateSystem : ComponentSystem
{
    private RenderData _enemyRenderData;
    private EntityArchetype _enemyArchetype;
    private EntityQuery _enemies;


    protected override void OnStartRunning()
    {
        _enemyRenderData = Resources.Load<MeshData>("EnemyMesh").GenerateRenderData();
        _enemies = GetEntityQuery(ComponentType.ReadOnly<Enemy>());
        _enemyArchetype = World.EntityManager.CreateArchetype(
            ComponentType.ReadOnly<RenderData>(),
            ComponentType.ReadOnly<Enemy>(),
            ComponentType.ReadWrite<Body>(),
            ComponentType.ReadWrite<Rotation>(),
            ComponentType.ReadWrite<Position>()
            ); 
    }

    protected override void OnUpdate()
    {
        if (_enemies.CalculateEntityCount() < 5)
        {
            CreateEnemy();
        }
    }

    private void CreateEnemy()
    {
        var em = World.EntityManager;
        var e = em.CreateEntity(_enemyArchetype);
        
        em.SetSharedComponentData(e, _enemyRenderData);
        em.SetComponentData(e, new Position {Value = Utility.RandomPointInScreenBounds()});
        em.SetComponentData(e, new Body{AngularDamping = 0.1f});
    }
}

public class EnemyMovementSystem : ComponentSystem
{
    private FloatValue _maxSpeed;
    
    private EntityQuery _enemies;
    private EntityQuery _players;

    protected override void OnStartRunning()
    {
        _maxSpeed = Resources.Load<FloatValue>("MaxEnemySpeed");
        
        _enemies = GetEntityQuery(
            ComponentType.ReadOnly<Enemy>(),            
            ComponentType.ReadWrite<Body>(),
            ComponentType.ReadWrite<Rotation>(),
            ComponentType.ReadWrite<Position>());
        
        _players = GetEntityQuery(
            ComponentType.ReadOnly<Player>(),
            ComponentType.ReadOnly<Position>());
    }
    
    protected override void OnUpdate()
    {
        if (_players.CalculateEntityCount() < 1) return;

        var player = GetPlayer();
        var playerPos = World.EntityManager.GetComponentData<Position>(player);

        Entities.With(_enemies).ForEach((ref Body body, ref Rotation rotation, ref Position position) =>
        {
            var v1 = rotation.Direction();
            var v2 = (playerPos.Value - position.Value).normalized;
            var angle = Vector3.SignedAngle(v1, v2, Vector3.up);
            if (angle > 0) body.AngularAcceleration = 500;
            else body.AngularAcceleration = -500;
            
            body.Acceleration += v2 * _maxSpeed.Value - body.Velocity;
        });
    }

    private Entity GetPlayer()
    {
        var playerArray = _players.ToEntityArray(Allocator.Persistent);
        var player = playerArray[0];
        playerArray.Dispose();
        return player;
    }
}