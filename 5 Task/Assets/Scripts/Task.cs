using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPP
{
    public abstract class Task
    {
        public enum TaskStatus : byte
        {
            Pending, // Task has not been initialized
            Working, // Task has been initialized
            Success, // Task completed successfully
            Failed, // Task completed unsuccessfully
            Aborted // Task was aborted
        }

        private TaskStatus _status = TaskStatus.Pending;
        public TaskStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        // Convenience status checking
        public bool IsPending { get { return Status == TaskStatus.Pending; } }
        public bool IsWorking { get { return Status == TaskStatus.Working; } }
        public bool IsSuccessful { get { return Status == TaskStatus.Success; } }
        public bool IsFailed { get { return Status == TaskStatus.Failed; } }
        public bool IsAborted { get { return Status == TaskStatus.Aborted; } }
        public bool IsFinished { get { return (Status == TaskStatus.Failed || Status == TaskStatus.Success || Status == TaskStatus.Aborted); } }

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

                case TaskStatus.Failed:
                    OnFail();
                    CleanUp();
                    break;

                case TaskStatus.Pending:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(newStatus.ToString(), newStatus, null);
            }
        }

        protected virtual void OnAbort() {}
        protected virtual void OnSuccess() {}
        protected virtual void OnFail() {}
        protected virtual void Init() {}
        protected virtual void CleanUp() {}

        internal virtual void Update() {}
    }

    public abstract class TaskRunner : Task
    {
        protected readonly List<Task> Tasks = new List<Task>();

        private readonly List<Task> _pendingAdd = new List<Task>();

        private readonly List<Task> _pendingRemove = new List<Task>();

        public bool HasTasks {get { return Tasks.Count > 0; }}

        public T GetTask<T>() where T : Task
        {
            foreach (var task in Tasks)
            {
                if (task.GetType() == typeof(T)) return (T)task;
            }
            return null;
        }

        public bool HasTask<T>() where T : Task
        {
            return GetTask<T>() != null;
        }

        public void Add(Task task)
        {
            SetStatus(TaskStatus.Working);
            task.SetStatus(TaskStatus.Pending);
            _pendingAdd.Add(task);
        }

        private void HandleCompletion(Task task)
        {
            _pendingRemove.Add(task);
            task.SetStatus(TaskStatus.Pending);
            if (Tasks.Count == 0)
            {
                SetStatus(TaskStatus.Success);
            }
        }

        protected void PostUpdate()
        {
            foreach (var task in _pendingRemove)
            {
                Tasks.Remove(task);
            }
            _pendingRemove.Clear();

            foreach (var task in _pendingAdd)
            {
                Tasks.Add(task);
            }
            _pendingAdd.Clear();

            if (!HasTasks)
            {
                SetStatus(TaskStatus.Success);
            }

        }

        public void Clear()
        {
            foreach (var t in Tasks)
            {
                t.Abort();
            }
        }

        protected void ProcessTask(Task task)
        {
            if (task.IsPending)
            {
                task.SetStatus(TaskStatus.Working);
            }

            if (task.IsFinished)
            {
                HandleCompletion(task);
            }
            else
            {
                task.Update();
                if (task.IsFinished)
                {
                    HandleCompletion(task);
                }
            }
        }

        protected override void OnAbort()
        {
            foreach (var task in Tasks)
            {
                task.Abort();
            }
        }
    }

    public class SerialTasks : TaskRunner
    {
        internal override void Update()
        {
            if (HasTasks)
            {
                var first = Tasks[0];
                ProcessTask(first);
            }
            PostUpdate();
        }
    }

    // This runs
    public class ParallelTasks : TaskRunner
    {
        internal override void Update()
        {
            foreach (var task in Tasks)
            {
                ProcessTask(task);
            }
            PostUpdate();
        }
    }

}