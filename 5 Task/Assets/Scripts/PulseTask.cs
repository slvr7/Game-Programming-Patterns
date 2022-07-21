using UnityEngine;

public class PulseTask : GPP.Task
{
    protected readonly GameObject _obj;
    private readonly float _scale;
    private readonly float _duration;
    private readonly int _times;

    private float _elapsedTime;

    public PulseTask(GameObject o, float scale, float duration, int times)
    {
        _obj = o;
        _duration = duration;
        _times = times;
        _scale = scale;
    }

    protected override void Init()
    {
        Debug.Log("Starting pulse");
        _obj.transform.localScale = Vector3.one;
    }

    protected override void CleanUp()
    {
        Debug.Log("Ending pulse");
        _obj.transform.localScale = Vector3.one;
    }

    internal override void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _duration)
        {
            SetStatus(TaskStatus.Success);
        }
        else
        {
            var t = _elapsedTime / _duration;
            var s = (1 - Mathf.Abs(Mathf.Cos(t * Mathf.PI * _times))) * _scale + 1;
            _obj.transform.localScale = new Vector3(s, s, s);
        }
    }
}