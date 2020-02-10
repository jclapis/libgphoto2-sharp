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
        /// <param name="Camera">This <see cref="Camera"/> handle</param>
        /// <param name="Context">The context that owns the camera</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_camera_init(IntPtr Camera, IntPtr Context);


        /// <summary>
        /// Closes a connection to the camera.
        /// </summary>
        /// <param name="Camera">This <see cref="Camera"/> handle</param>
        /// <param name="Context">The context that owns the camera</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_camera_exit(IntPtr Camera, IntPtr Context);


        /// <summary>
        /// Retrieve the configuration window for the camera.
        /// </summary>
        /// <param name="Camera">This <see cref="Camera"/> handle</param>
        /// <param name="Window">[OUT] The <see cref="CameraWidget"/> handle for this camera's configuration</param>
        /// <param name="Context">The context that owns the camera</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_camera_get_config(IntPtr Camera, out IntPtr Window, IntPtr Context);


        /// <summary>
        /// Sets the configuration.
        /// </summary>
        /// <param name="Camera">This <see cref="Camera"/> handle</param>
        /// <param name="Window">The <see cref="CameraWidget"/> handle for the configuration to set</param>
        /// <param name="Context">The context that owns the camera</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_camera_set_config(IntPtr Camera, IntPtr Window, IntPtr Context);


        /// <summary>
        /// Captures an image, movie, or sound clip depending on the given type.
        /// </summary>
        /// <param name="Camera">This <see cref="Camera"/> handle</param>
        /// <param name="Type">The type of media to capture</param>
        /// <param name="Path">A struct describing the location of the captured file</param>
        /// <param name="Context">The context that owns the camera</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_camera_capture(IntPtr Camera, CameraCaptureType Type, out CameraFilePath Path, IntPtr Context);


        /// <summary>
        /// Retrieves a file from the Camera.
        /// </summary>
        /// <param name="Camera">This <see cref="Camera"/> handle</param>
        /// <param name="Folder">The folder that the file resides in</param>
        /// <param name="File">The name of the file to retrieve</param>
        /// <param name="Type">The type of the file</param>
        /// <param name="CameraFile">The CameraFile object that will contain the file's data</param>
        /// <param name="Context">The context that owns the camera</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_camera_file_get(IntPtr Camera, string Folder, string File, CameraFileType Type, IntPtr CameraFile, IntPtr Context);

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
        /// The camera's configuration settings
        /// </summary>
        public CameraConfiguration Configuration { get; }


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

            result = gp_camera_get_config(Handle, out IntPtr widgetHandle, Context.Handle);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Error getting camera config: {result}");
            }
            CameraWidget widget = new CameraWidget(widgetHandle);
            Configuration = new CameraConfiguration(widget);
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


        /// <summary>
        /// Updates the camera's configuration settings. Modify the <see cref="Configuration"/>
        /// object, then call this to pass the updates to the camera.
        /// </summary>
        public void UpdateConfiguration()
        {
            GPResult result = gp_camera_set_config(Handle, Configuration.Widget.Handle, Context.Handle);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Error updating camera configuration: {result}");
            }
        }


        /// <summary>
        /// Captures and retrieves a file from the camera.
        /// </summary>
        /// <param name="CaptureType">The type of file to capture</param>
        /// <returns>The captured file</returns>
        public CameraFile Capture(CameraCaptureType CaptureType)
        {
            if (DisposedValue)
            {
                throw new ObjectDisposedException(nameof(Camera));
            }

            GPResult result = gp_camera_capture(Handle, CaptureType, out CameraFilePath path, Context.Handle);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Error during camera capture: {result}");
            }

            CameraFile cameraFile = new CameraFile(path.Folder, path.Name);
            result = gp_camera_file_get(Handle, path.Folder, path.Name, CameraFileType.Normal, cameraFile.Handle, Context.Handle);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Error getting file {path.Name} from camera: {result}");
            }
            cameraFile.DownloadData();
            cameraFile.Dispose();   // Get rid of the unmanaged camera_file object, so we don't have 2 copies of the file's data
            return cameraFile;
        }


        #region IDisposable Support
        private bool DisposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    Configuration.Dispose();
                }
                gp_camera_free(Handle);
                DisposedValue = true;
            }
        }

        ~Camera()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
