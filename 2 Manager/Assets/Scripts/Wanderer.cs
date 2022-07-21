using UnityEngine;

public class Wanderer : MonoBehaviour
{
    // When objects are managed it's often useful
    // to have them hold a reference to the 
    // manager that's responsible for them
    // so they can do things like tell the 
    // manager when they should be destroyed
    public WanderManager Manager { get; set; }
    
    public float speed = 0.5f;
    public float leadDistance = 5;
    public float wanderRadius = 2;
    public  float rotationRange = 10;
    public float lifetime = 10;
    public float timeToLive;
    private float _wanderAngle;
    private Vector3 _direction;
    
    private void Start()
    {
        timeToLive = lifetime;
        _wanderAngle = Random.value * 360;
        _direction = Quaternion.Euler(0, 0, Random.value * 360) * transform.right;
    }

    private void Update()
    {


        timeToLive -= Time.deltaTime;
        if (timeToLive < lifetime * 0.5f)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if (timeToLive > 0)
        {
            Shrink();
            Wander();
        }
        else
        {
            // Notice that we are not destroying ourselves directly
            // but telling the manager do it
            Manager.Destroy(this);
        }

    }
    
    private void Shrink()
    {
        var scale = timeToLive / lifetime;
        transform.localScale = new Vector3(scale, scale, scale);
    }
    
    private void Wander()
    {
        _wanderAngle += Random.Range(-rotationRange, rotationRange);
        var wanderOffset = Quaternion.Euler(0, 0, _wanderAngle) * new Vector3(wanderRadius, 0, 0);
        var toTarget = _direction * leadDistance + wanderOffset;
        _direction = toTarget.normalized;
        transform.position += _direction * Time.deltaTime * speed;
    }

}