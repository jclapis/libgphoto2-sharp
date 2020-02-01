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
    /// Current implementation status of the camera driver.
    /// </summary>
    internal enum CameraDriverStatus
    {
        /// <summary>
        /// Driver is production ready.
        /// </summary>
        Production,

        /// <summary>
        /// Driver is beta quality.
        /// </summary>
        Testing,

        /// <summary>
        /// Driver is alpha quality and might even not work.
        /// </summary>
        Experimental,

        /// <summary>
        /// Driver is no longer recommended to use and will be removed.
        /// </summary>
        Deprecated
    }


    /// <summary>
    /// Type of the device represented.
    /// </summary>
    [Flags]
    internal enum GPhotoDeviceType
    {
        /// <summary>
        /// Traditional still camera
        /// </summary>
        StillCamera,

        /// <summary>
        /// Audio player
        /// </summary>
        AudioPlayer
    }


    /// <summary>
    /// A bitmask of remote control related operations of the device.
    /// Some drivers might support additional dynamic capabilities (like the PTP driver).
    /// </summary>
    [Flags]
    internal enum CameraOperation
    {
        /// <summary>
        /// No remote control operation supported.
        /// </summary>
        None,

        /// <summary>
        /// Capturing images supported.
        /// </summary>
        CaptureImage,

        /// <summary>
        /// Capturing videos supported.
        /// </summary>
        CaptureVideo,

        /// <summary>
        /// Capturing audio supported.
        /// </summary>
        CaptureAudio,

        /// <summary>
        /// Capturing image previews supported.
        /// </summary>
        CapturePreview,

        /// <summary>
        /// Camera and Driver configuration supported.
        /// </summary>
        Config,

        /// <summary>
        /// Camera can trigger capture and wait for events.
        /// </summary>
        TriggerCapture
    }


    /// <summary>
    /// A bitmask of image related operations of the device.
    /// </summary>
    [Flags]
    internal enum CameraFileOperation
    {
        /// <summary>
        /// No special file operations, just download.
        /// </summary>
        None,

        /// <summary>
        /// Deletion of files is possible.
        /// </summary>
        Delete,

        /// <summary>
        /// Previewing viewfinder content is possible.
        /// </summary>
        Preview,

        /// <summary>
        /// Raw retrieval is possible (used by non-JPEG cameras)
        /// </summary>
        Raw,

        /// <summary>
        /// Audio retrieval is possible.
        /// </summary>
        Audio,

        /// <summary>
        /// EXIF retrieval is possible.
        /// </summary>
        EXIF
    }


    /// <summary>
    /// A bitmask of filesystem related operations of the device.
    /// </summary>
    [Flags]
    internal enum CameraFolderOperation
    {
        /// <summary>
        /// No special filesystem operation.
        /// </summary>
        None,

        /// <summary>
        /// Deletion of all files on the device.
        /// </summary>
        DeleteAll,

        /// <summary>
        /// Upload of files to the device possible.
        /// </summary>
        PutFile,

        /// <summary>
        /// Making directories on the device possible.
        /// </summary>
        MakeDir,

        /// <summary>
        /// Removing directories from the device possible.
        /// </summary>
        RemoveDir
    }


    /// <summary>
    /// Describes the properties of a specific camera.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct CameraAbilities
    {
        /// <summary>
        /// The name of the camera model
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Model;

        /// <summary>
        /// Driver quality
        /// </summary>
        public CameraDriverStatus Status;

        /// <summary>
        /// Supported port types.
        /// </summary>
        public GPPortType Port;

        /// <summary>
        /// Supported serial port speeds (terminated with a value of 0).
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public int[] Speed;

        /// <summary>
        /// Camera operation funcs
        /// </summary>
        public CameraOperation Operations;

        /// <summary>
        /// Camera file op funcs
        /// </summary>
        public CameraFileOperation FileOperations;

        /// <summary>
        /// Camera folder op funcs
        /// </summary>
        public CameraFolderOperation FolderOperations;

        /// <summary>
        /// USB Vendor D
        /// </summary>
        public int UsbVendor;

        /// <summary>
        /// USB Product ID
        /// </summary>
        public int UsbProduct;

        /// <summary>
        /// USB device class
        /// </summary>
        public int UsbClass;

        /// <summary>
        /// USB device subclass
        /// </summary>
        public int UsbSubclass;

        /// <summary>
        /// USB device protocol
        /// </summary>
        public int UsbProtocol;
    }
}
