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

namespace GPhoto2.Net
{
    /// <summary>
    /// This represents the result values that GPhoto2 returns from each of its
    /// function calls.
    /// </summary>
    public enum GPResult
    {
        /// <summary>
        /// The function succeeded.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Generic error
        /// </summary>
        Error = -1,

        /// <summary>
        /// Bad parameters passed
        /// </summary>
        BadParametersError = -2,

        /// <summary>
        /// Out of memory
        /// </summary>
        NoMemoryError = -3,

        /// <summary>
        /// Error in the camera driver
        /// </summary>
        LibraryError = -4,

        /// <summary>
        /// Unknown libgphoto2 port passed
        /// </summary>
        UnknownPortError = -5,

        /// <summary>
        /// Functionality not supported
        /// </summary>
        NotSupportedError = -6,

        /// <summary>
        /// Generic I/O error
        /// </summary>
        IOError = -7,

        /// <summary>
        /// Buffer overflow of internal structure
        /// </summary>
        FixedLimitExceededError = -8,

        /// <summary>
        /// Operation timed out
        /// </summary>
        TimeoutError = -10,

        /// <summary>
        /// Serial ports not supported
        /// </summary>
        SerialNotSupportedError = -20,

        /// <summary>
        /// USB ports not supported
        /// </summary>
        UsbNotSupportedError = -21,

        /// <summary>
        /// Error initializing I/O
        /// </summary>
        IOInitializationError = -31,

        /// <summary>
        /// Error during an I/O read operation
        /// </summary>
        IOReadError = -34,

        /// <summary>
        /// Error during an I/O write operation
        /// </summary>
        IOWriteError = -35,

        /// <summary>
        /// I/O error during a settings update
        /// </summary>
        IOUpdateError = -37,

        /// <summary>
        /// Specified serial speed is not possible
        /// </summary>
        IOSerialSpeedError = -41,

        /// <summary>
        /// Error during USB Clear HALT
        /// </summary>
        IOUsbClearHaltError = -51,

        /// <summary>
        /// Error while trying to find the USB device
        /// </summary>
        IOUsbFindError = -52,

        /// <summary>
        /// Error while trying to claim the USB device
        /// </summary>
        IOUsbClaimError = -53,

        /// <summary>
        /// Error when trying to lock the device
        /// </summary>
        IOLockError = -60,

        /// <summary>
        /// Unspecified error when talking to HAL
        /// </summary>
        HalError = -70,

        /// <summary>
        /// Data is corrupt. This error is reported by camera drivers if corrupted
        /// data has been received that can not be automatically handled. Normally,
        /// drivers will do everything possible to automatically recover from this
        /// error.
        /// </summary>
        CorruptedDataError = -102,

        /// <summary>
        /// An operation failed because a file existed. This error is reported for
        /// example when the user tries to create a file that already exists.
        /// </summary>
        FileExistsError = -103,

        /// <summary>
        /// The specified model could not be found. This error is reported when
        /// the user specified a model that does not seem to be supported by 
        /// any driver.
        /// </summary>
        ModelNotFoundError = -105,

        /// <summary>
        /// The specified directory could not be found. This error is reported when
        /// the user specified a directory that is non-existent.
        /// </summary>
        DirectoryNotFoundError = -107,

        /// <summary>
        /// The specified file could not be found. This error is reported when
        /// the user wants to access a file that is non-existent.
        /// </summary>
        FileNotFoundError = -108,

        /// <summary>
        /// The specified directory already exists. This error is reported for example 
        /// when the user wants to create a directory that already exists.
        /// </summary>
        DirectoryExistsError = -109,

        /// <summary>
        /// Camera I/O or a command is already in progress.
        /// </summary>
        CameraBusyError = -110,

        /// <summary>
        /// The specified path is not absolute. This error is reported when the user
        /// specifies paths that are not absolute, i.e. paths like "path/to/directory".
        /// As a rule of thumb, in gphoto2, there is nothing like relative paths.
        /// </summary>
        PathNotAbsoluteError = -111,

        /// <summary>
        /// A cancellation requestion by the frontend via progress callback and
        /// GP_CONTEXT_FEEDBACK_CANCEL was successful and the transfer has been aborted.
        /// </summary>
        CancelSucceeded = -112,

        /// <summary>
        /// The camera reported some kind of error. This can be either a
        /// photographic error, such as failure to autofocus, underexposure, or
        /// violating storage permission, anything else that stops the camera
        /// from performing the operation.
        /// </summary>
        UnspecifiedCameraError = -113,

        /// <summary>
        /// There was some sort of OS error in communicating with the camera,
        /// e.g. lack of permission for an operation.
        /// </summary>
        OSFailureError = -114,

        /// <summary>
        /// There was not enough free space when uploading a file.
        /// </summary>
        NoSpaceError = -115
    }
}
