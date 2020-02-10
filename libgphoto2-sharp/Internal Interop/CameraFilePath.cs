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

using System.Runtime.InteropServices;

namespace GPhoto2.Net
{
    /// <summary>
    /// A structure containing the folder and filename of an object
    /// after a successful capture and is passed as reference to the
    /// gp_camera_capture() function.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct CameraFilePath
    {
        /// <summary>
        /// Name of the captured file.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Name;


        /// <summary>
        /// Name of the folder of the captured file.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string Folder;
    }
}
