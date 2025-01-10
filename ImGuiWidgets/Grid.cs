namespace ktsu.ImGuiWidgets;

using System.Collections.ObjectModel;
using System.Numerics;
using ImGuiNET;
using ktsu.Extensions;

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
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="items"></param>
	/// <param name="measureDelegate"></param>
	/// <param name="drawDelegate"></param>
	/// <param name="gridOrder"></param>
	/// <param name="size"></param>
	/// <exception cref="NotImplementedException"></exception>
	public static void Grid2<T>(IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, GridOrder gridOrder, Vector2 size)
	{
		ArgumentNullException.ThrowIfNull(items);
		ArgumentNullException.ThrowIfNull(measureDelegate);
		ArgumentNullException.ThrowIfNull(drawDelegate);

		switch (gridOrder)
		{
			case GridOrder.RowMajor:
				GridImpl.ShowRowMajor(items, measureDelegate, drawDelegate, size);
				break;
			case GridOrder.ColumnMajor:
				GridImpl.ShowColumnMajor(items, measureDelegate, drawDelegate, size);
				break;
			default:
				throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Contains the implementation details for rendering grids.
	/// </summary>
	internal static class GridImpl
	{
		internal class CellData
		{
			internal int ItemIndex { get; set; }
			internal int RowIndex { get; set; }
			internal int ColumnIndex { get; set; }
		}

		internal static CellData CalculateCellData(int itemIndex, int columnCount, int rowCount, GridOrder gridOrder)
		{
			var cellData = new CellData();
			switch (gridOrder)
			{
				case GridOrder.RowMajor:
					cellData.ColumnIndex = itemIndex % columnCount;
					cellData.RowIndex = itemIndex / columnCount;
					cellData.ItemIndex = itemIndex;
					break;

				case GridOrder.ColumnMajor:
					cellData.ColumnIndex = itemIndex / rowCount;
					cellData.RowIndex = itemIndex % rowCount;
					cellData.ItemIndex = (cellData.ColumnIndex * rowCount) + cellData.RowIndex;
					break;

				default:
					throw new NotImplementedException();
			}
			return cellData;
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="measureDelegate"></param>
		/// <param name="drawDelegate"></param>
		/// <param name="size"></param>
		public static void ShowRowMajor<T>(IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, Vector2 size)
		{
			var itemSpacing = ImGui.GetStyle().ItemSpacing;
			var itemList = items.ToArray();
			var itemDimensions = itemList.Select(i => measureDelegate(i)).ToArray();
			var itemDimensionsWithSpacing = itemDimensions.Select(d => d + itemSpacing).ToArray();
			float gridWidth = size.X;
			int numColumns = (int)size.X;
			numColumns = 1;

			Collection<float> columnWidths = [];
			Collection<float> previousColumnWidths = [];
			Collection<float> rowHeights = [];
			Collection<float> previousRowHeights = [];

			float previousTotalContentWidth = 0f;
			float totalContentWidth = 0f;
			while (numColumns <= itemList.Length)
			{
				int numRowsForColumns = (int)Math.Ceiling((float)itemList.Length / numColumns);
				columnWidths = new float[numColumns].ToCollection();
				rowHeights = new float[numRowsForColumns].ToCollection();

				for (int i = 0; i < itemList.Length; i++)
				{
					var cellData = new CellData
					{
						ColumnIndex = i % numColumns,
						RowIndex = i / numColumns,
						ItemIndex = i
					};

					if (cellData.ItemIndex < itemList.Length)
					{
						var thisItemSizeWithSpacing = itemDimensionsWithSpacing[cellData.ItemIndex];

						int column = cellData.ColumnIndex;
						int row = cellData.RowIndex;
						columnWidths[column] = Math.Max(columnWidths[column], thisItemSizeWithSpacing.X);
						rowHeights[row] = Math.Max(rowHeights[row], thisItemSizeWithSpacing.Y);
					}
				}

				totalContentWidth = columnWidths.Sum();
				if (totalContentWidth > gridWidth)
				{
					if (numColumns > 1)
					{
						numColumns--;
						totalContentWidth = previousTotalContentWidth;
						columnWidths = previousColumnWidths;
						rowHeights = previousRowHeights;
					}
					break;
				}
				// Once we have iterated all items without exceeding the contentRegionAvailable.X we
				// want to break without incrementing the number of columns because the content will fit
				else if (numColumns == itemList.Length)
				{
					break;
				}

				numColumns++;
				previousTotalContentWidth = totalContentWidth;
				previousColumnWidths = columnWidths;
				previousRowHeights = rowHeights;
			}

			if (ImGui.BeginChild("grid", size, ImGuiChildFlags.Borders))
			{
				var marginTopLeftCursor = ImGui.GetCursorScreenPos();
				float gridHeight = rowHeights.Sum(h => h);

				int lastItemIndex = itemList.Length - 1;
				for (int i = 0; i < itemList.Length; i++)
				{
					var itemStartCursor = ImGui.GetCursorScreenPos();

					var cellData = new CellData
					{
						ColumnIndex = i % numColumns,
						RowIndex = i / numColumns,
						ItemIndex = i
					};

					int row = cellData.RowIndex;
					int column = cellData.ColumnIndex;
					int itemIndex = cellData.ItemIndex;
					var cellSize = new Vector2(columnWidths[column], rowHeights[row]);

					if (EnableGridDebugDraw)
					{
						uint borderColor = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
						var drawList = ImGui.GetWindowDrawList();
						drawList.AddRect(itemStartCursor, itemStartCursor + cellSize, ImGui.GetColorU32(borderColor));
					}

					drawDelegate(itemList[itemIndex], cellSize, itemDimensions[itemIndex]);

					if (itemIndex != lastItemIndex)
					{
						bool sameRow = column < numColumns - 1;
						var newCursorPos = sameRow
							? new Vector2(itemStartCursor.X + cellSize.X, itemStartCursor.Y)
							: new Vector2(marginTopLeftCursor.X, itemStartCursor.Y + cellSize.Y);

						ImGui.SetCursorScreenPos(newCursorPos);
					}
				}
			}
			ImGui.EndChild();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="measureDelegate"></param>
		/// <param name="drawDelegate"></param>
		/// <param name="size"></param>
		public static void ShowColumnMajor<T>(IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, Vector2 size)
		{
			int maxHeight = (int)size.Y;

			var itemSpacing = ImGui.GetStyle().ItemSpacing;
			var itemList = items.ToArray();
			var itemDimensions = itemList.Select(i => measureDelegate(i)).ToArray();
			var itemDimensionsWithSpacing = itemDimensions.Select(d => d + itemSpacing).ToArray();
			float rowHeight = itemDimensionsWithSpacing.Max(d => d.Y);
			var contentRegionAvailable = ImGui.GetContentRegionAvail();
			int numColumns = 1;

			int itemsToCount = itemDimensionsWithSpacing.Length;
			var remainingItemList = itemDimensionsWithSpacing;
			Collection<float> columnWidths = [];
			while (itemsToCount > 0)
			{
				int itemCountInColumn = (int)(maxHeight / rowHeight);
				var itemsInColumn = remainingItemList.Take(itemCountInColumn).ToArray();
				columnWidths.Add(itemsInColumn.Max(d => d.X));
				itemsToCount -= itemCountInColumn;
				remainingItemList = remainingItemList.Skip(itemCountInColumn).ToArray();
				if (itemsToCount > 0)
				{
					numColumns++;
				}
			}

			if (ImGui.BeginChild("grid", size, ImGuiChildFlags.Borders, ImGuiWindowFlags.HorizontalScrollbar))
			{
				var gridTopLeft = ImGui.GetCursorScreenPos();
				float heightUsed = 0f;
				int columnIndex = 0;
				int lastItemIndex = itemList.Length - 1;
				for (int itemIndex = 0; itemIndex < itemList.Length; itemIndex++)
				{
					var cellSize = new Vector2(columnWidths[columnIndex], rowHeight);
					var itemTopLeftCursor = ImGui.GetCursorScreenPos();

					if (EnableGridDebugDraw)
					{
						uint borderColor = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
						var drawList = ImGui.GetWindowDrawList();
						drawList.AddRect(itemTopLeftCursor, itemTopLeftCursor + cellSize, ImGui.GetColorU32(borderColor));
					}
					heightUsed += cellSize.Y;
					drawDelegate(itemList[itemIndex], cellSize, itemDimensions[itemIndex]);
					if (itemIndex != lastItemIndex)
					{
						if (heightUsed + rowHeight > maxHeight)
						{
							columnIndex++;
							heightUsed = 0f;
							ImGui.SetCursorScreenPos(new Vector2(itemTopLeftCursor.X + cellSize.X, gridTopLeft.Y));
						}
						else
						{
							ImGui.SetCursorScreenPos(itemTopLeftCursor + new Vector2(0f, cellSize.Y));
						}
					}
				}
			}
			ImGui.EndChild();
		}

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
			var itemDimensionsWithSpacing = itemDimensions.Select(d => d + itemSpacing).ToArray();
			var contentRegionAvailable = ImGui.GetContentRegionAvail();
			int numColumns = 1;

			Collection<float> columnWidths = [];
			Collection<float> previousColumnWidths = [];
			Collection<float> rowHeights = [];
			Collection<float> previousRowHeights = [];

			float previousTotalContentWidth = 0f;
			float totalContentWidth = 0f;
			while (numColumns <= itemList.Length)
			{
				int numRowsForColumns = (int)Math.Ceiling((float)itemList.Length / numColumns);
				columnWidths = new float[numColumns].ToCollection();
				rowHeights = new float[numRowsForColumns].ToCollection();

				for (int i = 0; i < itemList.Length; i++)
				{
					var cellData = CalculateCellData(i, numColumns, numRowsForColumns, gridOrder);
					if (cellData.ItemIndex < itemList.Length)
					{
						var thisItemSizeWithSpacing = itemDimensionsWithSpacing[cellData.ItemIndex];

						int column = cellData.ColumnIndex;
						int row = cellData.RowIndex;
						columnWidths[column] = Math.Max(columnWidths[column], thisItemSizeWithSpacing.X);
						rowHeights[row] = Math.Max(rowHeights[row], thisItemSizeWithSpacing.Y);
					}
				}

				totalContentWidth = columnWidths.Sum();
				if (totalContentWidth > contentRegionAvailable.X)
				{
					if (numColumns > 1)
					{
						numColumns--;
						totalContentWidth = previousTotalContentWidth;
						columnWidths = previousColumnWidths;
						rowHeights = previousRowHeights;
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
						if (numColumns > 1)
						{
							numColumns--;
							totalContentWidth = previousTotalContentWidth;
							columnWidths = previousColumnWidths;
							rowHeights = previousRowHeights;
						}
						break;
					}
				}
				numColumns++;
				previousTotalContentWidth = totalContentWidth;
				previousColumnWidths = columnWidths;
				previousRowHeights = rowHeights;
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

			for (int i = 0; i < itemList.Length; i++)
			{
				var itemStartCursor = ImGui.GetCursorScreenPos();

				var cellData = CalculateCellData(i, columnWidths.Count, rowHeights.Count, gridOrder);
				int row = cellData.RowIndex;
				int column = cellData.ColumnIndex;
				int itemIndex = cellData.ItemIndex;
				var cellSize = new Vector2(columnWidths[column], rowHeights[row]);

				if (EnableGridDebugDraw)
				{
					uint borderColor = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
					var drawList = ImGui.GetWindowDrawList();
					drawList.AddRect(itemStartCursor, itemStartCursor + cellSize, ImGui.GetColorU32(borderColor));
				}

				drawDelegate(itemList[itemIndex], cellSize, itemDimensions[itemIndex]);

				var newCursorPos = new Vector2();
				switch (gridOrder)
				{
					case GridOrder.RowMajor:
						bool sameRow = column < numColumns - 1;
						newCursorPos = sameRow
							? new Vector2(itemStartCursor.X + cellSize.X, itemStartCursor.Y)
							: new Vector2(marginTopLeftCursor.X, itemStartCursor.Y + cellSize.Y);
						break;

					case GridOrder.ColumnMajor:
						bool sameColumn = row < rowHeights.Count - 1;
						newCursorPos = sameColumn
							? new Vector2(itemStartCursor.X, itemStartCursor.Y + cellSize.Y)
							: new Vector2(itemStartCursor.X + itemStartCursor.Y + marginTopLeftCursor.Y);
						break;

					default:
						throw new NotImplementedException();
				}

				//var advance = new Vector2(marginTopLeftCursor.X, itemStartCursor.Y + cellSize.Y);
				//if (column < numColumns - 1)
				//{
				//	newCursorPos = new Vector2(itemStartCursor.X + cellSize.X, itemStartCursor.Y);
				//}
				ImGui.SetCursorScreenPos(newCursorPos);
			}

			ImGui.SetCursorScreenPos(marginTopLeftCursor + new Vector2(gridWidth, 0f));
			ImGui.Dummy(new Vector2(0, gridHeight));
		}
	}
}
