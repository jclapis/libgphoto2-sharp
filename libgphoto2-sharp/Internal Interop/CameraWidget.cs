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
    /// This represents the type of a config widget.
    /// </summary>
    internal enum CameraWidgetType
    {
        /// <summary>
        /// The top-level configuration root, which should probably be a window.
        /// </summary>
        Window,

        /// <summary>
        /// A section that contains multiple configuration settings (should be a tab).
        /// </summary>
        Section,

        /// <summary>
        /// A free-form text setting (should be a text box).
        /// </summary>
        Text,

        /// <summary>
        /// A float setting (should be a slider).
        /// </summary>
        Range,

        /// <summary>
        /// A boolean toggle setting (like a checkbox).
        /// </summary>
        Toggle,

        /// <summary>
        /// A selection of multiple options (like a radio or dropdown dialog).
        /// </summary>
        Radio,

        /// <summary>
        /// Same as <see cref="Radio"/>.
        /// </summary>
        Menu,

        /// <summary>
        /// A setting with a custom callback, which should probably be a button.
        /// </summary>
        Button,

        /// <summary>
        /// A date / time setting.
        /// </summary>
        Date
    }


    /// <summary>
    /// A widget that represents a configuration setting (or a recursive selection of settings),
    /// and the suggested type of UI control for that setting.
    /// </summary>
    internal class CameraWidget : IDisposable
    {
        #region Interop from gphoto2-widget.h

        /// <summary>
        /// Frees a widget.
        /// </summary>
        /// <param name="Widget">This <see cref="CameraWidget"/> handle</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_free(IntPtr Widget);


        /// <summary>
        /// Gets the name of the widget.
        /// </summary>
        /// <param name="Widget">This <see cref="CameraWidget"/> handle</param>
        /// <param name="Name">[OUT] The name of the widget</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_get_name(IntPtr Widget, out IntPtr Name);


        /// <summary>
        /// Gets an information string about the widget.
        /// </summary>
        /// <param name="Widget">This <see cref="CameraWidget"/> handle</param>
        /// <param name="Info">[OUT] The widget's info string</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_get_info(IntPtr Widget, out IntPtr Info);


        /// <summary>
        /// Retrieves the type of the widget.
        /// </summary>
        /// <param name="Widget">This <see cref="CameraWidget"/> handle</param>
        /// <param name="Type">[OUT] The widget's type.</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_get_type(IntPtr Widget, out CameraWidgetType Type);


        /// <summary>
        /// Gets a widget's label.
        /// </summary>
        /// <param name="Widget">This <see cref="CameraWidget"/> handle</param>
        /// <param name="Label">[OUT] The widget's label</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_get_label(IntPtr Widget, out IntPtr Label);


        /// <summary>
        /// Gets a widget's internal ID.
        /// </summary>
        /// <param name="Widget">This <see cref="CameraWidget"/> handle</param>
        /// <param name="Label">[OUT] The widget's ID</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_get_id(IntPtr Widget, out int ID);


        /// <summary>
        /// Gets the number of child widgets this widget has.
        /// </summary>
        /// <param name="Widget">This <see cref="CameraWidget"/> handle</param>
        /// <returns>The number of child widgets, or a <see cref="GPResult"/> if something went wrong.</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int gp_widget_count_children(IntPtr Widget);


        /// <summary>
        /// Gets the child widget at the specified index.
        /// </summary>
        /// <param name="Widget">This <see cref="CameraWidget"/> handle</param>
        /// <param name="ChildNumber">The index of the child to get</param>
        /// <param name="Child">[OUT] A handle to the child</param>
        /// <returns>A status code indicating the result of the operation</returns>
        [DllImport(Constants.GPhoto2Lib, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern GPResult gp_widget_get_child(IntPtr Widget, int ChildNumber, out IntPtr Child);

        #endregion


        /// <summary>
        /// The underlying handle used by libgphoto2
        /// </summary>
        public IntPtr Handle { get; }


        /// <summary>
        /// The widget's name
        /// </summary>
        public string Name
        {
            get
            {
                if(DisposedValue)
                {
                    throw new ObjectDisposedException(nameof(CameraWidget));
                }

                GPResult result = gp_widget_get_name(Handle, out IntPtr namePtr);
                if(result != GPResult.Ok)
                {
                    throw new Exception($"Error getting widget name: {result}");
                }
                string name = Marshal.PtrToStringAnsi(namePtr, 256);
                return name;
            }
        }


        /// <summary>
        /// The widget's info
        /// </summary>
        public string Info
        {
            get
            {
                if (DisposedValue)
                {
                    throw new ObjectDisposedException(nameof(CameraWidget));
                }

                GPResult result = gp_widget_get_info(Handle, out IntPtr infoPtr);
                if (result != GPResult.Ok)
                {
                    throw new Exception($"Error getting widget info: {result}");
                }
                string info = Marshal.PtrToStringAnsi(infoPtr, 1024);
                return info;
            }
        }


        /// <summary>
        /// The widget's type
        /// </summary>
        public CameraWidgetType Type
        {
            get
            {
                if (DisposedValue)
                {
                    throw new ObjectDisposedException(nameof(CameraWidget));
                }

                GPResult result = gp_widget_get_type(Handle, out CameraWidgetType type);
                if (result != GPResult.Ok)
                {
                    throw new Exception($"Error getting widget type: {result}");
                }
                return type;
            }
        }


        /// <summary>
        /// The widget's info
        /// </summary>
        public string Label
        {
            get
            {
                if (DisposedValue)
                {
                    throw new ObjectDisposedException(nameof(CameraWidget));
                }

                GPResult result = gp_widget_get_label(Handle, out IntPtr labelPtr);
                if (result != GPResult.Ok)
                {
                    throw new Exception($"Error getting widget label: {result}");
                }
                string label = Marshal.PtrToStringAnsi(labelPtr, 256);
                return label;
            }
        }


        /// <summary>
        /// The widget's ID
        /// </summary>
        public int ID
        {
            get
            {
                if (DisposedValue)
                {
                    throw new ObjectDisposedException(nameof(CameraWidget));
                }

                GPResult result = gp_widget_get_id(Handle, out int id);
                if (result != GPResult.Ok)
                {
                    throw new Exception($"Error getting widget ID: {result}");
                }
                return id;
            }
        }


        public IReadOnlyList<CameraWidget> Children
        {
            get
            {
                if (DisposedValue)
                {
                    throw new ObjectDisposedException(nameof(CameraWidget));
                }

                int childCount = gp_widget_count_children(Handle);
                if (childCount < (int)GPResult.Ok)
                {
                    throw new Exception($"Error getting number of widget children: {(GPResult)childCount}");
                }

                List<CameraWidget> children = new List<CameraWidget>();
                for(int i = 0; i < childCount; i++)
                {
                    GPResult result = gp_widget_get_child(Handle, i, out IntPtr childHandle);
                    if(result != GPResult.Ok)
                    {
                        throw new Exception($"Error getting child {i} of widget: {result}");
                    }
                    children.Add(new CameraWidget(childHandle));
                }
                return children;
            }
        }


        /// <summary>
        /// Creates a new <see cref="CameraWidget"/> instance.
        /// </summary>
        /// <param name="Handle">he underlying handle used by libgphoto2</param>
        public CameraWidget(IntPtr Handle)
        {
            this.Handle = Handle;
        }


        #region IDisposable Support
        private bool DisposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                gp_widget_free(Handle);
                DisposedValue = true;
            }
        }

        ~CameraWidget()
        {
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
