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
        #region Interop from gphoto2-context.h

        public delegate void GPContextIdleFunc(IntPtr Context, IntPtr Data);
        public delegate void GPContextErrorFunc(IntPtr Context, string Text, IntPtr Data);
        public delegate void GPContextStatusFunc(IntPtr Context, string Text, IntPtr Data);
        public delegate void GPContextMessageFunc(IntPtr Context, string Text, IntPtr Data);
        public delegate GPContextFeedback GPContextQuestionFunc(IntPtr Context, string Text, IntPtr Data);
        public delegate GPContextFeedback GPContextCancelFunc(IntPtr Context, IntPtr Data);
        public delegate uint GPContextProgressStartFunc(IntPtr Context, float Target, string Text, IntPtr Data);
        public delegate void GPContextProgressUpdateFunc(IntPtr Context, uint ID, float Current, IntPtr Data);
        public delegate void GPContextProgressStopFunc(IntPtr Context, uint ID, IntPtr Data);


        /// <summary>
        /// Created a new GPContext.
        /// </summary>
        /// <returns>A pointer to the new GPContext</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern IntPtr gp_context_new();


        /// <summary>
        /// Increments the reference counter for a context.
        /// </summary>
        /// <param name="Context">The context being referenced</param>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern void gp_context_ref(IntPtr Context);


        /// <summary>
        /// Decrements the reference count for a GPContext. If the count reaches 0,
        /// the context will be destroyed.
        /// </summary>
        /// <param name="Context">The GPContext to unreference</param>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern void gp_context_unref(IntPtr Context);


        /// <summary>
        /// Sets the callback for an idle period notification
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="Func">The idle period callback (this must be a <see cref="GPContextIdleFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern void gp_context_set_idle_func(IntPtr Context, IntPtr Func, IntPtr Data);


        /// <summary>
        /// Sets the callbacks for progress reports
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="StartFunc">The progress start callback (this must be a <see cref="GPContextProgressStartFunc"/>)</param>
        /// <param name="UpdateFunc">The progress updated callback (this must be a <see cref="GPContextProgressUpdateFunc"/>)</param>
        /// <param name="StopFunc">The progress finished callback (this must be a <see cref="GPContextProgressStopFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern void gp_context_set_progress_funcs(IntPtr Context, IntPtr StartFunc, IntPtr UpdateFunc, IntPtr StopFunc, IntPtr Data);


        /// <summary>
        /// Sets the callback for error messages
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="Func">The error report callback (this must be a <see cref="GPContextErrorFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern void gp_context_set_error_func(IntPtr Context, IntPtr Func, IntPtr Data);


        /// <summary>
        /// Sets the callback for status messages
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="Func">The status message callback (this must be a <see cref="GPContextStatusFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern void gp_context_set_status_func(IntPtr Context, IntPtr Func, IntPtr Data);


        /// <summary>
        /// Sets the callback for questions
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="Func">The question callback (this must be a <see cref="GPContextQuestionFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern void gp_context_set_question_func(IntPtr Context, IntPtr Func, IntPtr Data);


        /// <summary>
        /// Sets the callback for cancel notifications
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="Func">The cancel notification callback (this must be a <see cref="GPContextCancelFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern void gp_context_set_cancel_func(IntPtr Context, IntPtr Func, IntPtr Data);


        /// <summary>
        /// Sets the callback for info messages
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="Func">The info message callback (this must be a <see cref="GPContextMessageFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern void gp_context_set_message_func(IntPtr Context, IntPtr Func, IntPtr Data);

        #endregion

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
        private readonly GPContextIdleFunc IdleCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="ErrorCallback(IntPtr, string, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly GPContextErrorFunc ErrorCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="ProgressStartCallback(IntPtr, float, string, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly GPContextProgressStartFunc ProgressStartCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="ProgressUpdateCallback(IntPtr, uint, float, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly GPContextProgressUpdateFunc ProgressUpdateCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="ProgressStopCallback(IntPtr, uint, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly GPContextProgressStopFunc ProgressStopCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="StatusCallback(IntPtr, string, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly GPContextStatusFunc StatusCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="QuestionCallback(IntPtr, string, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly GPContextQuestionFunc QuestionCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="CancelCallback(IntPtr, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly GPContextCancelFunc CancelCallbackDelegate;


        /// <summary>
        /// A handle to the <see cref="MessageCallback(IntPtr, string, IntPtr)"/> function,
        /// which will be registered with the underlying GPContext object
        /// </summary>
        private readonly GPContextMessageFunc MessageCallbackDelegate;


        /// <summary>
        /// The user's response to a question
        /// </summary>
        private GPContextFeedback QuestionResponse;


        /// <summary>
        /// The user's response to a cancel query
        /// </summary>
        private GPContextFeedback CancelResponse;


        /// <summary>
        /// The collection of loaded camera drivers
        /// </summary>
        private CameraAbilitiesList DriverList;


        /// <summary>
        /// The collection of available I/O ports
        /// </summary>
        private PortInfoList PortList;


        /// <summary>
        /// A handle to the native GPContext object
        /// </summary>
        internal IntPtr Handle { get; }


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
            QuestionWaiter = new AutoResetEvent(false);
            CancelWaiter = new AutoResetEvent(false);

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
            Handle = gp_context_new();
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
            gp_context_set_idle_func(Handle, IdleCallbackPtr, IntPtr.Zero);
            gp_context_set_error_func(Handle, ErrorCallbackPtr, IntPtr.Zero);
            gp_context_set_progress_funcs(Handle, ProgressStartCallbackPtr, 
                ProgressUpdateCallbackPtr, ProgressStopCallbackPtr, IntPtr.Zero);
            gp_context_set_status_func(Handle, StatusCallbackPtr, IntPtr.Zero);
            gp_context_set_question_func(Handle, QuestionCallbackPtr, IntPtr.Zero);
            gp_context_set_cancel_func(Handle, CancelCallbackPtr, IntPtr.Zero);
            gp_context_set_message_func(Handle, MessageCallbackPtr, IntPtr.Zero);

            // Load all of the installed camera drivers
            DriverList = new CameraAbilitiesList(this);
            DriverList.LoadAvailableDrivers();

            // Load all of the available I/O ports
            PortList = new PortInfoList(this);
            PortList.LoadAvailablePorts();
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

        private bool DisposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    DriverList.Dispose();
                    PortList.Dispose();
                }

                gp_context_unref(Handle);
                DisposedValue = true;
            }
        }

        ~Context()
        {
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
