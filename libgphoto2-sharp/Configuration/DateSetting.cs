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
    public class DateSetting : Setting
    {
        #region Interop from gphoto2-widget.h

        /// <summary>
        /// Gets the value of this widget.
        /// </summary>
        /// <param name="Widget">The underlying <see cref="CameraWidget"/> handle</param>
        /// <param name="Value">The widget's value</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_get_value(IntPtr Widget, out int Value);


        /// <summary>
        /// Sets the value of this setting.
        /// </summary>
        /// <param name="Widget">The underlying <see cref="CameraWidget"/> handle</param>
        /// <param name="Value">The new value to set</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_set_value(IntPtr Widget, ref int Value);

        #endregion

        internal DateSetting(CameraWidget Widget)
            : base(Widget)
        {

        }

        protected override string GetValueAsString()
        {
            GPResult result = gp_widget_get_value(Widget.Handle, out int value);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Error getting value for {Title}: {result}");
            }

            return value.ToString();
        }

    }
}
