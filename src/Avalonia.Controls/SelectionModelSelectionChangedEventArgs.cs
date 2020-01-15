// This source file is adapted from the WinUI project.
// (https://github.com/microsoft/microsoft-ui-xaml)
//
// Licensed to The Avalonia Project under MIT License, courtesy of The .NET Foundation.

using System;
using System.Collections.Generic;

namespace Avalonia.Controls
{
    internal class SelectionModelSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="removedItems">The items removed from the selection.</param>
        /// <param name="addedItems">The items added to the selection.</param>
        public SelectionModelSelectionChangedEventArgs(
            IReadOnlyList<object> removedItems,
            IReadOnlyList<object> addedItems)
        {
            removedItems = removedItems ?? throw new ArgumentNullException(nameof(removedItems));
            addedItems = addedItems ?? throw new ArgumentNullException(nameof(addedItems));

            AddedItems = addedItems;
            RemovedItems = removedItems;
        }

        /// <summary>
        /// Gets the items that were added to the selection.
        /// </summary>
        public IReadOnlyList<object> AddedItems { get; }

        /// <summary>
        /// Gets the items that were removed from the selection.
        /// </summary>
        public IReadOnlyList<object> RemovedItems { get; }
    }
}
