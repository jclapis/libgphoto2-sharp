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
using System.Text;

namespace GPhoto2.Net
{
    public class CameraConfiguration : IDisposable
    {
        private readonly int ID;

        internal CameraWidget Widget { get; }

        public string Title { get; }

        public IReadOnlyList<ConfigurationSection> Sections { get; }

        internal CameraConfiguration(CameraWidget Widget)
        {
            if (Widget.Type != CameraWidgetType.Window)
            {
                throw new Exception($"Creating a new configuration must come from a {nameof(CameraWidgetType.Window)} widget, " +
                    $"but the provided type was {Widget.Type}.");
            }

            this.Widget = Widget;
            ID = Widget.ID;
            Title = Widget.Label;

            List<ConfigurationSection> sections = new List<ConfigurationSection>();
            List<Setting> uncategorizedSettings = new List<Setting>();

            IReadOnlyList<CameraWidget> children = Widget.Children;
            foreach(CameraWidget child in children)
            {
                switch (child.Type)
                {
                    case CameraWidgetType.Section:
                        ConfigurationSection section = new ConfigurationSection(child);
                        sections.Add(section);
                        break;

                    case CameraWidgetType.Text:
                        TextSetting textSetting = new TextSetting(child);
                        uncategorizedSettings.Add(textSetting);
                        break;

                    case CameraWidgetType.Radio:
                    case CameraWidgetType.Menu:
                        SelectionSetting selectionSetting = new SelectionSetting(child);
                        uncategorizedSettings.Add(selectionSetting);
                        break;

                    case CameraWidgetType.Range:
                        RangeSetting rangeSetting = new RangeSetting(child);
                        uncategorizedSettings.Add(rangeSetting);
                        break;

                    case CameraWidgetType.Toggle:
                        ToggleSetting toggleSetting = new ToggleSetting(child);
                        uncategorizedSettings.Add(toggleSetting);
                        break;

                    case CameraWidgetType.Date:
                        DateSetting dateSetting = new DateSetting(child);
                        uncategorizedSettings.Add(dateSetting);
                        break;

                    case CameraWidgetType.Button:
                        CustomSetting customSetting = new CustomSetting(child);
                        uncategorizedSettings.Add(customSetting);
                        break;

                    case CameraWidgetType.Window:
                        throw new Exception($"Found a {nameof(CameraWidgetType.Window)} widget that wasn't the root of a configuration.");
                }
            }

            if(uncategorizedSettings.Count > 0)
            {
                ConfigurationSection defaultSection = new ConfigurationSection(uncategorizedSettings);
                sections.Add(defaultSection);
            }

            Sections = sections;
        }


        public void ToString(StringBuilder Builder, string Indent)
        {
            foreach(ConfigurationSection section in Sections)
            {
                section.ToString(Builder, Indent + "\t");
            }
        }


        #region IDisposable Support
        private bool DisposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    Widget.Dispose();
                }
                DisposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
