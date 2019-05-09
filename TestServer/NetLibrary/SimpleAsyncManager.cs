using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Concurrent;

namespace SimpleAsyncManager
{
    public abstract class IAsyncJob
    {
        public abstract bool Init();
        public abstract bool Dowork();
        public abstract void Commit();
        public abstract void Rallback();

        public bool is_completed { get; set; } = false;
        public bool result { get; set; } = false;
    }

    public class CSimpleAsyncManager
    {
        Queue<IAsyncJob> queued_async_jobs;
        ConcurrentQueue<IAsyncJob> completed_queued_async_jobs;
        ConcurrentQueue<IAsyncJob> completed_nonqueued_async_jobs;

        public CSimpleAsyncManager()
        {
            queued_async_jobs = new Queue<IAsyncJob>();
            completed_queued_async_jobs = new ConcurrentQueue<IAsyncJob>();
            completed_nonqueued_async_jobs = new ConcurrentQueue<IAsyncJob>();
        }

        public void Update()
        {
            ProcessCompleteAsyncJob(completed_nonqueued_async_jobs);
            ProcessCompleteAsyncJob(completed_queued_async_jobs);
            ProcessQueueAsyncJob();
        }

        // no searialize job
        public void ExcuteNonQueue(IAsyncJob async_job)
        {
            Run(async_job);
        }

        public void Excute(IAsyncJob async_job)
        {
            queued_async_jobs.Enqueue(async_job);
        }

        private void ProcessCompleteAsyncJob(ConcurrentQueue<IAsyncJob> complete_async_jobs)
        {
            if (complete_async_jobs.IsEmpty == false)
            {
                for (int count = complete_async_jobs.Count; count != 0; --count)
                {
                    IAsyncJob async_job;
                    if (complete_async_jobs.TryDequeue(out async_job) == false)
                        break;

                    if (async_job == null)
                        break;

                    if (async_job.result)
                        async_job.Commit();
                    else
                        async_job.Rallback();
                }
            }
        }

        private void ProcessQueueAsyncJob()
        {
            for (int count = queued_async_jobs.Count; count != 0; --count)
            {
                IAsyncJob async_job;
                async_job = queued_async_jobs.Dequeue();

                if (async_job.Init())
                    Run(async_job, true);
                else
                    AsyncJobComplete(async_job, true, false);
            }
        }

        private void AsyncJobComplete(IAsyncJob async_job, bool is_queue_job, bool result)
        {
            async_job.is_completed = true;
            async_job.result = result;

            if (is_queue_job)
                completed_queued_async_jobs.Enqueue(async_job);
            else
                completed_nonqueued_async_jobs.Enqueue(async_job);
        }

        private async void Run(IAsyncJob async_job, bool is_queue_job = false)
        {
            var async_task = Task.Run(() => async_job.Dowork());
            bool result = await async_task;

            AsyncJobComplete(async_job, is_queue_job, result);
        }
    }
}
