namespace ktsu.ImGuiWidgets;

using System.Numerics;
using ImGuiNET;

/// <summary>
/// Provides custom ImGui widgets.
/// </summary>
public static partial class ImGuiWidgets
{
	public enum GridOrder
	{
		// Items are displayed in order left to right before dropping to the next row
		// Recommended for when displaying grids of icons
		// [ [1] [2] [3] ]
		// [ [4] [5] [6] ]
		// OR
		// [ [1] [2] [3] [4] [5] ]
		// [ [6]                 ]
		RowMajor,
		// Items are displayed top to bottom before moving to the next column
		// Recommended when displaying tables of data
		// Note: This will never display uneven rows that have more than 1 item
		// missing relative to them
		// [ [1] [3] [5] ]
		// [ [2] [4] [6] ]
		// OR
		// [ [1] [3] [5] ]
		// [ [2] [4]     ]
		// NEVER
		// [ [1] [3] [5] ]
		// [ [2]         ]
		ColumnMajor,
	}

	/// <summary>
	/// Delegate to measure the size of a grid cell.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="item">The item to measure.</param>
	/// <returns>The size of the item.</returns>
	public delegate Vector2 MeasureGridCell<T>(T item);

	/// <summary>
	/// Delegate to draw a grid cell.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="item">The item to draw.</param>
	/// <param name="cellSize">The calculated size of the grid cell.</param>
	/// <param name="itemSize">The calculated size of the item.</param>
	public delegate void DrawGridCell<T>(T item, Vector2 cellSize, Vector2 itemSize);

	/// <summary>
	/// Renders a grid with the specified items and delegates.
	/// </summary>
	/// <typeparam name="T">The type of the items.</typeparam>
	/// <param name="items">The items to be displayed in the grid.</param>
	/// <param name="measureDelegate">The delegate to measure the size of each item.</param>
	/// <param name="drawDelegate">The delegate to draw each item.</param>
	/// <param name="gridOrder">What ordering should grid items use</param>
	public static void Grid<T>(IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, GridOrder gridOrder) =>
		GridImpl.Show(items, measureDelegate, drawDelegate, gridOrder);

