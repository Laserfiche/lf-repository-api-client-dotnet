// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    partial interface ITasksClient
    {
        /// <summary>
        /// Waits for task to have expected status until timeout.
        /// </summary>
        /// <param name="repositoryId">Repository Id</param>
        /// <param name="taskId">Operation Id</param>
        /// <param name="timeout">Time to wait for operation to reach expected status</param>
        /// <param name="handleTaskProgress">Action called for each task progress</param>
        /// <param name="expectedTaskStatus">Expected Task status, defaults to TaskStatus.Completed.</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        Task<TaskProgress> WaitForTaskAsync(string repositoryId, string taskId,  TimeSpan timeout, Action<TaskProgress> handleTaskProgress = null, TaskStatus expectedTaskStatus = TaskStatus.Completed, CancellationToken cancellationToken = default);
    }

    partial class TasksClient : ITasksClient
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public async Task<TaskProgress> WaitForTaskAsync(string repositoryId, string taskId, TimeSpan timeout, Action<TaskProgress> handleTaskProgress = null, TaskStatus expectedTaskStatus = TaskStatus.Completed, CancellationToken cancellationToken = default)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (true)
            {
                var taskProgressList = await ListTasksAsync(new ListTasksParameters()
                {
                    RepositoryId = repositoryId,
                    TaskIds = new List<string> { taskId }
                }, cancellationToken).ConfigureAwait(false);

                TaskProgress taskProgress = taskProgressList.Value.First(element => element.Id.Equals(taskId));
                handleTaskProgress(taskProgress);

                if (taskProgress.Status == expectedTaskStatus)
                {
                    return taskProgress;
                }
                else if (expectedTaskStatus == TaskStatus.Completed && taskProgress.Status == TaskStatus.Failed)
                {
                    throw new Exception($"Expected task to complete, but operation with id {taskId} failed after {sw.Elapsed}.");
                }
                else if (expectedTaskStatus == TaskStatus.Failed && taskProgress.Status == TaskStatus.Completed)
                {
                    throw new Exception($"Expected task to fail, but operation with id {taskId} completed successfully after {sw.Elapsed}.");
                }
                else if (sw.Elapsed > timeout)
                {
                    throw new Exception($"Waiting for task {taskProgress.TaskType} to be {expectedTaskStatus} timed out after {sw.Elapsed}.");
                }
            }
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
