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
    using System.Linq;
    using System.Windows.Controls;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Contains methods for building up the syntax tree of a CSharp document.
    /// </summary>
    public static class Builder
    {
        #region Top level

        /// <summary>
        /// Processes a namespace declaration. The namespace contains all other nodes, and is the root of the visible items
        /// in the tree. Also detects regions inside the element.
        /// </summary>
        public static void ProcessNode(NamespaceDeclarationSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    node.Name.ToString(),
                    node.Span,
                    TreeImages.GetImage(ElementType.Namespace)),
                IsExpanded = true
            };

            HandleChildrenWithRegions(node, item.Items);

            AddOrInsert(itemCollection, item, index);
        }

        /// <summary>
        /// Processes a compulation unit, this is the root of an entire document.  Also detects regions inside the element.
        /// </summary>
        public static void ProcessNode(CompilationUnitSyntax node, ItemCollection itemCollection, int? index = null)
        {
            HandleChildrenWithRegions(node, itemCollection);
        }

        #endregion Top level

        #region Second level

        /// <summary>
        /// Processes a class declaration. Also detects regions inside the element.
        /// </summary>
        public static void ProcessNode(ClassDeclarationSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    node.Identifier.ToString(),
                    node.Span,
                    TreeImages.GetImage(ElementType.Class, GetModifiers(node))),
                IsExpanded = true
            };

            HandleChildrenWithRegions(node, item.Items);

            AddOrInsert(itemCollection, item, index);
        }

        /// <summary>
        /// Processes using directive. It is ignored, as they are not interesting to the overall overview.
        /// </summary>
        public static void ProcessNode(UsingDirectiveSyntax node, ItemCollection itemCollection, int? index = null)
        {
            // Using directives are ignored, they are not interesting to the overall overview.
        }

        /// <summary>
        /// Processes an enum declaration. Also detects regions inside the element.
        /// </summary>
        public static void ProcessNode(EnumDeclarationSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    node.Identifier.ToString(),
                    node.Span,
                    TreeImages.GetImage(ElementType.Enum, GetModifiers(node))),
                IsExpanded = true
            };

            HandleChildrenWithRegions(node, item.Items);

            AddOrInsert(itemCollection, item, index);
        }

        /// <summary>
        /// Processes a struct declaration. Also detects regions inside the element.
        /// </summary>
        public static void ProcessNode(StructDeclarationSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    node.Identifier.ToString(),
                    node.Span,
                    TreeImages.GetImage(ElementType.Struct, GetModifiers(node))),
                IsExpanded = true
            };

            HandleChildrenWithRegions(node, item.Items);

            AddOrInsert(itemCollection, item, index);
        }

        /// <summary>
        /// Processes an interface declaration. Also detects regions inside the element.
        /// </summary>
        public static void ProcessNode(InterfaceDeclarationSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    node.Identifier.ToString(),
                    node.Span,
                    TreeImages.GetImage(ElementType.Interface, GetModifiers(node))),
                IsExpanded = true
            };

            HandleChildrenWithRegions(node, item.Items);

            AddOrInsert(itemCollection, item, index);
        }

        #endregion Second level

        #region Third level


        /// <summary>
        /// Processes a field declaration.
        /// </summary>
        public static void ProcessNode(FieldDeclarationSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    $"{string.Join(", ", node.Declaration.Variables.Select(v => $"{v.Identifier}"))}: {node.Declaration.Type}",
                    node.Span,
                    TreeImages.GetImage(ElementType.Field, GetModifiers(node)))
            };

            AddOrInsert(itemCollection, item, index);
        }


        /// <summary>
        /// Processes a property declaration.
        /// </summary>
        public static void ProcessNode(PropertyDeclarationSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    $"{node.Identifier.ToString()}: {node.Type}",
                    node.Span,
                    TreeImages.GetImage(ElementType.Property, GetModifiers(node)))
            };

            AddOrInsert(itemCollection, item, index);
        }


        /// <summary>
        /// Processes a method declaration.
        /// </summary>
        public static void ProcessNode(MethodDeclarationSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    $"{node.Identifier}({string.Join(", ", node.ParameterList.Parameters.Select(p => p.Type))}): {node.ReturnType}",
                    node.Span,
                    TreeImages.GetImage(ElementType.Method, GetModifiers(node)))
            };

            AddOrInsert(itemCollection, item, index);
        }


        /// <summary>
        /// Processes an enum member declaration.
        /// </summary>
        public static void ProcessNode(EnumMemberDeclarationSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    string.Join(", ", node.Identifier.ToString()),
                    node.Span,
                    TreeImages.GetImage(ElementType.EnumValue))
            };

            AddOrInsert(itemCollection, item, index);
        }

        /// <summary>
        /// Processes a constructor declaration.
        /// </summary>
        public static void ProcessNode(ConstructorDeclarationSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    $"{node.Identifier}({string.Join(", ", node.ParameterList.Parameters.Select(p => p.Type))})",
                    node.Span,
                    TreeImages.GetImage(ElementType.Constructor, GetModifiers(node)))
            };

            AddOrInsert(itemCollection, item, index);
        }


        /// <summary>
        /// Processes an operator declaration.
        /// </summary>
        public static void ProcessNode(OperatorDeclarationSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    $"{node.ReturnType} {node.OperatorToken}({string.Join(", ", node.ParameterList.Parameters.Select(p => p.Type))})",
                    node.Span,
                    TreeImages.GetImage(ElementType.Operator, GetModifiers(node)))
            };

            AddOrInsert(itemCollection, item, index);
        }

        /// <summary>
        /// Processes an operator declaration.
        /// </summary>
        public static void ProcessNode(ConversionOperatorDeclarationSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    $"{node.Type}({string.Join(", ", node.ParameterList.Parameters.Select(p => p.Type))})",
                    node.Span,
                    TreeImages.GetImage(ElementType.Operator, GetModifiers(node)))
            };

            AddOrInsert(itemCollection, item, index);
        }


        #endregion Third level

        #region Extra elements

        /// <summary>
        /// Processes a base list. It is ignored, as they are not interesting to the overall overview.
        /// </summary>
        public static void ProcessNode(IncompleteMemberSyntax node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    node.Type.ToString(),
                    node.Span,
                    TreeImages.GetImage(ElementType.Unknown, GetModifiers(node)))
            };

            AddOrInsert(itemCollection, item, index);
        }

        /// <summary>
        /// Processes a base list. It is ignored, as they are not interesting to the overall overview.
        /// </summary>
        public static void ProcessNode(BaseListSyntax node, ItemCollection itemCollection, int? index = null)
        {
        }

        /// <summary>
        /// Processes an attribute list. It is ignored, as they are not interesting to the overall overview.
        /// </summary>
        public static void ProcessNode(AttributeListSyntax node, ItemCollection itemCollection, int? index = null)
        {
        }

        /// <summary>
        /// Processes a qualified name node. It is ignored, as they are accessed from their parents when appropriate.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="itemCollection"></param>
        public static void ProcessNode(QualifiedNameSyntax node, ItemCollection itemCollection, int? index = null)
        {
        }

        /// <summary>
        /// Processes a identifier name node. It is ignored, as they are accessed from their parents when appropriate.
        /// </summary>
        public static void ProcessNode(IdentifierNameSyntax node, ItemCollection itemCollection, int? index = null)
        {
        }

        #endregion Extra elements

        /// <summary>
        /// Processes any other type of syntax node. This means it is not understood, and the type is shown with
        /// a question mark.
        /// </summary>
        public static void ProcessNode(SyntaxNode node, ItemCollection itemCollection, int? index = null)
        {
            var item = new TreeViewItem
            {
                Header = new TreeElement(
                    $"{node.GetType()}",
                    node.Span,
                    TreeImages.GetImage(ElementType.Unknown))
            };

            AddOrInsert(itemCollection, item, index);
        }

        /// <summary>
        /// Handles nesting child nodes within regions. While regions are allowed in multiple levels,
        /// we only use this on second level nodes.
        /// </summary>
        private static void HandleChildrenWithRegions(SyntaxNode node, ItemCollection itemCollection)
        {
            // Determine regions, so we know whether to put nodes inside the regions, or not.
            var startRegionDirectives = node.DescendantNodes(descendIntoTrivia: true)
                .OfType<RegionDirectiveTriviaSyntax>().Cast<DirectiveTriviaSyntax>()
                .Where(s => node.Span.Contains(s.Span));
            var endRegionDirectives = node.DescendantNodes(descendIntoTrivia: true).OfType<EndRegionDirectiveTriviaSyntax>()
                .Where(s => node.Span.Contains(s.Span));
            var regionDirectives = startRegionDirectives.Union(endRegionDirectives).OrderBy(x => x.FullSpan.Start);

            var regionTree = new RegionTree(node.Span, itemCollection).Create(regionDirectives.GetEnumerator());

            // Insert all nodes in the region tree, this will update itemCollection.
            // Note that this uses an immutable structure to easily recurse over the region structure, while mutating
            // the mutable itemCollection as the intended side-effect. Hence the return value is not used.
            node.ChildNodes().Aggregate(regionTree, (current, child) => current.InsertNode(child));
        }

        /// <summary>
        /// Attempts to retrieve the access level of a node. Takes any <see cref="SyntaxNode"/>, but will only work if the type contains
        /// a property named Modifiers which returns a <see cref="SyntaxTokenList"/>.
        /// </summary>
        /// <param name="node">The node to get the access of.</param>
        /// <returns>The node <see cref="ElementModifier"/>.</returns>
        private static ElementModifier GetModifiers(SyntaxNode node)
        {
            var modifiers = GetNodeModifiers(node);
            var result = ElementModifier.None;

            // Access level. Private is inferred from no other access keyword, no need to check the modifier.
            if (modifiers.Any(m => m.Text == "public")) result |= ElementModifier.Public;
            if (modifiers.Any(m => m.Text == "internal")) result |= ElementModifier.Internal;
            if (modifiers.Any(m => m.Text == "protected")) result |= ElementModifier.Protected;
            if ((result & (ElementModifier.Public | ElementModifier.Protected | ElementModifier.Internal)) == 0)
                result |= ElementModifier.Private;

            if (modifiers.Any(m => m.Text == "abstract")) result |= ElementModifier.Abstract;
            if (modifiers.Any(m => m.Text == "async")) result |= ElementModifier.Async;
            if (modifiers.Any(m => m.Text == "const")) result |= ElementModifier.Const;
            if (modifiers.Any(m => m.Text == "event")) result |= ElementModifier.Event;
            if (modifiers.Any(m => m.Text == "extern")) result |= ElementModifier.Extern;
            if (modifiers.Any(m => m.Text == "override")) result |= ElementModifier.Override;
            if (modifiers.Any(m => m.Text == "new")) result |= ElementModifier.New;
            if (modifiers.Any(m => m.Text == "partial")) result |= ElementModifier.Partial;
            if (modifiers.Any(m => m.Text == "readonly")) result |= ElementModifier.Readonly;
            if (modifiers.Any(m => m.Text == "sealed")) result |= ElementModifier.Sealed;
            if (modifiers.Any(m => m.Text == "static")) result |= ElementModifier.Static;
            if (modifiers.Any(m => m.Text == "virtual")) result |= ElementModifier.Virtual;
            if (modifiers.Any(m => m.Text == "volatile")) result |= ElementModifier.Volatile;

            return result;
        }

        /// <summary>
        /// Attempts to retrieve the modifiers list of a <see cref="SyntaxNode"/>. Will only work if this propery exists.
        /// </summary>
        /// <param name="node">The node to get the modifiers of.</param>
        /// <returns>The modifiers list of the node.</returns>
        private static SyntaxTokenList GetNodeModifiers(SyntaxNode node)
        {
            if (!node.GetType().GetProperties().Any(p => p.Name == "Modifiers" && p.PropertyType == typeof(SyntaxTokenList)))
                throw new ArgumentException("The provided node does not have a modifiers list.");

            return (SyntaxTokenList)node.GetType().GetProperty("Modifiers").GetValue(node);
        }

        /// <summary>
        /// Adds or inserts an item in an itemcollection.
        /// </summary>
        /// <param name="itemCollection"></param>
        /// <param name="item"></param>
        /// <param name="index"></param>
        private static void AddOrInsert(ItemCollection itemCollection, TreeViewItem item, int? index = null)
        {
            if (index == null)
            {
                itemCollection.Add(item);
            }
            else
            {
                itemCollection.Insert(index.Value, item);
            }
        }
    }
}
