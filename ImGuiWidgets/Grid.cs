namespace ktsu.ImGuiWidgets;

using System.Numerics;
using ImGuiNET;

/// <summary>
/// Provides custom ImGui widgets.
/// </summary>
public static partial class ImGuiWidgets
{
	/// <summary>
	/// Gets or sets a value indicating whether to enable grid debug drawing.
	/// </summary>
	public static bool EnableGridDebugDraw { get; set; }

	/// <summary>
	/// Specifies the order in which grid items are displayed.
	/// </summary>
	/// <remarks>
	/// <see cref="RowMajor"/> displays items left to right before moving to the next row.
	/// <see cref="ColumnMajor"/> displays items top to bottom before moving to the next column.
	/// </remarks>
	public enum GridOrder
	{
		/// <summary>
		/// Items are displayed in order left to right before dropping to the next row.
		/// Recommended for when displaying grids of icons.
		/// Example:
		/// [ [1] [2] [3] ]
		/// [ [4] [5] [6] ]
		/// OR
		/// [ [1] [2] [3] [4] [5] ]
		/// [ [6]                 ]
		/// </summary>
		RowMajor,
		/// <summary>
		/// Items are displayed top to bottom before moving to the next column.
		/// Recommended when displaying tables of data.
		/// Note: This will never display uneven rows that have more than 1 item
		/// missing relative to them.
		/// Example:
		/// [ [1] [3] [5] ]
		/// [ [2] [4] [6] ]
		/// OR
		/// [ [1] [3] [5] ]
		/// [ [2] [4]     ]
		/// NEVER
		/// [ [1] [3] [5] ]
		/// [ [2]         ]
		/// </summary>
		ColumnMajor,
	}

	/// <summary>
	/// Specifies how grid items are laid out within the row.
	/// </summary>
	/// <remarks>
	/// <see cref="Left"/>Items will start from the left edge of the grid
	/// <see cref="Center"/>Items will be displayed evenly spaced out and centered on a per row basis
	/// </remarks>
	public enum GridRowAlignment
	{
		/// <summary>
		/// Each row will start from the left edge of the grid
		/// Example:
		/// [ [1] [2] [3] ]
		/// [ [4] ]
		/// </summary>
		Left,
		/// <summary>
		/// Items will be displayed evenly spaced out and centered on a per row basis
		/// As this is per row, the rows will likely not appear aligned unless each
		/// row contains the exact same number of items with the exact same size
		/// Example:
		/// [ [1] [2] [3] ]
		/// [   [4] [5]   ]
		/// </summary>
		Center
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
	public static void Grid<T>(IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, GridOrder gridOrder)
	{
		ArgumentNullException.ThrowIfNull(items);
		ArgumentNullException.ThrowIfNull(measureDelegate);
		ArgumentNullException.ThrowIfNull(drawDelegate);

		GridImpl.Show(items, measureDelegate, drawDelegate, gridOrder, GridRowAlignment.Left);
	}

	/// <summary>
	/// Renders a grid with the specified items and delegates.
	/// </summary>
	/// <typeparam name="T">The type of the items.</typeparam>
	/// <param name="items">The items to be displayed in the grid.</param>
	/// <param name="measureDelegate">The delegate to measure the size of each item.</param>
	/// <param name="drawDelegate">The delegate to draw each item.</param>
	/// <param name="gridOrder">What ordering should grid items use</param>
	/// <param name="gridAlignment">What alignment should grid items use</param>
	public static void Grid<T>(IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, GridOrder gridOrder, GridRowAlignment gridAlignment)
	{
		ArgumentNullException.ThrowIfNull(items);
		ArgumentNullException.ThrowIfNull(measureDelegate);
		ArgumentNullException.ThrowIfNull(drawDelegate);

		GridImpl.Show(items, measureDelegate, drawDelegate, gridOrder, gridAlignment);
	}

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
		/// <param name="gridAlignment">What alignment should grid items use</param>
		public static void Show<T>(IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, GridOrder gridOrder, GridRowAlignment gridAlignment)
		{
			var itemSpacing = ImGui.GetStyle().ItemSpacing;
			var itemList = items.ToArray();
			var itemDimensions = itemList.Select(i => measureDelegate(i)).ToArray();
			var itemDimensionsWithSpacing = itemList.Select(i => measureDelegate(i) + itemSpacing).ToArray();
			var contentRegionAvailable = ImGui.GetContentRegionAvail();
			int numColumns = 1;

			List<float> previousColumnWidths = [];
			List<float> columnWidths = [];
			float previousTotalContentWidth = 0f;
			float totalContentWidth = 0f;
			while (numColumns <= itemList.Length)
			{
				columnWidths = new List<float>(new float[numColumns]);
				int numRowsForColumns = (int)Math.Ceiling((float)itemList.Length / numColumns);

				for (int i = 0; i < itemList.Length; i++)
				{
					int column = i % numColumns;
					int row = i / numColumns;

					int itemIndex = gridOrder switch
					{
						GridOrder.RowMajor => i,
						GridOrder.ColumnMajor => (column * numRowsForColumns) + row,
						_ => throw new NotImplementedException()
					};

					var thisItemSizeWithSpacing = itemDimensionsWithSpacing[itemIndex];
					columnWidths[column] = Math.Max(columnWidths[column], thisItemSizeWithSpacing.X);
				}

				totalContentWidth = columnWidths.Sum();
				if (totalContentWidth > contentRegionAvailable.X)
				{
					if (numColumns > 1)
					{
						numColumns--;
						totalContentWidth = previousTotalContentWidth;
						columnWidths = previousColumnWidths;
					}
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
				previousTotalContentWidth = totalContentWidth;
				previousColumnWidths = columnWidths;
			}

			int numRows = (int)Math.Ceiling((float)itemList.Length / numColumns);

			// calculate column widths and row heights
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
					var thisItemSizeWithSpacing = itemDimensionsWithSpacing[itemIndex];
					rowHeights[row] = Math.Max(rowHeights[row], thisItemSizeWithSpacing.Y);
				}
			}

			if (gridAlignment == GridRowAlignment.Center)
			{
				float extraSpace = contentRegionAvailable.X - totalContentWidth;
				float extraSpacePerColumn = extraSpace / numColumns;
				for (int i = 0; i < numColumns; i++)
				{
					columnWidths[i] += extraSpacePerColumn;
				}
			}

			var marginTopLeftCursor = ImGui.GetCursorScreenPos();
			float gridWidth = contentRegionAvailable.X;
			float gridHeight = rowHeights.Sum(h => h);

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
					if (EnableGridDebugDraw)
					{
						uint borderColor = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
						var drawList = ImGui.GetWindowDrawList();
						drawList.AddRect(itemStartCursor, itemStartCursor + cellSize, ImGui.GetColorU32(borderColor));
					}

					drawDelegate(itemList[itemIndex], cellSize, itemDimensions[itemIndex]);
				}

				var advance = new Vector2(marginTopLeftCursor.X, itemStartCursor.Y + cellSize.Y);
				if (column < numColumns - 1)
				{
					advance = new Vector2(itemStartCursor.X + cellSize.X, itemStartCursor.Y);
				}
				ImGui.SetCursorScreenPos(advance);
			}

			ImGui.SetCursorScreenPos(marginTopLeftCursor + new Vector2(gridWidth, 0f));
			ImGui.Dummy(new Vector2(0, gridHeight));
		}
	}
}
