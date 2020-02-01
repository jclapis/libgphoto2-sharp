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
    /// This represents a list of camera drivers that are available on the system.
    /// </summary>
    internal class CameraAbilitiesList : IDisposable
    {
        #region gphoto2-abilities-list.h

        /// <summary>
        /// Creates a new abilities list.
        /// </summary>
        /// <param name="AbilitiesList">[OUT] The pointer to the list being created</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern GPResult gp_abilities_list_new(out IntPtr AbilitiesList);


        /// <summary>
        /// Frees an abilities list.
        /// </summary>
        /// <param name="AbilitiesList">The list to destroy</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern GPResult gp_abilities_list_free(IntPtr AbilitiesList);


        /// <summary>
        /// Scans the system for camera drivers.
        /// </summary>
        /// <param name="List">The abilities list that will receive the loaded drivers</param>
        /// <param name="Context">The current GPContext</param>
        /// <returns></returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern GPResult gp_abilities_list_load(IntPtr List, IntPtr Context);

        #endregion


        /// <summary>
        /// The underlying handle used by libgphoto2
        /// </summary>
        private IntPtr Handle;


        /// <summary>
        /// The context that owns this list
        /// </summary>
        private readonly Context Context;


        /// <summary>
        /// Creates a new CameraAbilitiesList instance.
        /// </summary>
        /// <param name="Context">The context that owns this list</param>
        public CameraAbilitiesList(Context Context)
        {
            this.Context = Context;
            GPResult result = gp_abilities_list_new(out Handle);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Creating a new {nameof(CameraAbilitiesList)} failed: {result}");
            }
        }


        /// <summary>
        /// Loads the camera drivers that are installed on the system
        /// </summary>
        public void LoadAvailableDrivers()
        {
            GPResult result = gp_abilities_list_load(Handle, Context.Handle);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Loading the list of installed drivers failed: {result}");
            }
        }


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
                    gp_abilities_list_free(Handle);
                    Handle = IntPtr.Zero;
                }
                disposedValue = true;
            }
        }

        ~CameraAbilitiesList()
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
