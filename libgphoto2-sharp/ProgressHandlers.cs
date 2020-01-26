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
    /// This is sent by the <see cref="Context"/> class when a new operation begins
    /// that supports progress tracking.
    /// </summary>
    public class ProgressStartArgs : EventArgs
    {
        /// <summary>
        /// The ID of the progress-tracking event, which is necessary since multiple
        /// events can occur simultaneously.
        /// </summary>
        public uint ProgressID { get; }


        /// <summary>
        /// The message delivered with the start of the event, indicating some information
        /// about it (such as the cause, or the type).
        /// </summary>
        public string Message { get; }


        /// <summary>
        /// Creates a new ProgressStartArgs instance.
        /// </summary>
        /// <param name="ProgressID">
        /// The ID of the progress-tracking event, which is necessary since multiple
        /// events can occur simultaneously.
        /// </param>
        /// <param name="Message">
        /// The message delivered with the start of the event, indicating some information
        /// about it (such as the cause, or the type).
        /// </param>
        internal ProgressStartArgs(uint ProgressID, string Message)
        {
            this.ProgressID = ProgressID;
            this.Message = Message;
        }
    }


    /// <summary>
    /// This is sent by the <see cref="Context"/> class when an operation with progress
    /// tracking support updates its progress.
    /// </summary>
    public class ProgressUpdateArgs : EventArgs
    {
        /// <summary>
        /// The ID of the progress-tracking event, which is necessary since multiple
        /// events can occur simultaneously.
        /// </summary>
        public uint ProgressID { get; }


        /// <summary>
        /// The current progress of the event, from 0 to 1. Once it reaches 1, the
        /// operation should be complete.
        /// </summary>
        public float Progress { get; }


        /// <summary>
        /// Creates a new ProgressUpdateArgs instance.
        /// </summary>
        /// <param name="ProgressID">
        /// The ID of the progress-tracking event, which is necessary since multiple
        /// events can occur simultaneously.
        /// </param>
        /// <param name="Progress">
        /// The current progress of the event, from 0 to 1. Once it reaches 1, the
        /// operation should be complete.
        /// </param>
        internal ProgressUpdateArgs(uint ProgressID, float Progress)
        {
            this.ProgressID = ProgressID;
            this.Progress = Progress;
        }
    }

}
