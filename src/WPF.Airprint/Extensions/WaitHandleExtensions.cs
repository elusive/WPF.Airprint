namespace WPF.Airprint.Extensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class WaitHandleExtensions
    {
        /// <summary>
        /// Allows use of wait handle to create async/await task method.
        /// </summary>
        /// <remarks>
        /// Timeout results in false return value.
        /// </remarks>
        /// <param name="waitHandle"></param>
        /// <param name="timeoutMs"></param>
        /// <returns>Boolean success indicator wrapped in a task.</returns>
        public static Task<bool> WaitOneAsync(this WaitHandle waitHandle, int timeoutMs)
        {
            if (waitHandle == null)
                throw new ArgumentNullException(nameof(waitHandle));

            var tcs = new TaskCompletionSource<bool>();

            RegisteredWaitHandle registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                waitHandle,
                callBack: (state, timedOut) => { tcs.TrySetResult(!timedOut); },
                state: null,
                millisecondsTimeOutInterval: timeoutMs,
                executeOnlyOnce: true);

            return tcs.Task.ContinueWith((antecedent) =>
            {
                registeredWaitHandle.Unregister(waitObject: null);
                try
                {
                    return antecedent.Result;
                }
                catch
                {
                    return false;
                    throw;
                }
            });
        }
    }
}
