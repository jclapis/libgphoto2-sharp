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
        public CameraDriverQuality Status;

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


        // ===== Internal Members for libgphoto2 Use =====

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        private string Library;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        private string ID;

        private GPhotoDeviceType DeviceType;

        private int Reserved2;
        private int Reserved3;
        private int Reserved4;
        private int Reserved5;
        private int Reserved6;
        private int Reserved7;
        private int Reserved8;
    }
}
