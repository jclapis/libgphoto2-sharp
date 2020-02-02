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
    public class ConfigurationSection
    {
        private readonly CameraWidget Widget;

        private readonly int ID;

        public string Title { get; }

        public IReadOnlyList<Setting> Settings { get; }

        internal ConfigurationSection(CameraWidget Widget)
        {
            this.Widget = Widget;
            ID = Widget.ID;
            Title = Widget.Label;

            List<Setting> settings = new List<Setting>();
            IReadOnlyList<CameraWidget> children = Widget.Children;
            foreach (CameraWidget child in children)
            {
                switch (child.Type)
                {
                    case CameraWidgetType.Text:
                        TextSetting textSetting = new TextSetting(child);
                        settings.Add(textSetting);
                        break;

                    case CameraWidgetType.Radio:
                    case CameraWidgetType.Menu:
                        SelectionSetting selectionSetting = new SelectionSetting(child);
                        settings.Add(selectionSetting);
                        break;

                    case CameraWidgetType.Range:
                        RangeSetting rangeSetting = new RangeSetting(child);
                        settings.Add(rangeSetting);
                        break;

                    case CameraWidgetType.Toggle:
                        ToggleSetting toggleSetting = new ToggleSetting(child);
                        settings.Add(toggleSetting);
                        break;

                    case CameraWidgetType.Date:
                        DateSetting dateSetting = new DateSetting(child);
                        settings.Add(dateSetting);
                        break;

                    case CameraWidgetType.Button:
                        CustomSetting customSetting = new CustomSetting(child);
                        settings.Add(customSetting);
                        break;

                    case CameraWidgetType.Window:
                        throw new Exception($"Found a {nameof(CameraWidgetType.Window)} widget that wasn't the root of a configuration.");

                    case CameraWidgetType.Section:
                        throw new Exception($"Found a {nameof(CameraWidgetType.Section)} widget that was a child of another section.");

                }
            }

            Settings = settings;
        }

        internal ConfigurationSection(List<Setting> Settings)
        {
            Title = "Uncategorized Settings";
            this.Settings = Settings;
        }

        public void ToString(StringBuilder Builder, string Indent)
        {
            Builder.AppendLine($"{Indent}{Title}:");
            string childIndent = Indent + "\t";
            foreach(Setting setting in Settings)
            {
                Builder.AppendLine($"{childIndent}{setting}");
            }
        }

    }
}
