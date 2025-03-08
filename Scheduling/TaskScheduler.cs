using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Scheduling
{
public class TaskScheduler : MonoBehaviour
{
    private static TaskScheduler _instance;
    public static TaskScheduler Instance
    {
        get
        {
            if (_instance != null) return _instance;
            var taskScheduler = new GameObject("TaskScheduler");
            _instance = taskScheduler.AddComponent<TaskScheduler>();
            return _instance;
        }
    }

    private readonly List<ScheduledTask> _tasks = new List<ScheduledTask>();
    private readonly List<ScheduledTask> _tasksToCancel = new List<ScheduledTask>();

    private void Update()
    {
        for (var i = _tasks.Count - 1; i >= 0; i--)
        {
            var task = _tasks[i];

            if (task.IsCancelled)
            {
                continue;
            }

            task.Delay -= Time.deltaTime;

            if (!(task.Delay <= 0)) continue;
            task.ToExecute();
            task.ExecutedCount++;

            if (task.RepeatCount > 0 && task.ExecutedCount >= task.RepeatCount)
            {
                task.Cancel();
            }
            else
            {
                task.Delay = task.Interval;
            }
        }

        foreach (var task in _tasksToCancel)
        {
            _tasks.Remove(task);
        }

        _tasksToCancel.Clear();
    }

    public ScheduledTask Schedule(Action action, float delay)
    {
        ScheduledTask task = new() { ToExecute = action, Delay = delay, Interval = delay, RepeatCount = 1 };
        _tasks.Add(task);
        return task;
    }

    public ScheduledTask ScheduleRepeating(Action action, float interval, int repeatCount = -1)
    {
        ScheduledTask task = new() { ToExecute = action, Delay = interval, Interval = interval, RepeatCount = repeatCount };
        _tasks.Add(task);
        return task;
    }

    public void Cancel(ScheduledTask task)
    {
        if (task.IsCancelled) return;
        task.Cancel();
        _tasksToCancel.Add(task);
    }

    public void CancelAll()
    {
        foreach (var task in _tasks)
        {
            task.Cancel();
        }
    }
}
}