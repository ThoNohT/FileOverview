﻿/*
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
    /// <summary>
    /// Enumeration listing the possible elements that can appear in the tree.
    /// </summary>
    public enum ElementType
    {
        /// <summary>
        /// An unknown element, should be fixed in a future version.
        /// </summary>
        Unknown,

        /// <summary>
        /// A namespace.
        /// </summary>
        Namespace,

        /// <summary>
        /// A class.
        /// </summary>
        Class,

        /// <summary>
        /// An interface
        /// </summary>
        Interface,

        /// <summary>
        /// A struct.
        /// </summary>
        Struct,

        /// <summary>
        /// An enumeration.
        /// </summary>
        Enum,

        /// <summary>
        /// A constructor.
        /// </summary>
        Constructor,

        /// <summary>
        /// A method.
        /// </summary>
        Method,

        /// <summary>
        /// A property.
        /// </summary>
        Property,

        /// <summary>
        /// A field.
        /// </summary>
        Field,

        /// <summary>
        /// An enumeration value.
        /// </summary>
        EnumValue,

        /// <summary>
        /// A region.
        /// </summary>
        Region,

        /// <summary>
        /// An operator overload.
        /// </summary>
        Operator,
    }
}