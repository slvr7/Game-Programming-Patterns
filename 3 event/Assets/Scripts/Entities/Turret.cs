using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform muzzle;

    public GameObject bulletPrefab;
    
    private void Update()
    {
        // Point the turret towards the mouse
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        var target = (mousePos - transform.position).normalized;
        transform.right = Vector3.RotateTowards(transform.right, target, Mathf.PI * 2, 0);
        
        // Fire a bullet when player presses the space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var bullet = Instantiate(bulletPrefab);
            bullet.transform.rotation = transform.rotation;
            bullet.transform.position = muzzle.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EventManager.Instance.Fire(new PlayerDied());
        Destroy(gameObject);
    }
}