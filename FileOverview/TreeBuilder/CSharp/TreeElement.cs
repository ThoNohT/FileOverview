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

namespace ThoNohT.FileOverview.TreeBuilder.CSharp
{
    using System;
    using System.Windows.Media.Imaging;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// An element in the file tree.
    /// </summary>
    class TreeElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeElement"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="span">The span in the code that this element covers.</param>
        /// <param name="image">The image to show for this element.</param>
        /// <param name="regionId">The identifier of the region that this tree element represents.</param>
        public TreeElement(string text, TextSpan span, BitmapImage image, Guid? regionId = null)
        {
            this.Text = text;
            this.Span = span;
            this.Image = image;
            this.RegionIdentifier = regionId;
        }

        /// <summary>
        /// The text to be shown for this element.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// The text span in the code that this element covers.
        /// </summary>
        public TextSpan Span { get; }

        /// <summary>
        /// The image to show before this element.
        /// </summary>
        public BitmapImage Image { get; }

        /// <summary>
        /// The identifier of the region that this tree element represents. Is <c>null</c> if the tree element
        /// does not represent a region.
        /// </summary>
        public Guid? RegionIdentifier { get; }

        /// <summary>
        /// The color to show text in.
        /// </summary>
        public string TextColor { get; set; } = "#000000";

        /// <summary>
        /// The color to show the background in.
        /// </summary>
        public string BackgroundColor { get; set; } = "#FFFFFF";

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Text;
        }
    }
}