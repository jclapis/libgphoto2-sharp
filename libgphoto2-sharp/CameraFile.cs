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
        private static extern GPResult gp_file_free(IntPtr File);


        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="File">This <see cref="CameraFile"/> handle</param>
        /// <param name="Name">[OUT] The name of the file</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_file_get_name(IntPtr File, out IntPtr Name);


        /// <summary>
        /// Get a pointer to the data and the file's size.
        /// </summary>
        /// <param name="File">This <see cref="CameraFile"/> handle</param>
        /// <param name="Data">[OUT] A pointer to the file's data</param>
        /// <param name="Size">[OUT] The size of the file's data, in bytes</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_file_get_data_and_size(IntPtr File, out IntPtr Data, out ulong Size);


        /// <summary>
        /// Gets the MIME type of the file.
        /// </summary>
        /// <param name="File">This <see cref="CameraFile"/> handle</param>
        /// <param name="MimeType">[OUT] The file's MIME type</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_file_get_mime_type(IntPtr File, out IntPtr MimeType);


        /// <summary>
        /// Gets the time that the file was last modified.
        /// </summary>
        /// <param name="File">This <see cref="CameraFile"/> handle</param>
        /// <param name="MimeType">[OUT] The file's modification time</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_file_get_mtime(IntPtr File, out long MTime);


        /// <summary>
        /// Determines the MIME type of the file based on its header information.
        /// </summary>
        /// <param name="File">This <see cref="CameraFile"/> handle</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_file_detect_mime_type(IntPtr File);

        #endregion

        private static readonly DateTime UnixEpoch;


        internal IntPtr Handle { get; }


        public string Name { get; private set; }


        public string MimeType { get; private set; }


        public DateTime ModifiedTime { get; private set; }


        public IntPtr Data { get; private set; }


        public ulong Size { get; private set; }


        static CameraFile()
        {
            UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
        }


        internal CameraFile(string Name = null)
        {
            GPResult result = gp_file_new(out IntPtr handle);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Error creating new file for {Name}: {result}");
            }
            Handle = handle;
            if(Name != null)
            { 
                this.Name = Name; 
            }
        }


        internal void Initialize()
        {
            GPResult result;
            if (Name == null)
            {
                result = gp_file_get_name(Handle, out IntPtr namePtr);
                if (result != GPResult.Ok)
                {
                    throw new Exception($"Error getting name of file: {result}");
                }
                Name = Marshal.PtrToStringAnsi(namePtr);
            }

            result = gp_file_get_mime_type(Handle, out IntPtr mimeTypePtr);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Error getting MIME type of {Name}: {result}");
            }
            MimeType = Marshal.PtrToStringAnsi(mimeTypePtr);


            result = gp_file_get_mtime(Handle, out long time);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Error getting modified time of {Name}: {result}");
            }
            ModifiedTime = UnixEpoch.AddSeconds(time);

            result = gp_file_get_data_and_size(Handle, out IntPtr dataPtr, out ulong size);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Error getting buffer for {Name}: {result}");
            }
            Data = dataPtr;
            Size = size;
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
