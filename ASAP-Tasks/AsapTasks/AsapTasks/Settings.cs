using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsapTasks
{
    public static class Settings
    {
        private static ISettings AppSetting => CrossSettings.Current;

        public static string DeveloperId
        {
            get => AppSetting.GetValueOrDefault(nameof(DeveloperId), string.Empty);

            set => AppSetting.AddOrUpdateValue(nameof(DeveloperId), value);
        }
    }
}
