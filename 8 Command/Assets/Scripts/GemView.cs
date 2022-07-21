using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GPP
{
    public class GemView : MonoBehaviour
    {
        private float _idlePeriod;

        private void Start()
        {
            _idlePeriod = Random.Range(10.0f, 100.0f);
            StartCoroutine(IdleAnimation());
        }

        private IEnumerator IdleAnimation()
        {
            var t = 0.0f;
            while (Application.isPlaying)
            {
                var s = Mathf.Sin(t / _idlePeriod ) * Mathf.Rad2Deg * 0.5f;
                transform.rotation = Quaternion.Euler(0, s, 0);
                t += Time.deltaTime;
                yield return null;
            }
        }

    }
}
