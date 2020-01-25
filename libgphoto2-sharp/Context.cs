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
    /// This class allows callback handling, passing error contexts back, 
    /// progress handling and download cancellation and similar things.
    /// It is usually passed around the functions.
    /// </summary>
    public class Context : IDisposable
    {
        /// <summary>
        /// A handle to the native GPContext object
        /// </summary>
        internal IntPtr GPContextHandle { get; private set; }


        /// <summary>
        /// Creates a new Context instance.
        /// </summary>
        public Context()
        {
            GPContextHandle = Interop.gp_context_new();
            if(GPContextHandle == IntPtr.Zero)
            {
                throw new Exception("Context creation failed.");
            }
        }


        /// <summary>
        /// Retrieves a collection of the cameras that are connected to
        /// this machine.
        /// </summary>
        /// <returns>A collection of available cameras</returns>
        public IEnumerable<Camera> GetCameras()
        {
            // TODO
            return null;
        }


        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                if(GPContextHandle != IntPtr.Zero)
                {
                    Interop.gp_context_unref(GPContextHandle);
                    GPContextHandle = IntPtr.Zero;
                }

                disposedValue = true;
            }
        }

        ~Context()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
