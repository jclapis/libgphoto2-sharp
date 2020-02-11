﻿/* ========================================================================
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
    /// The type of a file on retrieve from the camera.
    /// </summary>
    internal enum CameraFileType
    {
        /// <summary>
        /// A preview of an image.
        /// </summary>
        Preview,

        /// <summary>
        /// The regular normal data of a file.
        /// </summary>
        Normal,

        /// <summary>
        /// The raw mode of a file, for instance the raw bayer data for cameras
		/// where postprocessing is done in the driver. The RAW files of modern
        /// DSLRs are <see cref="Normal"/> usually.
        /// </summary>
        Raw,

        /// <summary>
        /// The audio view of a file. Perhaps an embedded comment or similar.
        /// </summary>
        Audio,

        /// <summary>
        /// The embedded EXIF data of an image.
        /// </summary>
        Exif,

        /// <summary>
        /// The metadata of a file, like Metadata of files on MTP devices.
        /// </summary>
        Metadata
    }


    /// <summary>
    /// This class represents a file that was retrieved from the camera's filesystem.
    /// </summary>
    public class CameraFile : IDisposable
    {
        #region Interop from gphoto2-file.h

        /// <summary>
        /// Creates a new CameraFile object.
        /// </summary>
        /// <param name="File">[OUT] This <see cref="CameraFile"/> handle</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_file_new(out IntPtr File);


        /// <summary>
        /// Destroys a CameraFile object.
        /// </summary>
        /// <param name="File">This <see cref="CameraFile"/> handle</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_file_free(IntPtr InfoFile);


        /// <summary>
        /// Get a pointer to the data and the file's size.
        /// </summary>
        /// <param name="File">This <see cref="CameraFile"/> handle</param>
        /// <param name="Data">[OUT] A pointer to the file's data</param>
        /// <param name="Size">[OUT] The size of the file's data, in bytes</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_file_get_data_and_size(IntPtr File, out IntPtr Data, out ulong Size);

        #endregion


        internal IntPtr Handle { get; }


        public string Folder { get; }


        public string Name { get; }


        public byte[] Data { get; private set; }


        public CameraFile(string Folder, string Name)
        {
            this.Folder = Folder;
            this.Name = Name;

            GPResult result = gp_file_new(out IntPtr handle);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Error creating new file for {Name}: {result}");
            }
            Handle = handle;
        }


        public (IntPtr Buffer, int Size) GetBuffer()
        {
            GPResult result = gp_file_get_data_and_size(Handle, out IntPtr dataPtr, out ulong size);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Error getting buffer for {Name}: {result}");
            }

            return (dataPtr, (int)size);
        }


        internal void DownloadData()
        {
            GPResult result = gp_file_get_data_and_size(Handle, out IntPtr dataPtr, out ulong size);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Error downloading file {Name}: {result}");
            }

            Data = new byte[size];
            Marshal.Copy(dataPtr, Data, 0, Data.Length);
        }


        #region IDisposable Support
        private bool DisposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {

                }
                gp_file_free(Handle);
                DisposedValue = true;
            }
        }

        ~CameraFile()
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
