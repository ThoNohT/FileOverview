/*
Copyright (C) 2016 by Eric Bataille <e.c.p.bataille@gmail.com>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace ThoNohT.FileOverview.TreeBuilder.CSharp
{
    /// <summary>
    /// Contains all images that can be used in file overview tree.
    /// </summary>
    public static class TreeImages
    {
        /// <summary>
        /// The cache containing all images that were ever generated.
        /// </summary>
        private static Dictionary<string, BitmapImage> imageCache = new Dictionary<string, BitmapImage>();

        /// <summary>
        /// Retrieves an image with the specified parameters. The image is retrieved from cache if it was gerneated
        /// before, otherwise it is generated anew.
        /// </summary>
        /// <param name="type">The element type.</param>
        /// <param name="modifiers">The access type.</param>
        /// <param name="static">A value indicating whether the element is static or not.</param>
        /// <returns>The retrieved image.</returns>
        public static BitmapImage GetImage(ElementType type, ElementModifier modifiers = ElementModifier.None)
        {
            var key = $"{type}:{modifiers}";

            if (!imageCache.ContainsKey(key)) imageCache.Add(key, CreateImage(type, modifiers));

            return imageCache[key];
        }

        /// <summary>
        /// Creates a new image with the specified parameters.
        /// </summary>
        /// <param name="type">The element type.</param>
        /// <param name="modifiers">The element modifiers.</param>
        /// <returns>The generated image.</returns>
        private static BitmapImage CreateImage(ElementType type, ElementModifier modifiers)
        {
            var result = new Bitmap(GetElementImage(type));
            using (var g = Graphics.FromImage(result))
            {
                foreach (var image in GetModifierImage(modifiers))
                    g.DrawImage(image, new Point(0, 0));
            }

            return result.ToBitmapImage();
        }

        /// <summary>
        /// Retrieves the images that are used to specify the provided modifiers.
        /// </summary>
        /// <param name="modifier">The modifiers to get the images for.</param>
        /// <returns>The images.</returns>
        private static IEnumerable<Image> GetModifierImage(ElementModifier modifier)
        {
            // Access
            if ((modifier & ElementModifier.Public) != 0)
                yield return Properties.Resources._public;
            if ((modifier & ElementModifier.Internal) != 0)
                yield return Properties.Resources._internal;
            if ((modifier & ElementModifier.Protected) != 0)
                yield return Properties.Resources._protected;
            if ((modifier & ElementModifier.Private) != 0)
                yield return Properties.Resources._private;

            // Other
            if ((modifier & ElementModifier.Partial) != 0)
                yield return Properties.Resources.partial;
            if ((modifier & ElementModifier.Static) != 0)
                yield return Properties.Resources._static;
            if ((modifier & ElementModifier.Sealed) != 0)
                yield return Properties.Resources._sealed;

        }

        /// <summary>
        /// Retrieves the image that used as the base for the specified element type.
        /// </summary>
        /// <param name="type">The type to get the image for.</param>
        /// <returns>The image.</returns>
        private static Image GetElementImage(ElementType type)
        {
            switch (type)
            {
                case ElementType.Unknown:
                    return Properties.Resources.unknown;
                case ElementType.Namespace:
                    return Properties.Resources._namespace;
                case ElementType.Class:
                    return Properties.Resources._class;
                case ElementType.Interface:
                    return Properties.Resources._interface;
                case ElementType.Struct:
                    return Properties.Resources._struct;
                case ElementType.Enum:
                    return Properties.Resources._enum;
                case ElementType.Constructor:
                    return Properties.Resources.constructor;
                case ElementType.Method:
                    return Properties.Resources.method;
                case ElementType.Property:
                    return Properties.Resources.property;
                case ElementType.Field:
                    return Properties.Resources.field;
                case ElementType.EnumValue:
                    return Properties.Resources.enumvalue;
                case ElementType.Region:
                    return Properties.Resources.region;
                case ElementType.Operator:
                    return Properties.Resources._operator;
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }

        /// <summary>
        /// Convert a <see cref="Bitmap"/> to a <see cref="BitmapImage"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap to convert.</param>
        /// <returns>The converted bitmap image.</returns>
        private static BitmapImage ToBitmapImage(this Image image)
        {
            using (var memory = new MemoryStream())
            {
                image.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
}