using System.Collections;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    public float duration = 1;

    private IEnumerator Shrink()
    {
        while (duration >= 0)
        {
            duration -= Time.deltaTime;
            transform.localScale = transform.localScale * 0.9f;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(Shrink());
    }
}