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
    /// This is a wrapper for all of the external C functions exposed
    /// by the library.
    /// </summary>
    internal static class Interop
    {
        /// <summary>
        /// The name of the main library that exposes the C API.
        /// </summary>
        private const string GPhoto2Lib = "libgphoto2";


        #region gphoto2-context.h

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
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern IntPtr gp_context_new();


        /// <summary>
        /// Increments the reference counter for a context.
        /// </summary>
        /// <param name="Context">The context being referenced</param>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern void gp_context_ref(IntPtr Context);


        /// <summary>
        /// Decrements the reference count for a GPContext. If the count reaches 0,
        /// the context will be destroyed.
        /// </summary>
        /// <param name="Context">The GPContext to unreference</param>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern void gp_context_unref(IntPtr Context);


        /// <summary>
        /// Sets the callback for an idle period notification
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="Func">The idle period callback (this must be a <see cref="GPContextIdleFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern void gp_context_set_idle_func(IntPtr Context, IntPtr Func, IntPtr Data);


        /// <summary>
        /// Sets the callbacks for progress reports
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="StartFunc">The progress start callback (this must be a <see cref="GPContextProgressStartFunc"/>)</param>
        /// <param name="UpdateFunc">The progress updated callback (this must be a <see cref="GPContextProgressUpdateFunc"/>)</param>
        /// <param name="StopFunc">The progress finished callback (this must be a <see cref="GPContextProgressStopFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern void gp_context_set_progress_funcs(IntPtr Context, IntPtr StartFunc, IntPtr UpdateFunc, IntPtr StopFunc, IntPtr Data);


        /// <summary>
        /// Sets the callback for error messages
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="Func">The error report callback (this must be a <see cref="GPContextErrorFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern void gp_context_set_error_func(IntPtr Context, IntPtr Func, IntPtr Data);


        /// <summary>
        /// Sets the callback for status messages
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="Func">The status message callback (this must be a <see cref="GPContextStatusFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern void gp_context_set_status_func(IntPtr Context, IntPtr Func, IntPtr Data);


        /// <summary>
        /// Sets the callback for questions
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="Func">The question callback (this must be a <see cref="GPContextQuestionFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern void gp_context_set_question_func(IntPtr Context, IntPtr Func, IntPtr Data);


        /// <summary>
        /// Sets the callback for cancel notifications
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="Func">The cancel notification callback (this must be a <see cref="GPContextCancelFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern void gp_context_set_cancel_func(IntPtr Context, IntPtr Func, IntPtr Data);


        /// <summary>
        /// Sets the callback for info messages
        /// </summary>
        /// <param name="Context">The GPContext to set the callback for</param>
        /// <param name="Func">The info message callback (this must be a <see cref="GPContextMessageFunc"/>)</param>
        /// <param name="Data">User-specified data that will be included in the callback</param>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern void gp_context_set_message_func(IntPtr Context, IntPtr Func, IntPtr Data);

        #endregion


        #region gphoto2-camera.h

        /// <summary>
        /// Create a new camera device.
        /// </summary>
        /// <param name="Camera">[OUT] A pointer to the new camera object</param>
        /// <returns>A status code indicating whether or not the function succeeded,
        /// and the kind of error if it failed.</returns>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern GPResult gp_camera_new(out IntPtr Camera);


        /// <summary>
        /// Initializes the camera and reconnects to it.
        /// </summary>
        /// <param name="Camera">The camera to initialize and connect to</param>
        /// <param name="Context">The context that owns the camera</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern GPResult gp_camera_init(IntPtr Camera, IntPtr Context);

        #endregion


        #region gphoto2-list.h

        /// <summary>
        /// Creates a new camera list.
        /// </summary>
        /// <param name="CameraList">[OUT] The pointer to the list being created</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern GPResult gp_list_new(out IntPtr CameraList);


        /// <summary>
        /// Frees a camera list.
        /// </summary>
        /// <param name="CameraList">The list to destroy</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern GPResult gp_list_free(IntPtr CameraList);

        #endregion


    }
}
