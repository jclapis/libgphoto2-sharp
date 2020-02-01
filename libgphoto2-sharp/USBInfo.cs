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
    /// This contains information about a camera's USB details.
    /// </summary>
    public class USBInfo
    {
        /// <summary>
        /// The camera's vendor ID
        /// </summary>
        public int VendorID { get; }


        /// <summary>
        /// The camera's product ID
        /// </summary>
        public int ProductID { get; }


        /// <summary>
        /// The camera's USB class
        /// </summary>
        public int Class { get; }


        /// <summary>
        /// The camera's USB subclass
        /// </summary>
        public int Subclass { get; }


        /// <summary>
        /// The camera's supported USB protocol
        /// </summary>
        public int Protocol { get; }


        /// <summary>
        /// Creates a new <see cref="USBInfo"/> instance.
        /// </summary>
        /// <param name="VendorID">The camera's vendor ID</param>
        /// <param name="ProductID">The camera's product ID</param>
        /// <param name="Class">The camera's USB class</param>
        /// <param name="Subclass">The camera's USB subclass</param>
        /// <param name="Protocol">The camera's supported USB protocol</param>
        internal USBInfo(int VendorID, int ProductID, int Class, int Subclass, int Protocol)
        {
            this.VendorID = VendorID;
            this.ProductID = ProductID;
            this.Class = Class;
            this.Subclass = Subclass;
            this.Protocol = Protocol;
        }

    }
}
