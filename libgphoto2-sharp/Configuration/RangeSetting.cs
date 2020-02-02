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
    public class RangeSetting : Setting
    {
        #region Interop from gphoto2-widget.h

        /// <summary>
        /// Gets the value of this widget.
        /// </summary>
        /// <param name="Widget">The underlying <see cref="CameraWidget"/> handle</param>
        /// <param name="Value">The widget's value</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_get_value(IntPtr Widget, out float Value);


        /// <summary>
        /// Gets the range details for this setting.
        /// </summary>
        /// <param name="Widget">The underlying <see cref="CameraWidget"/> handle</param>
        /// <param name="Min">[OUT] The minimum value</param>
        /// <param name="Max">[OUT] The maximum value</param>
        /// <param name="Increment">[OUT] The accepted increment value</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_get_range(IntPtr Widget, out float Min, out float Max, out float Increment);


        /// <summary>
        /// Sets the value of this setting.
        /// </summary>
        /// <param name="Widget">The underlying <see cref="CameraWidget"/> handle</param>
        /// <param name="Value">The new value to set</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_set_value(IntPtr Widget, ref float Value);

        #endregion

        public float Minimum { get; }

        public float Maximum { get; }

        public float Increment { get; }


        internal RangeSetting(CameraWidget Widget)
            : base(Widget)
        {
            GPResult result = gp_widget_get_range(Widget.Handle, out float min, out float max, out float increment);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Error getting range for {Title}: {result}");
            }

            Minimum = min;
            Maximum = max;
            Increment = increment;
        }

        protected override string GetValueAsString()
        {
            return string.Empty;
        }

        public override string ToString()
        {
            GPResult result = gp_widget_get_value(Widget.Handle, out float value);
            if(result != GPResult.Ok)
            {
                throw new Exception($"Error getting value for {Title}: {result}");
            }

            return $"{Title}: Min = {Minimum}, Max = {Maximum}, Increment = {Increment}, Current Value = {value}";
        }

    }
}
