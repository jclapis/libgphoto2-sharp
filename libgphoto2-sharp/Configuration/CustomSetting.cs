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
    /// This is the signature for custom setting callbacks.
    /// </summary>
    /// <param name="Camera">The handle of the <see cref="Camera"/> that owns the setting</param>
    /// <param name="Widget">The handle of the <see cref="CameraWidget"/> representing this setting</param>
    /// <param name="Context">The handle of the <see cref="Context"/> that is currently running</param>
    /// <returns>A status code indicating the result of the operation</returns>
    public delegate GPResult CustomSettingCallback(IntPtr Camera, IntPtr Widget, IntPtr Context);

    public class CustomSetting : Setting
    {
        #region Interop from gphoto2-widget.h

        /// <summary>
        /// Gets the value of this widget.
        /// </summary>
        /// <param name="Widget">The underlying <see cref="CameraWidget"/> handle</param>
        /// <param name="Value">A pointer to the widget's <see cref="CustomSettingCallback"/></param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_get_value(IntPtr Widget, out IntPtr Value);


        /// <summary>
        /// Sets the value of this setting.
        /// </summary>
        /// <param name="Widget">The underlying <see cref="CameraWidget"/> handle</param>
        /// <param name="Value">The pointer to the <see cref="CustomSettingCallback"/> to use.</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_set_value(IntPtr Widget, IntPtr Value);

        #endregion

        internal CustomSetting(CameraWidget Widget)
            : base(Widget)
        {

        }

        public override string ToString()
        {
            return $"{Title} (Custom): <Callback>";
        }

    }
}
