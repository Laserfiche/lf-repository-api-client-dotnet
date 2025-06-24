// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
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
        /// <param name="operationId">Operation Id</param>
        /// <param name="timeout">Time to wait for operation to reach expected status</param>
        /// <param name="handleOperationProgress">Action called for each task progress</param>
        /// <param name="expectedOperationStatus">Expected Task status, defaults to TaskStatus.Completed.</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        Task<OperationProgress> WaitForTaskAsync(string repositoryId, string operationId, TimeSpan timeout, Action<OperationProgress> handleOperationProgress = null, OperationStatus expectedOperationStatus = OperationStatus.Completed, CancellationToken cancellationToken = default);
    }

    partial class TasksClient : ITasksClient
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public async Task<OperationProgress> WaitForTaskAsync(string repositoryId, string operationId, TimeSpan timeout, Action<OperationProgress> handleOperationProgress = null, OperationStatus expectedOperationStatus = OperationStatus.Completed, CancellationToken cancellationToken = default)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (true)
            {
                var operationStatus = await GetOperationStatusAndProgressAsync(repositoryId, operationId, cancellationToken).ConfigureAwait(false);

                if (handleOperationProgress != null) handleOperationProgress(operationStatus);

                if (operationStatus.Status == expectedOperationStatus)
                {
                    return operationStatus;
                }
                else if (expectedOperationStatus == OperationStatus.Completed && operationStatus.Status == OperationStatus.Failed)
                {
                    throw new Exception($"Expected task to complete, but operation with id {operationId} failed after {sw.Elapsed}.");
                }
                else if (expectedOperationStatus == OperationStatus.Failed && operationStatus.Status == OperationStatus.Completed)
                {
                    throw new Exception($"Expected task to fail, but operation with id {operationId} completed successfully after {sw.Elapsed}.");
                }
                else if (sw.Elapsed > timeout)
                {
                    throw new Exception($"Waiting for task {operationStatus.OperationType} to be {expectedOperationStatus} timed out after {sw.Elapsed}.");
                }
            }
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}