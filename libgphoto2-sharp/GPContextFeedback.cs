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
    /// An application can return special values back to the libgphoto2
    /// progress callback handling functions. If "Cancel" is selected,
    /// libgphoto2 and the camera driver will try to cancel transfer.
    /// </summary>
    public enum GPContextFeedback
    {
        /// <summary>
        /// Everything ok... proceed.
        /// </summary>
        OK,

        /// <summary>
        /// Please cancel the current transfer if possible.
        /// </summary>
        Cancel
    }
}
