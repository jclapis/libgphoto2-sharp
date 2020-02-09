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
using System.Runtime.InteropServices;

namespace GPhoto2.Net
{
    internal class GPPortInfo
    {
        #region Interop from gphoto2-port-info-list.h

        /// <summary>
        /// Retreives the name of the passed in GPPortInfo, by reference.
        /// </summary>
        /// <param name="Info">This <see cref="GPPortInfo"/> handle</param>
        /// <param name="Name">[OUT] The name of the port</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_port_info_get_name(IntPtr Info, out IntPtr Name);


        /// <summary>
        /// Retreives the path of the passed in GPPortInfo, by reference.
        /// </summary>
        /// <param name="Info">This <see cref="GPPortInfo"/> handle</param>
        /// <param name="Name">[OUT] The path of the port</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_port_info_get_path(IntPtr Info, out IntPtr Path);


        /// <summary>
        /// Retreives the type of the passed in GPPortInfo.
        /// </summary>
        /// <param name="Info">This <see cref="GPPortInfo"/> handle</param>
        /// <param name="Name">[OUT] The type of the port</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_port_info_get_type(IntPtr Info, out GPPortType Path);

        #endregion


        /// <summary>
        /// The underlying handle used by libgphoto2
        /// </summary>
        public IntPtr Handle { get; }


        /// <summary>
        /// The name of the port
        /// </summary>
        public string Name
        {
            get
            {
                GPResult result = gp_port_info_get_name(Handle, out IntPtr namePtr);
                if(result != GPResult.Ok)
                {
                    throw new Exception($"Getting the port name failed: {result}");
                }

                string name = Marshal.PtrToStringAnsi(namePtr);
                return name;
            }
        }


        /// <summary>
        /// The path of the port
        /// </summary>
        public string Path
        {
            get
            {
                GPResult result = gp_port_info_get_path(Handle, out IntPtr pathPtr);
                if (result != GPResult.Ok)
                {
                    throw new Exception($"Getting the port path failed: {result}");
                }

                string path = Marshal.PtrToStringAnsi(pathPtr);
                return path;
            }
        }


        /// <summary>
        /// The type of the port
        /// </summary>
        public GPPortType Type
        {
            get
            {
                GPResult result = gp_port_info_get_type(Handle, out GPPortType type);
                if (result != GPResult.Ok)
                {
                    throw new Exception($"Getting the port type failed: {result}");
                }

                return type;
            }
        }


        /// <summary>
        /// Creates a new <see cref="GPPortInfo"/> instance.
        /// </summary>
        /// <param name="Context">The underlying handle used by libgphoto2</param>
        public GPPortInfo(IntPtr Handle)
        {
            this.Handle = Handle;
        }

    }
}
