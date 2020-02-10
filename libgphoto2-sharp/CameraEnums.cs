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

namespace GPhoto2.Net
{
    /// <summary>
    /// The type of capture to perform when asking the camera to capture.
    /// </summary>
    public enum CameraCaptureType
    {
        /// <summary>
        /// Capture an image
        /// </summary>
        Image,

        /// <summary>
        /// Capture a movie
        /// </summary>
        Movie,

        /// <summary>
        /// Capture audio
        /// </summary>
        Sound
    }


    /// <summary>
    /// Current implementation status of the camera driver.
    /// </summary>
    public enum CameraDriverQuality
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
    public enum GPhotoDeviceType
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
    public enum CameraOperation
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
    public enum CameraFileOperation
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
    public enum CameraFolderOperation
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
    /// The gphoto port type.
    /// </summary>
    [Flags]
    public enum GPPortType
    {
        /// <summary>
        /// No specific type associated.
        /// </summary>
        None,

        /// <summary>
        /// Serial port.
        /// </summary>
        Serial,

        /// <summary>
        /// USB port.
        /// </summary>
        USB,

        /// <summary>
        /// Disk / local mountpoint port.
        /// </summary>
        Disk,

        /// <summary>
        /// PTP/IP port.
        /// </summary>
        PTP_IP,

        /// <summary>
        /// Direct I/O to a USB mass storage device.
        /// </summary>
        UsbDiskDirect,

        /// <summary>
        /// USB Mass Storage raw SCSI port.
        /// </summary>
        UsbSCSI
    }

}
