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
using System.Threading;

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
        /// A thread-safe waiter that will block until the user responds to a question
        /// (probably on a different thread, such as the UI thread)
        /// </summary>
        private readonly AutoResetEvent QuestionWaiter;


        /// <summary>
        /// A thread-safe waiter that will block until the user responds to a cancel
        /// request (probably on a different thread, such as the UI thread)
        /// </summary>
        private readonly AutoResetEvent CancelWaiter;


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
        /// A handle to the <see cref="StatusCallback(IntPtr, string, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly Interop.GPContextStatusFunc StatusCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="QuestionCallback(IntPtr, string, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly Interop.GPContextQuestionFunc QuestionCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="CancelCallback(IntPtr, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly Interop.GPContextCancelFunc CancelCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="MessageCallback(IntPtr, string, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly Interop.GPContextMessageFunc MessageCallbackDelegate;


        /// <summary>
        /// The user's response to a question
        /// </summary>
        private GPContextFeedback QuestionResponse;


        /// <summary>
        /// The user's response to a cancel query
        /// </summary>
        private GPContextFeedback CancelResponse;


        /// <summary>
        /// A pointer to the collection of loaded camera drivers.
        /// </summary>
        private readonly CameraAbilitiesList DriverList;


        /// <summary>
        /// A handle to the native GPContext object
        /// </summary>
        internal IntPtr Handle { get; private set; }


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


        /// <summary>
        /// This is trigered when the context updates its status.
        /// </summary>
        public event EventHandler<string> StatusNotification;


        /// <summary>
        /// This is triggered when the context asks a question to the user.
        /// </summary>
        public event EventHandler<string> QuestionAsked;


        /// <summary>
        /// This is triggered when the context asks the user if they want to
        /// cancel the current operation.
        /// </summary>
        public event EventHandler CancelRequested;


        /// <summary>
        /// This is triggered when the context sends a message to the user.
        /// </summary>
        public event EventHandler<string> MessageReceived;

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
            StatusCallbackDelegate = StatusCallback;
            QuestionCallbackDelegate = QuestionCallback;
            CancelCallbackDelegate = CancelCallback;
            MessageCallbackDelegate = MessageCallback;

            // Create the underlying GPContext
            Handle = Interop.gp_context_new();
            if(Handle == IntPtr.Zero)
            {
                throw new Exception("Context creation failed.");
            }

            // Create pinned function pointers for the callbacks
            IntPtr IdleCallbackPtr = Marshal.GetFunctionPointerForDelegate(IdleCallbackDelegate);
            IntPtr ErrorCallbackPtr = Marshal.GetFunctionPointerForDelegate(ErrorCallbackDelegate);
            IntPtr ProgressStartCallbackPtr = Marshal.GetFunctionPointerForDelegate(ProgressStartCallbackDelegate);
            IntPtr ProgressUpdateCallbackPtr = Marshal.GetFunctionPointerForDelegate(ProgressUpdateCallbackDelegate);
            IntPtr ProgressStopCallbackPtr = Marshal.GetFunctionPointerForDelegate(ProgressStopCallbackDelegate);
            IntPtr StatusCallbackPtr = Marshal.GetFunctionPointerForDelegate(StatusCallbackDelegate);
            IntPtr QuestionCallbackPtr = Marshal.GetFunctionPointerForDelegate(QuestionCallbackDelegate);
            IntPtr CancelCallbackPtr = Marshal.GetFunctionPointerForDelegate(CancelCallbackDelegate);
            IntPtr MessageCallbackPtr = Marshal.GetFunctionPointerForDelegate(MessageCallbackDelegate);

            // Register the callbacks with the GPContext
            Interop.gp_context_set_idle_func(Handle, IdleCallbackPtr, IntPtr.Zero);
            Interop.gp_context_set_error_func(Handle, ErrorCallbackPtr, IntPtr.Zero);
            Interop.gp_context_set_progress_funcs(Handle, ProgressStartCallbackPtr, 
                ProgressUpdateCallbackPtr, ProgressStopCallbackPtr, IntPtr.Zero);
            Interop.gp_context_set_status_func(Handle, StatusCallbackPtr, IntPtr.Zero);
            Interop.gp_context_set_question_func(Handle, QuestionCallbackPtr, IntPtr.Zero);
            Interop.gp_context_set_cancel_func(Handle, CancelCallbackPtr, IntPtr.Zero);
            Interop.gp_context_set_message_func(Handle, MessageCallbackPtr, IntPtr.Zero);

            DriverList = new CameraAbilitiesList(this);
            DriverList.LoadAvailableDrivers();
        }


        /// <summary>
        /// Retrieves a collection of the cameras that are connected to
        /// this machine.
        /// </summary>
        /// <returns>A collection of available cameras</returns>
        public IEnumerable<Camera> GetCameras()
        {
            GPResult result = Interop.gp_list_new(out IntPtr cameraList);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Creating a new camera list failed: {result}");
            }




            return null;
        }


        /// <summary>
        /// Sets the user's response to a question. This is a thread-safe call.
        /// </summary>
        /// <param name="Response">The user's response to the question</param>
        public void SetQuestionResponse(GPContextFeedback Response)
        {
            QuestionResponse = Response;
            QuestionWaiter.Set();
        }


        /// <summary>
        /// Sets the user's response to a cancel request. This is a thread-safe call.
        /// </summary>
        /// <param name="Response">The user's response to the cancel request</param>
        public void SetCancelResponse(GPContextFeedback Response)
        {
            CancelResponse = Response;
            CancelWaiter.Set();
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


        /// <summary>
        /// Triggers the status notification event when the underlying GPContext calls this callback.
        /// </summary>
        /// <param name="Context">Not used</param>
        /// <param name="Text">The new status message</param>
        /// <param name="Data">Not used</param>
        private void StatusCallback(IntPtr Context, string Text, IntPtr Data)
        {
            StatusNotification?.Invoke(this, Text);
        }


        /// <summary>
        /// Triggers the question asked event when the underlying GPContext calls this callback.
        /// </summary>
        /// <param name="Context">Not used</param>
        /// <param name="Text">The text of the question to display to the user</param>
        /// <param name="Data">Not used</param>
        /// <returns>A <see cref="GPContextFeedback"/> value indicating the user's response</returns>
        private GPContextFeedback QuestionCallback(IntPtr Context, string Text, IntPtr Data)
        {
            QuestionAsked?.Invoke(this, Text);
            QuestionWaiter.WaitOne();
            return QuestionResponse;
        }


        /// <summary>
        /// Triggers the cancel requested event when the underlying GPContext calls this callback.
        /// </summary>
        /// <param name="Context">Not used</param>
        /// <param name="Data">Not used</param>
        /// <returns>A <see cref="GPContextFeedback"/> value indicating the user's response</returns>
        private GPContextFeedback CancelCallback(IntPtr Context, IntPtr Data)
        {
            CancelRequested?.Invoke(this, null);
            CancelWaiter.WaitOne();
            return CancelResponse;
        }


        /// <summary>
        /// Triggers the message event when the underlying GPContext calls this callback.
        /// </summary>
        /// <param name="Context">Not used</param>
        /// <param name="Text">The text of the message to display to the user</param>
        /// <param name="Data">Not used</param>
        private void MessageCallback(IntPtr Context, string Text, IntPtr Data)
        {
            MessageReceived?.Invoke(this, Text);
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

                if(Handle != IntPtr.Zero)
                {
                    Interop.gp_context_unref(Handle);
                    Handle = IntPtr.Zero;
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
