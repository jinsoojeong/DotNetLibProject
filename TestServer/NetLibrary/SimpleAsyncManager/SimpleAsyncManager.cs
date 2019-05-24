using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Collections.Concurrent;

namespace NetLibrary.SimpleAsyncManager
{
    public abstract class IAsyncJob
    {
        public abstract bool Init();
        public abstract bool Dowork();
        public abstract void Commit();
        public abstract void Rallback();

        public bool is_completed { get; internal set; } = false;
        public bool result { get; internal set; } = false;
    }

    public class CSimpleAsyncManager
    {
        Queue<IAsyncJob> queued_async_jobs;
        ConcurrentQueue<IAsyncJob> completed_queued_async_jobs;
        ConcurrentQueue<IAsyncJob> completed_nonqueued_async_jobs;

        private bool shutdown { get; set; } = false;
        private object lock_obj;

        private int async_job_count { get; set; } = 0;

        public CSimpleAsyncManager()
        {
            lock_obj = new object();
            queued_async_jobs = new Queue<IAsyncJob>();
            completed_queued_async_jobs = new ConcurrentQueue<IAsyncJob>();
            completed_nonqueued_async_jobs = new ConcurrentQueue<IAsyncJob>();

            async_job_count = 0;
            shutdown = false;
        }

        ~CSimpleAsyncManager()
        {
            return;
        }

        public void Stop()
        {
            shutdown = true;

            // 남은 작업 대기
            // Monitor.Wait 호출 후 동기화 객체에 대한 Monitor.Pulse 가 호출 되기 전까지 대기하게 됨
            // 여기서는 job_count 가 남은 상황에서 남은 작업 끝날 때 까지 대기하는 작업을 수행 (타임 아웃은 10초)
            lock (lock_obj)
            {
                if (async_job_count > 0)
                    Monitor.Wait(lock_obj, 10000);
            }

            queued_async_jobs.Clear();

            IAsyncJob remove_job = null;
            while (true)
            {
                if (completed_queued_async_jobs.TryDequeue(out remove_job) == false)
                    break;
            }

            remove_job = null;
            while (true)
            {
                if (completed_nonqueued_async_jobs.TryDequeue(out remove_job) == false)
                    break;
            }
        }

        public void Update()
        {
            if (shutdown)
                return;

            ProcessCompleteAsyncJob(completed_nonqueued_async_jobs);
            ProcessCompleteAsyncJob(completed_queued_async_jobs);
            ProcessQueueAsyncJob();
        }

        // no searialize job
        public void ExcuteNonQueue(IAsyncJob async_job)
        {
            if (shutdown)
                return;

            Run(async_job);
        }

        public void Excute(IAsyncJob async_job)
        {
            if (shutdown)
                return;

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

        public void IncreaseAsyncRunCount()
        {
            lock (lock_obj)
            {
                async_job_count++;
            }
        }

        public void DecreaseAsyncRunCount()
        {
            lock (lock_obj)
            {
                async_job_count--;

                if (shutdown && async_job_count == 0)
                    Monitor.Pulse(lock_obj);
            }
        }

        private async void Run(IAsyncJob async_job, bool is_queue_job = false)
        {
            // async_job 객체의 DoWork 함수를 비동기로 처리
            IncreaseAsyncRunCount();

            var async_task = Task.Run(() => async_job.Dowork());
            bool result = await async_task;

            DecreaseAsyncRunCount();
            AsyncJobComplete(async_job, is_queue_job, result);
        }
    }
}
