using System.Collections;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    public float timeLeft = 10;// in seconds
    
    private IEnumerator Countdown()
    {
        while (timeLeft >=0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        Destroy(this);
        gameObject.AddComponent<Disappear>();
    }    

    private void Start()
    {
        StartCoroutine(Countdown());
    }
}