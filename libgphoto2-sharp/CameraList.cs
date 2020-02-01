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
    /// This represents a list of string key-value pairs with information
    /// relating to a set of cameras.
    /// </summary>
    internal class CameraList : IDisposable
    {
        #region Interop from gphoto2-port-info-list.h

        /// <summary>
        /// Creates a new camera list.
        /// </summary>
        /// <param name="List">[OUT] The handle to the new camera list</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_list_new(out IntPtr List);


        /// <summary>
        /// Frees a camera list.
        /// </summary>
        /// <param name="List">The camera list to free</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_list_free(IntPtr List);


        /// <summary>
        /// Gets the number of entries in the camera list.
        /// </summary>
        /// <param name="List">The handle to this <see cref="CameraList"/></param>
        /// <returns>The number of entries in the list</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int gp_list_count(IntPtr List);


        /// <summary>
        /// Gets the name of the camera at the specified index.
        /// </summary>
        /// <param name="List">The handle to this <see cref="CameraList"/></param>
        /// <param name="Index">The index of the camera being queried</param>
        /// <param name="Name">[OUT] The name of the camera</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_list_get_name(IntPtr List, int Index, out string Name);

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
        /// The number of entries in the list
        /// </summary>
        public int Count
        {
            get
            {
                if(DisposedValue)
                {
                    throw new ObjectDisposedException(nameof(CameraList));
                }

                return gp_list_count(Handle);
            }
        }


        /// <summary>
        /// Creates a new <see cref="CameraList"/> instance.
        /// </summary>
        /// <param name="Context">The context that owns this list</param>
        public CameraList(Context Context)
        {
            this.Context = Context;

            GPResult result = gp_list_new(out IntPtr handle);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Failed to create new {nameof(CameraList)}: {result}");
            }
            Handle = handle;
        }


        /// <summary>
        /// Gets the name of the camera at the provided index.
        /// </summary>
        /// <param name="Index">The index of the camera to get the name for</param>
        /// <returns>The name of the camera at the given index</returns>
        public string GetName(int Index)
        {
            if(DisposedValue)
            {
                throw new Exception(nameof(CameraList));
            }

            GPResult result = gp_list_get_name(Handle, Index, out string name);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Error getting name for camera {Index}: {result}");
            }

            return name;
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

                gp_list_free(Handle);
                DisposedValue = true;
            }
        }

        ~CameraList()
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
