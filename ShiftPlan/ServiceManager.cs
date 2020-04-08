using System;
using ShiftPlan.Business;
using ShiftPlan.Global;

namespace ShiftPlan
{
    /// <summary>
    /// Provides the functions to interact with the nancy REST service
    /// </summary>
    internal sealed class ServiceManager : IDisposable
    {
        /// <summary>
        /// Contains the value which indicates if the class was already disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Contains the mail manager
        /// </summary>
        private MailManager _manager;

        /// <summary>
        /// Starts the service
        /// </summary>
        public void Start()
        {
            _manager = new MailManager();
            _manager.Run();

            ServiceLogger.Info("Service started.");
        }

        /// <summary>
        /// Stops the service
        /// </summary>
        public void Stop()
        {
            _manager?.Stop();
            ServiceLogger.Info("Service stopped.");
        }

        /// <summary>
        /// Disposes the class
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the class
        /// </summary>
        /// <param name="dispose">true to dispose the class</param>
        private void Dispose(bool dispose)
        {
            if (_disposed)
                return;

            if (dispose)
            {
                _manager?.Dispose();
            }

            _disposed = true;
        }
    }
}
