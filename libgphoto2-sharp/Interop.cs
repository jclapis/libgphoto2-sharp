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


        #region gphoto2-camera.h

        /// <summary>
        /// Create a new camera device.
        /// </summary>
        /// <param name="Camera">[OUT] A pointer to the new camera object</param>
        /// <returns>A status code indicating whether or not the function succeeded,
        /// and the kind of error if it failed.</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern GPResult gp_camera_new(out IntPtr Camera);


        /// <summary>
        /// Initializes the camera and reconnects to it.
        /// </summary>
        /// <param name="Camera">The camera to initialize and connect to</param>
        /// <param name="Context">The context that owns the camera</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern GPResult gp_camera_init(IntPtr Camera, IntPtr Context);

        #endregion


        #region gphoto2-list.h

        /// <summary>
        /// Creates a new camera list.
        /// </summary>
        /// <param name="CameraList">[OUT] The pointer to the list being created</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern GPResult gp_list_new(out IntPtr CameraList);


        /// <summary>
        /// Frees a camera list.
        /// </summary>
        /// <param name="CameraList">The list to destroy</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern GPResult gp_list_free(IntPtr CameraList);

        #endregion


    }
}
