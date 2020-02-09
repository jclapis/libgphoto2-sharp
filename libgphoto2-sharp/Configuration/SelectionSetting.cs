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
    public class SelectionSetting : Setting
    {
        #region Interop from gphoto2-widget.h

        /// <summary>
        /// Gets the value of this widget.
        /// </summary>
        /// <param name="Widget">The underlying <see cref="CameraWidget"/> handle</param>
        /// <param name="Value">The widget's value</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_get_value(IntPtr Widget, out IntPtr Value);


        /// <summary>
        /// Gets the number of valid choices for this setting.
        /// </summary>
        /// <param name="Widget">The underlying <see cref="CameraWidget"/> handle</param>
        /// <returns>The number of available choices, or a <see cref="GPResult"/> if something went wrong.</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int gp_widget_count_choices(IntPtr Widget);


        /// <summary>
        /// Gets the choice at the specified index.
        /// </summary>
        /// <param name="Widget">The underlying <see cref="CameraWidget"/> handle</param>
        /// <param name="ChoiceNumber">The index of the choice to get</param>
        /// <param name="Choice">[OUT] The choice at the given index</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_get_choice(IntPtr Widget, int ChoiceNumber, out IntPtr Choice);


        /// <summary>
        /// Sets the value of this setting.
        /// </summary>
        /// <param name="Widget">The underlying <see cref="CameraWidget"/> handle</param>
        /// <param name="Value">The new value to set</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_set_value(IntPtr Widget, string Value);

        #endregion


        public IReadOnlyList<string> Options { get; }


        public string Value
        {
            get
            {
                GPResult result = gp_widget_get_value(Widget.Handle, out IntPtr valuePtr);
                if(result != GPResult.Ok)
                {
                    throw new Exception($"Error getting value for {Title}: {result}");
                }

                string value = Marshal.PtrToStringAnsi(valuePtr);
                return value;
            }
            set
            {
                GPResult result = gp_widget_set_value(Widget.Handle, value);
                if (result != GPResult.Ok)
                {
                    throw new Exception($"Error setting value for {Title}: {result}");
                }
            }
        }


        internal SelectionSetting(CameraWidget Widget)
            : base(Widget)
        {
            List<string> options = new List<string>();

            int numberOfChoices = gp_widget_count_choices(Widget.Handle);
            if (numberOfChoices < (int)GPResult.Ok)
            {
                throw new Exception($"Error getting number of options for {Title}: {(GPResult)numberOfChoices}");
            }

            for(int i = 0; i < numberOfChoices; i++)
            {
                GPResult result = gp_widget_get_choice(Widget.Handle, i, out IntPtr optionPtr);
                if(result != GPResult.Ok)
                {
                    throw new Exception($"Error getting option {i} for {Title}: {result}");
                }
                string option = Marshal.PtrToStringAnsi(optionPtr);
                options.Add(option);
            }

            Options = options;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{Title} (Selection): Options = ");
            builder.Append(Options[0]);

            for(int i = 1; i < Options.Count; i++)
            {
                builder.Append($", {Options[i]}");
            }

            GPResult result = gp_widget_get_value(Widget.Handle, out IntPtr valuePtr);
            if (result != GPResult.Ok)
            {
                throw new Exception($"Error getting current value for {Title}: {result}");
            }

            string value = Marshal.PtrToStringAnsi(valuePtr);
            builder.Append($"; Current = {value}");
            return builder.ToString();
        }

    }
}
