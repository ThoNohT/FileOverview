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

namespace ThoNohT.FileOverview
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.CodeAnalysis.CSharp;
    using ThoNohT.FileOverview.TreeBuilder.CSharp;

    /// <summary>
    /// Interaction logic for OverviewWindowControl.
    /// </summary>
    public partial class OverviewWindowControl : UserControl
    {
        private DTE2 service;
        private readonly IServiceProvider serviceProvider;

        private DocumentEvents documentEvents;
        private TextEditorEvents textEditorEvents;
        private WindowEvents windowEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="OverviewWindowControl"/> class.
        /// </summary>
        public OverviewWindowControl(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            this.serviceProvider = serviceProvider;
        }

        private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs args)
        {
            var item = (TreeViewItem)this.OverviewTree.SelectedItem;
            if (item == null) return;
            var element = (TreeElement)item.Header;

            var sel = (TextSelection)this.service.ActiveDocument.Selection;
            try
            {
                sel.MoveToAbsoluteOffset(element.Span.Start + 1);
                //args.Handled = true;
            }
            catch
            {
                // Don't move if it cannot be done.
            }

            this.service.ActiveDocument.ActiveWindow.Activate();
        }

        private void UpdateTree()
        {
            this.OverviewTree.Items.Clear();

            var doc = this.service.ActiveDocument?.Object() as TextDocument;

            if (doc == null) return;
            if (doc.Language != "CSharp") return;

            var text = doc.CreateEditPoint(doc.StartPoint).GetText(doc.EndPoint).Replace("\r\n", "\n");

            var tree = CSharpSyntaxTree.ParseText(text);
            var root = tree.GetRoot();

            Builder.ProcessNode((dynamic)root, this.OverviewTree.Items);
        }

        private void OverviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.service = (DTE2)this.serviceProvider.GetService(typeof(DTE));
            this.documentEvents = this.service.Events.DocumentEvents;
            this.textEditorEvents = this.service.Events.TextEditorEvents;
            this.windowEvents = this.service.Events.WindowEvents;

            this.windowEvents.WindowActivated += (newWindow, oldWindow) => this.UpdateTree();
            this.documentEvents.DocumentOpened += doc => this.UpdateTree();
            this.textEditorEvents.LineChanged += (start, end, idx) => this.UpdateTree();
        }
    }
}