﻿using System;
using System.Threading.Tasks;
using System.Threading;

namespace AllOverIt.Threading
{
    /// <summary>Represents a lock that limits the number of threads that can access a resource within an asynchronous context.</summary>
    public sealed class AwaitableLock : IAwaitableLock
    {
        private SemaphoreSlim _semaphore = new(1, 1);

        /// <inheritdoc />
        public Task EnterLockAsync(CancellationToken cancellationToken)
        {
            return _semaphore.WaitAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> TryEnterLockAsync(int milliseconds, CancellationToken cancellationToken)
        {
            return _semaphore.WaitAsync(milliseconds, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> TryEnterLockAsync(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            return _semaphore.WaitAsync(timeSpan, cancellationToken);
        }

        /// <inheritdoc />
        public void ExitLock()
        {
            try
            {
                _semaphore.Release();
            }
            catch (SemaphoreFullException exception)
            {
                throw new SynchronizationLockException("The lock is not currently held.", exception);
            }
        }

        /// <summary>Disposes of the managed resources.</summary>
        public void Dispose()
        {
            _semaphore?.Dispose();
            _semaphore = null;
        }
    }
}