	/// <summary>
	/// Contains the implementation details for rendering grids.
	/// </summary>
	internal static class GridImpl
	{
		/// <summary>
		/// Shows the grid with the specified items and delegates.
		/// </summary>
		/// <typeparam name="T">The type of the items.</typeparam>
		/// <param name="items">The items to be displayed in the grid.</param>
		/// <param name="measureDelegate">The delegate to measure the size of each item.</param>
		/// <param name="drawDelegate">The delegate to draw each item.</param>
		/// <param name="gridOrder">What ordering should grid items use</param>
		public static void Show<T>(IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, GridOrder gridOrder)
		{
			var itemSpacing = ImGui.GetStyle().ItemSpacing;
			var itemList = items.ToArray();
			var itemDimensions = items.Select(i => measureDelegate(i) + itemSpacing).ToArray();
			var contentRegionAvailable = ImGui.GetContentRegionAvail();
			int numColumns = 1;

			while (numColumns <= itemList.Length)
			{
				int numRowsForColumns = (int)Math.Ceiling((float)itemList.Length / numColumns);

				float maxRowWidth = 0f;

				switch (gridOrder)
				{
					case GridOrder.RowMajor:
						float rowWidth = 0f;

						for (int i = 0; i < itemList.Length; i++)
						{
							if (i % numColumns == 0)
							{
								rowWidth = 0f;
							}

							rowWidth += itemDimensions[i].X + itemSpacing.X;
							maxRowWidth = Math.Max(maxRowWidth, rowWidth);
						}
						break;

					case GridOrder.ColumnMajor:
						for (int i = 0; i < numColumns; i++)
						{
							int colOffset = i * numRowsForColumns;
							var colItems = itemDimensions.Skip(colOffset).Take(numRowsForColumns);
							if (colItems.Any())
							{
								maxRowWidth += colItems.Max(item => item.X) + itemSpacing.X;
							}
						}
						break;

					default:
						throw new NotImplementedException($"GridOrder '{gridOrder}' not implemented in ImGuiWidgets.Grid.Show");
				}

				if (maxRowWidth > contentRegionAvailable.X)
				{
					numColumns--;
					break;
				}
				// Once we have iterated all items without exceeding the contentRegionAvailable.X we
				// want to break without incrementing the number of columns because the content will fit
				else if (numColumns == itemList.Length)
				{
					break;
				}
				// ColumnMajor grids are not allowed to have any rows with 2 or more unused cells. If we tried
				// to draw such a case we would end up displaying items that were meant for the end cells on a
				// different row which would then cause the grid to be misaligned.
				// [1] [2] [3]  => [1] [3] [X]
				// [4]			   [2] [4]
				// Don't check for uneven columns if there is only 1 column as it isn't possible for them
				// to be uneven (an simplifies the checking logic)
				else if (gridOrder == GridOrder.ColumnMajor && numColumns != 1)
				{
					int maxItemsPerRow = numColumns;
					int minItemsPerRow = numColumns - 1;

					List<int> itemsPerRow = [];
					int itemCount = 0;
					for (int i = 0; i < itemList.Length; i++)
					{
						itemCount++;
						// The first item will trigger the end of row log (0 % 1 == 0) and we only get in here
						// if numColumns > 1 so we can safely ignore the end of row check in that situation
						bool endOfRow = (i != 0) && (i % numColumns == 0);
						if (endOfRow)
						{
							itemsPerRow.Add(itemCount);
							itemCount = 0;
						}
					}

					if (itemsPerRow.Any(c => c < minItemsPerRow))
					{
						numColumns--;
						break;
					}
				}
				numColumns++;
			}

			if (numColumns < 1)
			{
				numColumns = 1;
			}

			int numRows = (int)Math.Ceiling((float)itemList.Length / numColumns);

			// calculate column widths and row heights
			float[] columnWidths = new float[numColumns];
			float[] rowHeights = new float[numRows];

			for (int i = 0; i < numColumns * numRows; i++)
			{
				int column = i % numColumns;
				int row = i / numColumns;

				int itemIndex = gridOrder switch
				{
					GridOrder.RowMajor => i,
					GridOrder.ColumnMajor => (column * numRows) + row,
					_ => throw new NotImplementedException()
				};

				if (itemIndex < itemList.Length)
				{
					var thisItemSize = itemDimensions[itemIndex];

					columnWidths[column] = Math.Max(columnWidths[column], thisItemSize.X);
					rowHeights[row] = Math.Max(rowHeights[row], thisItemSize.Y);
				}
			}

			float totalContentWidth = columnWidths.Sum();
			float extraSpace = contentRegionAvailable.X - totalContentWidth;
			float extraSpacePerColumn = extraSpace / numColumns;

			for (int i = 0; i < numColumns; i++)
			{
				columnWidths[i] += extraSpacePerColumn;
			}

			var marginTopLeftCursor = ImGui.GetCursorScreenPos();

			int numCells = numColumns * numRows;
			for (int i = 0; i < numCells; i++)
			{
				var itemStartCursor = ImGui.GetCursorScreenPos();
				int column = i % numColumns;
				int row = i / numColumns;

				int itemIndex = gridOrder switch
				{
					GridOrder.RowMajor => i,
					GridOrder.ColumnMajor => (column * numRows) + row,
					_ => throw new NotImplementedException()
				};

				var cellSize = new Vector2(columnWidths[column], rowHeights[row]);

				if (itemIndex < itemList.Length)
				{
					drawDelegate(itemList[itemIndex], cellSize, itemDimensions[itemIndex]);
				}

				var advance = new Vector2(marginTopLeftCursor.X, itemStartCursor.Y + cellSize.Y);
				if (column < numColumns - 1)
				{
					advance = new Vector2(itemStartCursor.X + cellSize.X, itemStartCursor.Y);
				}
				ImGui.SetCursorScreenPos(advance);
			}
		}
	}
}
