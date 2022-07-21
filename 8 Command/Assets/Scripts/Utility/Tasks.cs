using System;
using System.Collections.Generic;

public abstract class Task
{
    public enum TaskStatus : byte
    {
        Detached, // Task has not been attached to a TaskManager
        Pending, // Task has not been initialized
        Working, // Task has been initialized
        Success, // Task completed successfully
        Fail, // Task completed unsuccessfully
        Aborted // Task was aborted
    }

    private TaskStatus _status = TaskStatus.Detached;

    public TaskStatus Status
    {
        get { return _status; }
        set { _status = value; }
    }

    // Convenience status checking
    public bool IsDetached
    {
        get { return Status == TaskStatus.Detached; }
    }

    public bool IsAttached
    {
        get { return Status != TaskStatus.Detached; }
    }

    public bool IsPending
    {
        get { return Status == TaskStatus.Pending; }
    }

    public bool IsWorking
    {
        get { return Status == TaskStatus.Working; }
    }

    public bool IsSuccessful
    {
        get { return Status == TaskStatus.Success; }
    }

    public bool IsFailed
    {
        get { return Status == TaskStatus.Fail; }
    }

    public bool IsAborted
    {
        get { return Status == TaskStatus.Aborted; }
    }

    public bool IsFinished
    {
        get { return (Status == TaskStatus.Fail || Status == TaskStatus.Success || Status == TaskStatus.Aborted); }
    }

    public void Abort()
    {
        SetStatus(TaskStatus.Aborted);
    }

    internal void SetStatus(TaskStatus newStatus)
    {
        if (Status == newStatus) return;

        Status = newStatus;

        switch (newStatus)
        {
            case TaskStatus.Working:
                Init();
                break;

            case TaskStatus.Success:
                OnSuccess();
                CleanUp();
                break;

            case TaskStatus.Aborted:
                OnAbort();
                CleanUp();
                break;

            case TaskStatus.Fail:
                OnFail();
                CleanUp();
                break;

            case TaskStatus.Detached:
            case TaskStatus.Pending:
                break;

            default:
                throw new ArgumentOutOfRangeException(newStatus.ToString(), newStatus, null);
        }
    }

    protected virtual void OnAbort()
    {
    }

    protected virtual void OnSuccess()
    {
    }

    protected virtual void OnFail()
    {
    }

    protected virtual void Init()
    {
    }

    protected virtual void CleanUp()
    {
    }

    internal virtual void Update()
    {
    }
}

public abstract class TaskCollection : Task
{
    protected List<Task> _tasks = new List<Task>();

    public bool HasTasks
    {
        get { return _tasks.Count > 0; }
    }

    public int NumTasks
    {
        get { return _tasks.Count; }
    }

    private readonly bool _stopOnFailOrAbort;

    public bool StopOnFailOrAbort
    {
        get { return _stopOnFailOrAbort; }
    }

    protected TaskCollection(bool stopOnFailOrAbort = false)
    {
        _stopOnFailOrAbort = stopOnFailOrAbort;
    }

    public void Add(Task task)
    {
        SetStatus(TaskStatus.Pending);
        task.SetStatus(TaskStatus.Pending);
        _tasks.Add(task);
    }

    public void Add(params Task[] tasks)
    {
        foreach (var task in tasks)
        {
            Add(task);
        }
    }

    public void Clear()
    {
        foreach (var t in _tasks)
        {
            t.Abort();
            t.SetStatus(TaskStatus.Detached);
        }

        _tasks.Clear();
    }

    protected void ProcessTask(int index)
    {
        var task = _tasks[index];
        if (task.IsPending)
        {
            task.SetStatus(TaskStatus.Working);
        }

        if (task.IsFinished)
        {
            HandleCompletion(task, index);
        }
        else
        {
            task.Update();
            if (task.IsFinished)
            {
                HandleCompletion(task, index);
            }
        }
    }

    private void HandleCompletion(Task task, int index)
    {
        if ((task.IsFailed || task.IsAborted) && StopOnFailOrAbort)
        {
            SetStatus(task.Status);
            Clear();
        }
        else
        {
            _tasks.RemoveAt(index);
            task.SetStatus(TaskStatus.Detached);
        }
    }
}

// Processes the tasks sequentially
public class SerialTasks : TaskCollection
{
    public Task ActiveTask
    {
        get { return HasTasks ? _tasks[0] : null; }
    }

    internal override void Update()
    {
        if (HasTasks)
        {
            ProcessTask(0);
            if (!HasTasks)
                SetStatus(TaskStatus.Success);
        }
    }
}

// Processes tasks in parallel
public class ParallelTasks : TaskCollection
{
    internal override void Update()
    {
        for (var i = _tasks.Count - 1; i >= 0; i--)
        {
            ProcessTask(i);
        }

        if (!HasTasks)
            SetStatus(TaskStatus.Success);
    }
}