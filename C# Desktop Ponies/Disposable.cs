﻿namespace CsDesktopPonies
{
    using System;
    using System.Threading;

    /// <summary>
    /// A class capable of releasing allocated resources.
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        /// <summary>
        /// Value that indicates disposal has begun.
        /// </summary>
        private const int IsDisposing = 1;
        /// <summary>
        /// Indicates whether disposal has begun on this object.
        /// </summary>
        private int disposalState = IsDisposing - 1;
        /// <summary>
        /// Gets a value indicating whether the object is being, or has been, disposed.
        /// </summary>
        public bool Disposed
        {
            get { return disposalState == IsDisposing; }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="CsDesktopPonies.Disposable"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly",
            Justification = "Implementation is correct, but not an exact match which misleads analysis.")]
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposalState, IsDisposing) != IsDisposing)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">Indicates if managed resources should be disposed in addition to unmanaged resources; otherwise, only
        /// unmanaged resources should be disposed.</param>
        protected abstract void Dispose(bool disposing);
    }

    /// <summary>
    /// Provides extensions methods for <see cref="T:System.IDisposable"/> objects.
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        /// Performs additional setup on a newly instantiated <see cref="T:System.IDisposable"/> whilst ensuring the resource is disposed
        /// if an exception occurs.
        /// </summary>
        /// <typeparam name="TDisposable">The type of the resource to setup.</typeparam>
        /// <param name="disposable">A newly instantiated resource to be setup.</param>
        /// <param name="setup">Action to perform on the newly instantiated resource. This is done in a try-catch block that will ensure
        /// the resource is disposed in the event of an exception; before allowing the exception to propagate.</param>
        /// <returns>A reference to the <see cref="T:System.IDisposable"/>.</returns>
        public static TDisposable SetupSafely<TDisposable>(this TDisposable disposable, Action<TDisposable> setup)
            where TDisposable : IDisposable
        {
            Argument.EnsureNotNull(disposable, "disposable");
            Argument.EnsureNotNull(setup, "setup");
            try
            {
                setup(disposable);
                return disposable;
            }
            catch (Exception)
            {
                disposable.Dispose();
                throw;
            }
        }
    }
}
