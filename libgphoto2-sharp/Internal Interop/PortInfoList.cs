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
    /// <summary>
    /// This represents a collection of information containers around the ports that
    /// are currently available on the system.
    /// </summary>
    internal class PortInfoList : IDisposable
    {
        #region Interop from gphoto2-port-info-list.h

        /// <summary>
        /// Creates a new list of port infos.
        /// </summary>
        /// <param name="List">[OUT] The handle to the new list of port infos</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_port_info_list_new(out IntPtr List);


        /// <summary>
        /// Frees a list of port infos.
        /// </summary>
        /// <param name="List">This <see cref="PortInfoList"/> handle</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_port_info_list_free(IntPtr List);


        /// <summary>
        /// Searches the system for io-drivers and appends them to the list.
        /// </summary>
        /// <param name="List">This <see cref="PortInfoList"/> handle</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_port_info_list_load(IntPtr List);


        /// <summary>
        /// Looks for an entry in the list with the supplied path.
        /// </summary>
        /// <param name="List">This <see cref="PortInfoList"/> handle</param>
        /// <param name="Path">The path to look for</param>
        /// <returns>The index of the entry or a gphoto2 error code</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int gp_port_info_list_lookup_path(IntPtr List, string Path);


        /// <summary>
        /// Get port information of specific entry
        /// </summary>
        /// <param name="List">This <see cref="PortInfoList"/> handle</param>
        /// <param name="Index"The index of the entry></param>
        /// <param name="Info">[OUT] >A pointer to the current entry</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_port_info_list_get_info(IntPtr List, int Index, out IntPtr Info);

        #endregion


        /// <summary>
        /// The context that owns this list
        /// </summary>
        private readonly Context Context;


        /// <summary>
        /// The underlying handle used by libgphoto2
        /// </summary>
        public IntPtr Handle { get; }


        /// <summary>
        /// Creates a new <see cref="PortInfoList"/> instance.
        /// </summary>
        /// <param name="Context">The context that owns this list</param>
        public PortInfoList(Context Context)
        {
            this.Context = Context;

            GPResult result = gp_port_info_list_new(out IntPtr handle);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Failed to create new {nameof(PortInfoList)}: {result}");
            }
            Handle = handle;
        }


        /// <summary>
        /// Loads the available I/O ports on the system.
        /// </summary>
        public void LoadAvailablePorts()
        {
            if(DisposedValue)
            {
                throw new ObjectDisposedException(nameof(PortInfoList));
            }

            GPResult result = gp_port_info_list_load(Handle);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Loading available ports failed: {result}");
            }
        }

        
        /// <summary>
        /// Gets the index of the port info 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public GPPortInfo FindInfoForPath(string Path)
        {
            int index = gp_port_info_list_lookup_path(Handle, Path);
            if(index < (int)GPResult.Ok)
            {
                throw new Exception($"Error finding port info for path {Path}: {(GPResult)index}");
            }

            GPResult result = gp_port_info_list_get_info(Handle, index, out IntPtr infoHandle);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Error getting the port info for index {index}: {result}");
            }

            GPPortInfo info = new GPPortInfo(infoHandle);
            return info;
        }


        #region IDisposable Support
        private bool DisposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                gp_port_info_list_free(Handle);
                DisposedValue = true;
            }
        }

        ~PortInfoList()
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
