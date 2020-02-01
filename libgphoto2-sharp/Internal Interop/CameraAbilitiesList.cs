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
        #region Interop from gphoto2-abilities-list.h

        /// <summary>
        /// Creates a new abilities list.
        /// </summary>
        /// <param name="AbilitiesList">[OUT] The pointer to the list being created</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_abilities_list_new(out IntPtr AbilitiesList);


        /// <summary>
        /// Frees an abilities list.
        /// </summary>
        /// <param name="AbilitiesList">The list to destroy</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_abilities_list_free(IntPtr AbilitiesList);


        /// <summary>
        /// Scans the system for camera drivers.
        /// </summary>
        /// <param name="List">The abilities list that will receive the loaded drivers</param>
        /// <param name="Context">The <see cref="Context"/> that owns this list</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_abilities_list_load(IntPtr List, IntPtr Context);


        /// <summary>
        /// Tries to detect any camera connected to the computer using the supplied
        /// list of supported cameras and the supplied info_list of ports.
        /// </summary>
        /// <param name="AbilitiesList">A handle to this <see cref="CameraAbilitiesList"/></param>
        /// <param name="PortInfoList">A <see cref="PortInfoList"/> handle that contains the list of ports to scan</param>
        /// <param name="CameraList">A <see cref="CameraList"/> that will hold the detected cameras</param>
        /// <param name="Context">The <see cref="Context"/> that owns this list</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_abilities_list_detect(IntPtr AbilitiesList, IntPtr PortInfoList, IntPtr CameraList, IntPtr Context);


        /// <summary>
        /// Gets the index of the driver that supports the provided camera model.
        /// </summary>
        /// <param name="List">A handle to this <see cref="CameraAbilitiesList"/></param>
        /// <param name="Model">The name of the camera model to get the driver for</param>
        /// <returns>The index of the matching driver, or a <see cref="GPResult"/> error code if something went wrong.</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int gp_abilities_list_lookup_model(IntPtr List, string Model);


        /// <summary>
        /// Retrieve the camera abilities of entry with supplied index number.
        /// </summary>
        /// <param name="List">A handle to this <see cref="CameraAbilitiesList"/></param>
        /// <param name="Index">The index of the camera to get the abilities for</param>
        /// <param name="Abilities">[OUT] The camera's abilities</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_abilities_list_get_abilities(IntPtr List, int Index, out CameraAbilities Abilities);

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
        /// Creates a new <see cref="CameraAbilitiesList"/> instance.
        /// </summary>
        /// <param name="Context">The context that owns this list</param>
        public CameraAbilitiesList(Context Context)
        {
            this.Context = Context;

            GPResult result = gp_abilities_list_new(out IntPtr handle);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Creating a new {nameof(CameraAbilitiesList)} failed: {result}");
            }
            Handle = handle;
        }


        /// <summary>
        /// Loads the camera drivers that are installed on the system
        /// </summary>
        public void LoadAvailableDrivers()
        {
            if(DisposedValue)
            {
                throw new ObjectDisposedException(nameof(CameraAbilitiesList));
            }

            GPResult result = gp_abilities_list_load(Handle, Context.Handle);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Loading the list of installed drivers failed: {result}");
            }
        }


        /// <summary>
        /// Scans the computer to find all of the devices that are supported by the installed drivers.
        /// </summary>
        /// <param name="PortsToScan">The ports to scan for connected devices</param>
        /// <returns>A list of connected cameras (or rather, [model, port] string pairs)</returns>
        public CameraList FindAllConnectedCameras(PortInfoList PortsToScan)
        {
            CameraList discoveredCameras = new CameraList(Context);

            GPResult result = gp_abilities_list_detect(Handle, PortsToScan.Handle, discoveredCameras.Handle, Context.Handle);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Error detecting available cameras: {result}");
            }

            return discoveredCameras;
        }


        /// <summary>
        /// Finds the list of capabilities that the driver for the provided camera supports.
        /// </summary>
        /// <param name="CameraName">The name of the camera to get the abiliteis for</param>
        /// <returns>The abilities that the camera supports</returns>
        public CameraAbilities FindAbilitiesForCamera(string CameraName)
        {
            GPResult result;

            // Get the index of the driver that supports this camera
            int driverIndex = gp_abilities_list_lookup_model(Handle, CameraName);
            if (driverIndex < (int)GPResult.Ok)
            {
                result = (GPResult)driverIndex;
                throw new Exception($"Failed to load driver for camera {CameraName}: {result}");
            }

            // Get the list of abilities that the driver supports
            result = gp_abilities_list_get_abilities(Handle, driverIndex, out CameraAbilities cameraAbilities);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Failed to get abilities for camera {CameraName}: {result}");
            }

            return cameraAbilities;
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

                gp_abilities_list_free(Handle);
                DisposedValue = true;
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
