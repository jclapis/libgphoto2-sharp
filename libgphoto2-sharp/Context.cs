/* ========================================================================
 * Copyright (C) 2020 Joe Clapis.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ======================================================================== */
 
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GPhoto2.Net
{
    /// <summary>
    /// This class allows callback handling, passing error contexts back, 
    /// progress handling and download cancellation and similar things.
    /// It is usually passed around the functions.
    /// </summary>
    public class Context : IDisposable
    {
        #region Properties, Fields, and Events

        /// <summary>
        /// A counter for the next available ID for a progress counter
        /// </summary>
        private uint NextProgressID;


        /// <summary>
        /// A sync lock to safely handle progress starts, updates, and stops
        /// </summary>
        private readonly object ProgressLock;


        /// <summary>
        /// A map of progress trackers to the target values for when they reach completion
        /// </summary>
        private readonly Dictionary<uint, float> ProgressTargets;


        /// <summary>
        /// A handle to the <see cref="IdleCallback(IntPtr, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly Interop.GPContextIdleFunc IdleCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="ErrorCallback(IntPtr, string, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly Interop.GPContextErrorFunc ErrorCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="ProgressStartCallback(IntPtr, float, string, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly Interop.GPContextProgressStartFunc ProgressStartCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="ProgressUpdateCallback(IntPtr, uint, float, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly Interop.GPContextProgressUpdateFunc ProgressUpdateCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="ProgressStopCallback(IntPtr, uint, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly Interop.GPContextProgressStopFunc ProgressStopCallbackDelegate;


        /// <summary>
        /// A handle to the native GPContext object
        /// </summary>
        internal IntPtr GPContextHandle { get; private set; }


        /// <summary>
        /// This occurs when the context reports that it is idle, so user processing
        /// (such as frontend updates) can proceed.
        /// </summary>
        public event EventHandler IdleNotification;


        /// <summary>
        /// This is triggered when an error occurs. The argument is the error message.
        /// </summary>
        public event EventHandler<string> ErrorOccurred;


        /// <summary>
        /// This is triggered when the context starts a new operation that supports
        /// progress tracking.
        /// </summary>
        public event EventHandler<ProgressStartArgs> ProgressStarted;


        /// <summary>
        /// This is triggered when the progress of an operation has been updated.
        /// </summary>
        public event EventHandler<ProgressUpdateArgs> ProgressUpdated;


        /// <summary>
        /// This is triggered when an operation that supports progress tracking
        /// has completed or is cancelled. The argument is the ID of the operation.
        /// </summary>
        public event EventHandler<uint> ProgressStopped;

        #endregion

        /// <summary>
        /// Creates a new Context instance.
        /// </summary>
        public Context()
        {
            // Initialize progress tracker system
            NextProgressID = 1;
            ProgressLock = new object();
            ProgressTargets = new Dictionary<uint, float>();

            // Set up handles to callback delegates so they don't get garbage collected
            IdleCallbackDelegate = IdleCallback;
            ErrorCallbackDelegate = ErrorCallback;
            ProgressStartCallbackDelegate = ProgressStartCallback;
            ProgressUpdateCallbackDelegate = ProgressUpdateCallback;
            ProgressStopCallbackDelegate = ProgressStopCallback;

            // Create the underlying GPContext
            GPContextHandle = Interop.gp_context_new();
            if(GPContextHandle == IntPtr.Zero)
            {
                throw new Exception("Context creation failed.");
            }

            // Create pinned function pointers for the callbacks
            IntPtr IdleCallbackPtr = Marshal.GetFunctionPointerForDelegate(IdleCallbackDelegate);
            IntPtr ErrorCallbackPtr = Marshal.GetFunctionPointerForDelegate(ErrorCallbackDelegate);
            IntPtr ProgressStartCallbackPtr = Marshal.GetFunctionPointerForDelegate(ProgressStartCallbackDelegate);
            IntPtr ProgressUpdateCallbackPtr = Marshal.GetFunctionPointerForDelegate(ProgressUpdateCallbackDelegate);
            IntPtr ProgressStopCallbackPtr = Marshal.GetFunctionPointerForDelegate(ProgressStopCallbackDelegate);

            // Register the callbacks with the GPContext
            Interop.gp_context_set_idle_func(GPContextHandle, IdleCallbackPtr, IntPtr.Zero);
            Interop.gp_context_set_error_func(GPContextHandle, ErrorCallbackPtr, IntPtr.Zero);
            Interop.gp_context_set_progress_funcs(GPContextHandle, ProgressStartCallbackPtr, 
                ProgressUpdateCallbackPtr, ProgressStopCallbackPtr, IntPtr.Zero);
        }


        /// <summary>
        /// Retrieves a collection of the cameras that are connected to
        /// this machine.
        /// </summary>
        /// <returns>A collection of available cameras</returns>
        public IEnumerable<Camera> GetCameras()
        {
            // TODO
            return null;
        }


        #region Callbacks

        /// <summary>
        /// Triggers the idle event when the underlying GPContext calls this callback.
        /// </summary>
        /// <param name="Context">Not used</param>
        /// <param name="Data">Not used</param>
        private void IdleCallback(IntPtr Context, IntPtr Data)
        {
            IdleNotification?.Invoke(this, null);
        }


        /// <summary>
        /// Triggers the progress started event when the underlying GPContext calls this callback.
        /// </summary>
        /// <param name="Context">Not used</param>
        /// <param name="Target">The target that represents completion for this operation</param>
        /// <param name="Text">A message describing the operation</param>
        /// <param name="Data">Not used</param>
        /// <returns>The unique identifier for this operation</returns>
        private uint ProgressStartCallback(IntPtr Context, float Target, string Text, IntPtr Data)
        {
            lock(ProgressLock)
            {
                uint eventID = NextProgressID;
                NextProgressID++;

                ProgressTargets.Add(eventID, Target);
                ProgressStarted?.Invoke(this, new ProgressStartArgs(eventID, Text));
                return eventID;
            }
        }


        /// <summary>
        /// Triggers the progress updated event when the underlying GPContext calls this callback.
        /// </summary>
        /// <param name="Context">Not used</param>
        /// <param name="ID">The unique identifier for the updated operation</param>
        /// <param name="Current">The current progress of the operation</param>
        /// <param name="Data">Not used</param>
        private void ProgressUpdateCallback(IntPtr Context, uint ID, float Current, IntPtr Data)
        {
            lock(ProgressLock)
            {
                if(!ProgressTargets.TryGetValue(ID, out float target))
                {
                    // Ignore it, or log it or something.
                }
                float currentProgress = Current / target;
                ProgressUpdated?.Invoke(this, new ProgressUpdateArgs(ID, currentProgress));
            }
        }


        /// <summary>
        /// Triggers the progress stopped event when the underlying GPContext calls this callback.
        /// </summary>
        /// <param name="Context">Not used</param>
        /// <param name="ID">The unique identifier for the stopped operation</param>
        /// <param name="Data">Not used</param>
        private void ProgressStopCallback(IntPtr Context, uint ID, IntPtr Data)
        {
            lock(ProgressLock)
            {
                ProgressTargets.Remove(ID);
                ProgressStopped?.Invoke(this, ID);
            }
        }


        /// <summary>
        /// Triggers the error event when the underlying GPContext calls this callback.
        /// </summary>
        /// <param name="Context">Not used</param>
        /// <param name="ErrorMessage">The error message sent by the GPContext</param>
        /// <param name="Data">Not used</param>
        private void ErrorCallback(IntPtr Context, string ErrorMessage, IntPtr Data)
        {
            ErrorOccurred?.Invoke(this, ErrorMessage);
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                if(GPContextHandle != IntPtr.Zero)
                {
                    Interop.gp_context_unref(GPContextHandle);
                    GPContextHandle = IntPtr.Zero;
                }

                disposedValue = true;
            }
        }

        ~Context()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
