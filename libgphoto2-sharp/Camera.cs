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
    public class Camera : IDisposable
    {
        #region Interop from gphoto2-camera.h

        /// <summary>
        /// Creates a new camera.
        /// </summary>
        /// <param name="Camera">[OUT] The handle to the new camera</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_camera_new(out IntPtr Camera);


        /// <summary>
        /// Frees a camera.
        /// </summary>
        /// <param name="Camera">This <see cref="Camera"/> handle</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_camera_free(IntPtr Camera);


        /// <summary>
        /// Sets the camera's supported abilities.
        /// </summary>
        /// <param name="Camera">This <see cref="Camera"/> handle</param>
        /// <param name="Abilities">The list of abilities that the camera supports</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_camera_set_abilities(IntPtr Camera, CameraAbilities Abilities);


        /// <summary>
        /// Sets the camera's connected port info.
        /// </summary>
        /// <param name="Camera">This <see cref="Camera"/> handle</param>
        /// <param name="PortInfo">The info about the port this camera is connected to</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_camera_set_port_info(IntPtr Camera, IntPtr PortInfo);


        /// <summary>
        /// Opens a connection to the camera.
        /// </summary>
        /// <param name="Camera">The camera to connect to</param>
        /// <param name="Context">The context that owns the camera</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_camera_init(IntPtr Camera, IntPtr Context);


        /// <summary>
        /// Closes a connection to the camera.
        /// </summary>
        /// <param name="Camera">The camera to close</param>
        /// <param name="Context">The context that owns the camera</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_camera_exit(IntPtr Camera, IntPtr Context);

        #endregion


        /// <summary>
        /// The context that owns this camera
        /// </summary>
        private readonly Context Context;


        /// <summary>
        /// The underlying handle used by libgphoto2
        /// </summary>
        internal IntPtr Handle { get; }


        /// <summary>
        /// The name of the camera model
        /// </summary>
        public string ModelName { get; }


        /// <summary>
        /// The quality of the camera driver
        /// </summary>
        public CameraDriverQuality DriverQuality { get; }


        /// <summary>
        /// The type of connection the camera uses
        /// </summary>
        public GPPortType ConnectionType { get; }


        /// <summary>
        /// The supported serial baud rates for this camera, if it uses a serial connection.
        /// </summary>
        public IReadOnlyList<int> SupportedSerialSpeeds { get; }


        /// <summary>
        /// The media capturing operations that the camera's driver supports
        /// </summary>
        public CameraOperation SupportedCaptureFunctions { get; }


        /// <summary>
        /// The operations on files that the camera's driver supports
        /// </summary>
        public CameraFileOperation SupportedFileFunctions { get; }


        /// <summary>
        /// The operations on folders that the camera's driver supports
        /// </summary>
        public CameraFolderOperation SupportedFolderFunctions { get; }


        /// <summary>
        /// Additional information about the device if it's connected via USB
        /// </summary>
        public USBInfo USBInfo { get; }


        /// <summary>
        /// Creates a new <see cref="Camera"/> instance.
        /// </summary>
        /// <param name="Context">The context that owns this camera</param>
        /// <param name="Abilities">The camera's abilities</param>
        /// <param name="PortInfo">The camera's port info</param>
        internal Camera(Context Context, CameraAbilities Abilities, GPPortInfo PortInfo)
        {
            this.Context = Context;

            GPResult result = gp_camera_new(out IntPtr handle);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Failed to create new {nameof(Camera)}: {result}");
            }
            Handle = handle;

            result = gp_camera_set_abilities(handle, Abilities);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Failed to set camera abilities: {result}");
            }

            result = gp_camera_set_port_info(handle, PortInfo.Handle);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Failed to set camera port info: {result}");
            }

            ModelName = Abilities.Model;
            DriverQuality = Abilities.Status;
            ConnectionType = Abilities.Port;
            SupportedCaptureFunctions = Abilities.Operations;
            SupportedFileFunctions = Abilities.FileOperations;
            SupportedFolderFunctions = Abilities.FolderOperations;

            List<int> serialSpeeds = new List<int>();
            for(int i = 0; i < 64; i++)
            {
                int speed = Abilities.Speed[i];
                if(speed == 0)
                {
                    break;
                }
                else
                {
                    serialSpeeds.Add(speed);
                }
            }
            SupportedSerialSpeeds = serialSpeeds;

            USBInfo = new USBInfo(
                Abilities.UsbVendor,
                Abilities.UsbProduct,
                Abilities.UsbClass,
                Abilities.UsbSubclass,
                Abilities.UsbProtocol);
        }


        /// <summary>
        /// Connects to the camera, enabing remote control and capturing.
        /// </summary>
        public void Connect()
        {
            if(DisposedValue)
            {
                throw new ObjectDisposedException(nameof(Camera));
            }

            GPResult result = gp_camera_init(Handle, Context.Handle);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Error connecting to camera: {result}");
            }
        }


        /// <summary>
        /// Disconnects from the camera.
        /// </summary>
        public void Disconnect()
        {
            if (DisposedValue)
            {
                throw new ObjectDisposedException(nameof(Camera));
            }

            GPResult result = gp_camera_exit(Handle, Context.Handle);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Error disconnecting from camera: {result}");
            }
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

                gp_camera_free(Handle);
                DisposedValue = true;
            }
        }

        ~Camera()
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
