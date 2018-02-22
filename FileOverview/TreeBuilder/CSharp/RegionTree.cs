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
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Windows.Controls;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// A region tree represents a node that contains regions, and these regions can again contain sub-regions,
    /// and so on. This helper class can build up the tree, and insert nodes in the correct place. Each region tree has
    /// a reference to an ItemCollection, which is updated as the tree is built up and nodes are inserted.
    /// </summary>
    public sealed class RegionTree
    {
        #region Constructors

        public RegionTree(TextSpan containerSpan, ItemCollection itemCollection)
            : this("root", containerSpan.Start, containerSpan.End, ImmutableList<RegionTree>.Empty, true, itemCollection)
        {
        }

        private RegionTree(string name, TextSpan containerSpan, ItemCollection itemCollection)
            : this(
                name,
                containerSpan.Start,
                containerSpan.End,
                ImmutableList<RegionTree>.Empty,
                false,
                itemCollection)
        {
        }

        private RegionTree(
            string name,
            int spanStart,
            int spanEnd,
            ImmutableList<RegionTree> children,
            bool isRoot,
            ItemCollection itemCollection)
        {
            this.Id = Guid.NewGuid();

            this.Name = name;
            this.SpanStart = spanStart;
            this.SpanEnd = spanEnd;
            this.Children = children;
            this.IsRoot = isRoot;
            this.ItemCollection = itemCollection;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The identifier of this region tree.
        /// </summary>
        private Guid Id { get; }

        /// <summary>
        /// Indicates whether this is the root region tree, which is not actually a region, but represents the node
        /// that contains regions.
        /// </summary>
        private bool IsRoot { get; }

        /// <summary>
        /// The name of this region. is "root" for the root region tree.
        /// </summary>
        private string Name { get; }

        /// <summary>
        /// The start of this region tree's span. For regions, this represents the start of the full span of the
        /// region directive.
        /// </summary>
        private int SpanStart { get; }

        /// <summary>
        /// The end of this region tree's span. For regions, this represents the end of the full span of the
        /// endregion directive.
        /// </summary>
        private int SpanEnd { get; }

        /// <summary>
        /// The child regions of this region tree.
        /// </summary>
        private ImmutableList<RegionTree> Children { get; }

        /// <summary>
        /// The item collection in the tree view that contains this region's children.
        /// Due to the nature of the TreeView, this is the only property that is not truely immutable.
        /// </summary>
        private ItemCollection ItemCollection { get; }

        #endregion Properties

        /// <summary>
        /// Builds a region tree from a list of <see cref="RegionDirectiveTriviaSyntax"/> and
        /// <see cref="EndRegionDirectiveTriviaSyntax"/> elements. Builds up the item collections with only regions.
        /// </summary>
        /// <param name="directives">The directives to build the region tree from.</param>
        /// <returns>The built region tree.</returns>
        public RegionTree Create(IEnumerator<DirectiveTriviaSyntax> directives)
        {
            var result = this;

            while (directives.MoveNext())
            {
                if (directives.Current is RegionDirectiveTriviaSyntax start)
                {
                    // A new region is started, so try finding sub-regions inside it.
                    // Note that if there are more start-regions than end-regions, this will result the outermost
                    // region being ended at the same time as the last region that was ended.
                    var item = new TreeViewItem { IsExpanded = true };
                    var child = new RegionTree(start.ToString(), start.FullSpan, item.Items).Create(directives);
                    result = result.AddChild(child);
                    item.Header = new TreeElement(
                        child.Name.Substring("#region ".Length),
                        new TextSpan(child.SpanStart, child.SpanEnd - child.SpanStart),
                        TreeImages.GetImage(ElementType.Region),
                        regionId: child.Id) { TextColor = "#666" };

                    this.ItemCollection.Add(item);
                }
                else if (directives.Current is EndRegionDirectiveTriviaSyntax end)
                {
                    // We return the region currently being built up with its span-end filled in.
                    // Note that an additional endregion syntax will exit this method early, resulting in fewer regions
                    // being returned. But that is okay, since the syntax is not correct anyway.
                    return result.SetSpanEnd(end.FullSpan.End);
                }
            }

            // For the root, don't update the span end, it was set when creating the region.
            // Otherwise, there are end-region directives missing, so end the current region at the end of its
            // child regions (if it has any).
            var lastChild = result.Children.LastOrDefault();
            if (result.IsRoot || lastChild == null)
                return result;
            else
                return result.SetSpanEnd(lastChild.SpanEnd);
        }

        /// <summary>
        /// Inserts a node in the region tree. If the node is contained in a child region, that child region is updated
        /// and this result is returned. If the node contains a child region, that child region is replaced by this
        /// node. Otherwise, the node is inserted in this region's <see cref="ItemCollection"/> at the appropriate
        /// place.
        /// </summary>
        /// <param name="node">The node to insert.</param>
        /// <returns>The region tree with the node inserted.</returns>
        public RegionTree InsertNode(SyntaxNode node)
        {
            // Attempt to insert it in a region that contains the entire node.
            var containingRegion = this.Children
                .Where(r => r.SpanStart < node.SpanStart)
                .SingleOrDefault(r => r.SpanEnd > node.Span.End);

            if (containingRegion != null)
                return this.UpdateChild(containingRegion, containingRegion.InsertNode(node));

            // Attempt to replace all regions that are entirely contained by the node.
            var containedRegions = this.Children
                .Where(r => r.SpanStart > node.SpanStart).Where(r => r.SpanEnd < node.Span.End).ToList();

            if (containedRegions.Any())
            {
                var resultingTree = this;

                // Find the item of the first region in the item collection.
                var regionItem = resultingTree.ItemCollection.Cast<TreeViewItem>()
                    .Single(i => ((TreeElement)i.Header).RegionIdentifier == containedRegions.First().Id);
                var index = this.ItemCollection.IndexOf(regionItem);

                foreach (var containedRegion in containedRegions)
                {
                    // Remove the region item.
                    regionItem = resultingTree.ItemCollection.Cast<TreeViewItem>()
                        .Single(i => ((TreeElement)i.Header).RegionIdentifier == containedRegion.Id);
                    resultingTree.ItemCollection.Remove(regionItem);

                    resultingTree = resultingTree.RemoveChild(containedRegion);
                }

                Builder.ProcessNode((dynamic)node, resultingTree.ItemCollection, index);
                return resultingTree;
            }

            // Attempt to find the first region that starts directly behind it.
            var firstRegionAfter = this.Children.FirstOrDefault(r => r.SpanStart > node.Span.End);

            if (firstRegionAfter != null)
            {
                // Find the item in the item collection.
                var regionItem = this.ItemCollection.Cast<TreeViewItem>()
                    .Single(i => ((TreeElement)i.Header).RegionIdentifier == firstRegionAfter.Id);
                var index = this.ItemCollection.IndexOf(regionItem);

                // Insert the node at that position.
                Builder.ProcessNode((dynamic)node, this.ItemCollection, index);
            }
            else
            {
                // If no region was not found after the node, simply insert the item at the end.
                Builder.ProcessNode((dynamic)node, this.ItemCollection);
            }

            return this;
        }

        #region Helpers

        /// <summary>
        /// Returns a new <see cref="RegionTree"/> with the specified child removed.
        /// </summary>
        /// <param name="childToRemove">The child to remove.</param>
        /// <returns>The updated <see cref="RegionTree"/>.</returns>
        private RegionTree RemoveChild(RegionTree childToRemove)
        {
            return new RegionTree(
                this.Name,
                this.SpanStart,
                this.SpanEnd,
                this.Children.Remove(childToRemove),
                this.IsRoot,
                this.ItemCollection);
        }

        /// <summary>
        /// Returns a new <see cref="RegionTree"/> with <paramref name="oldChild"/> replaced with
        /// <paramref name="newChild"/>.
        /// </summary>
        /// <param name="oldChild">The child to be replaced.</param>
        /// <param name="newChild">The replacemend child.</param>
        /// <returns>The updated <see cref="RegionTree"/>.</returns>
        private RegionTree UpdateChild(RegionTree oldChild, RegionTree newChild)
        {
            return new RegionTree(
                this.Name,
                this.SpanStart,
                this.SpanEnd,
                this.Children.Replace(oldChild, newChild),
                this.IsRoot,
                this.ItemCollection);
        }

        /// <summary>
        /// Returns a new <see cref="RegionTree"/> with the specified child added at the end.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>The updated <see cref="RegionTree"/>.</returns>
        private RegionTree AddChild(RegionTree child)
        {
            return new RegionTree(
                this.Name,
                this.SpanStart,
                this.SpanEnd,
                this.Children.Add(child),
                this.IsRoot,
                this.ItemCollection);
        }

        /// <summary>
        /// Returns a new <see cref="RegionTree"/> with <see cref="SpanEnd"/> set to the new value.
        /// </summary>
        /// <param name="spanEnd">The new value for <see cref="SpanEnd"/>.</param>
        /// <returns>The updated <see cref="RegionTree"/>.</returns>
        private RegionTree SetSpanEnd(int spanEnd)
        {
            return new RegionTree(this.Name, this.SpanStart, spanEnd, this.Children, this.IsRoot, this.ItemCollection);
        }

        #endregion Helpers
    }
}
