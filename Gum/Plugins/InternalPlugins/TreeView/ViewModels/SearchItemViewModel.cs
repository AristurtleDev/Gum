﻿using Gum.DataTypes;
using System;
using System.Windows.Media.Imaging;

namespace Gum.Plugins.InternalPlugins.TreeView.ViewModels
{
    public class SearchItemViewModel
    {

        public static BitmapImage ComponentIcon;
        public static BitmapImage ScreenIcon;
        public static BitmapImage StandardIcon;

        static SearchItemViewModel()
        {
            ComponentIcon = LoadIcon("Component");
            ScreenIcon = LoadIcon("screen");
            StandardIcon = LoadIcon("StandardElement");

            BitmapImage LoadIcon(string iconName)
            {
                var location = $"pack://application:,,,/Plugins/InternalPlugins/TreeView/Content/{iconName}.png";
                var bitmapImage = new BitmapImage(new Uri(location, UriKind.Absolute));
                return bitmapImage;
            }
        }

        public object BackingObject { get; set; }

        public string CustomText { get; set; }

        public string Display => !string.IsNullOrWhiteSpace(CustomText) ? CustomText : BackingObject?.ToString();

        public BitmapImage Image =>
            BackingObject is ScreenSave ? ScreenIcon
            : BackingObject is ComponentSave ? ComponentIcon
            : StandardIcon;

    }
}
