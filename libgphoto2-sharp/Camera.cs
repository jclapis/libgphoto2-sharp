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
    /// A Camera object represents a specific instance of a (physical of
    /// virtual) camera attached to the system.
    /// 
    /// The abilities of this type of camera are stored in a CameraAbility
    /// object.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// A handle to the native Camera object
        /// </summary>
        private IntPtr GPCameraHandle;


        /// <summary>
        /// The Context that owns this camera
        /// </summary>
        public Context Context { get; }


        /// <summary>
        /// Creates a new Camera instance.
        /// </summary>
        /// <param name="Context">The Context that owns this camera</param>
        internal Camera(Context Context)
        {
            this.Context = Context;
            GPResult result = Interop.gp_camera_new(out GPCameraHandle);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Error creating camera: {result}");
            }

            // TODO
            //result = 
        }

    }
}
