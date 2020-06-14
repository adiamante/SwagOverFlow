using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SwagOverFlow.Utils
{
    public class ConcurrentOrderedTasks : IDisposable
    {
        #region Properties
        public Int32 MaxConcurrency { get; set; } = Environment.ProcessorCount;
        public ConcurrentDictionary<int, Thread> Threads { get; } = new ConcurrentDictionary<int, Thread>();
        public BlockingCollection<Task> Tasks { get; } = new BlockingCollection<Task>();
        public BlockingCollection<Task> CompletedTasks { get; } = new BlockingCollection<Task>();
        #endregion Properties

        #region Initialization
        public ConcurrentOrderedTasks()
        {

        }

        public ConcurrentOrderedTasks(Int32 maxConcurrency)
        {
            MaxConcurrency = maxConcurrency;
        }
        #endregion Initialization

        #region Methods
        public void Append(Task task)
        {
            Tasks.Add(task);
        }

        public void AddThread()
        {
            Thread thread = new Thread(DoTask);
            thread.Start();
            Threads.TryAdd(thread.ManagedThreadId, thread);
        }

        public void Execute()
        {
            Int32 numThreadsToCreate = Math.Min(MaxConcurrency, Tasks.Count) - Threads.Count;
            while (numThreadsToCreate > 0)
            {
                AddThread();
                numThreadsToCreate--;
            }

            foreach (KeyValuePair<int, Thread> threadKVP in Threads)
            {
                threadKVP.Value.Join();
            }
        }

        public void DoTask()
        {
            while (Tasks.TryTake(out Task task))
            {
                task.Start();
                task.Wait();
                CompletedTasks.Add(task);
            }
            Threads.TryRemove(Thread.CurrentThread.ManagedThreadId, out Thread thread);
        }

        public void Dispose()
        {
            Tasks.CompleteAdding();
        }
        #endregion Methods
    }

    public static class ConcurrentOrderedTasksHelper
    {
        #region GetResult
        public static object GetResult(this Task task)
        {
            return task.GetType().GetProperty("Result").GetValue(task, null);
        }

        public static T GetResult<T>(this Task task)
        {
            Task<T> t = task as Task<T>;
            return t.Result;
        }
        #endregion GetResult
    }
}
