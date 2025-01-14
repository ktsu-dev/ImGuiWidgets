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
		/// Example:
		/// [ [1] [4] ]
		/// [ [2] [5] ]
		/// [ [3] [6] ]
		/// OR
		/// [ [1] [5] ]
		/// [ [2] [6] ]
		/// [ [3]     ]
		/// [ [4]     ]
		/// </summary>
		ColumnMajor,
	}

	/// <summary>
	/// Options for changing how the grid is laid out.
	/// </summary>
	public enum GridOptions
	{
		/// <summary>
		/// None
		/// </summary>
		None = 0,
		/// <summary>
		/// Size the content region to cover all of the items.
		/// This will result in no scrolling area.
		/// </summary>
		FitToContents = 1,
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
	/// <param name="id">Id for the grid.</param>
	/// <param name="items">The items to be displayed in the grid.</param>
	/// <param name="measureDelegate">The delegate to measure the size of each item.</param>
	/// <param name="drawDelegate">The delegate to draw each item.</param>
	/// <param name="gridOrder">What ordering should grid items use.</param>
	/// <param name="size">Size of the grid. Will not change the size of the grid cells.</param>
	/// <param name="gridOptions">Options for changing how the grid is laid out.</param>
	public static void Grid<T>(string id, IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, GridOrder gridOrder, Vector2 size, GridOptions gridOptions)
	{
		ArgumentNullException.ThrowIfNull(items);
		ArgumentNullException.ThrowIfNull(measureDelegate);
		ArgumentNullException.ThrowIfNull(drawDelegate);

		switch (gridOrder)
		{
			case GridOrder.RowMajor:
				GridImpl.ShowRowMajor(id, items, measureDelegate, drawDelegate, size, gridOptions);
				break;
			case GridOrder.ColumnMajor:
				GridImpl.ShowColumnMajor(id, items, measureDelegate, drawDelegate, size, gridOptions);
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
			internal int CellIndex { get; set; }
			internal int RowIndex { get; set; }
			internal int ColumnIndex { get; set; }
		}

		internal static CellData CalculateCellData(int itemIndex, int columnCount)
		{
			var cellData = new CellData
			{
				ColumnIndex = itemIndex % columnCount,
				RowIndex = itemIndex / columnCount,
				CellIndex = itemIndex
			};
			return cellData;
		}

		/// <summary>
		/// Shows the grid with the specified items and delegates and renders the items RowMajor.
		/// </summary>
		/// <typeparam name="T">The type of the items.</typeparam>
		/// <param name="id">Id for the grid.</param>
		/// <param name="items">The items to be displayed in the grid.</param>
		/// <param name="measureDelegate">The delegate to measure the size of each item.</param>
		/// <param name="drawDelegate">The delegate to draw each item.</param>
		/// <param name="gridSize">Size of the grid. Will not change the size of the grid cells.</param>
		/// <param name="gridOptions">Options for changing how the grid is laid out.</param>
		public static void ShowRowMajor<T>(string id, IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, Vector2 gridSize, GridOptions gridOptions)
		{
			if (gridSize.X <= 0)
			{
				gridSize.X = ImGui.GetContentRegionAvail().X;
			}
			if (gridSize.Y <= 0)
			{
				gridSize.Y = ImGui.GetContentRegionAvail().Y;
			}

			var itemSpacing = ImGui.GetStyle().ItemSpacing;
			var itemList = items.ToArray();
			var itemDimensions = itemList.Select(i => measureDelegate(i)).ToArray();
			var itemDimensionsWithSpacing = itemDimensions.Select(d => d + itemSpacing).ToArray();
			float gridWidth = gridSize.X;
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
					var cellData = CalculateCellData(i, numColumns);
					if (cellData.CellIndex < itemList.Length)
					{
						var thisItemSizeWithSpacing = itemDimensionsWithSpacing[cellData.CellIndex];

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

			if (gridOptions.HasFlag(GridOptions.FitToContents))
			{
				float width = columnWidths.Sum();
				float height = rowHeights.Sum();
				gridSize = new Vector2(width, height);
			}

			var drawList = ImGui.GetWindowDrawList();
			uint borderColor = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
			if (ImGui.BeginChild($"rowMajorGrid_{id}", gridSize))
			{
				var marginTopLeftCursor = ImGui.GetCursorScreenPos();
				if (EnableGridDebugDraw)
				{
					drawList.AddRect(marginTopLeftCursor, marginTopLeftCursor + gridSize, borderColor);
				}

				int lastItemIndex = itemList.Length - 1;
				for (int i = 0; i < itemList.Length; i++)
				{
					var itemStartCursor = ImGui.GetCursorScreenPos();

					var cellData = CalculateCellData(i, numColumns);
					int row = cellData.RowIndex;
					int column = cellData.ColumnIndex;
					int itemIndex = cellData.CellIndex;
					var cellSize = new Vector2(columnWidths[column], rowHeights[row]);

					if (EnableGridDebugDraw)
					{
						drawList.AddRect(itemStartCursor, itemStartCursor + cellSize, ImGui.GetColorU32(borderColor));
					}

					drawDelegate(itemList[itemIndex], cellSize, itemDimensions[itemIndex]);

					if (itemIndex != lastItemIndex)
					{
						bool sameRow = column < numColumns - 1;
						var newCursorScreenPos = sameRow
							? new Vector2(itemStartCursor.X + cellSize.X, itemStartCursor.Y)
							: new Vector2(marginTopLeftCursor.X, itemStartCursor.Y + cellSize.Y);

						ImGui.SetCursorScreenPos(newCursorScreenPos);
					}
				}
			}
			ImGui.EndChild();
		}

		internal class GridLayoutData
		{
			internal GridLayoutData() { }
			internal GridLayoutData(GridLayoutData other)
			{
				ColumnCount = other.ColumnCount;
				RowCount = other.RowCount;
				ColumnWidths = new Collection<float>(other.ColumnWidths);
				RowHeights = new Collection<float>(other.RowHeights);
			}

			internal int ColumnCount { get; set; }
			internal int RowCount { get; set; }
			internal Collection<float> ColumnWidths { get; set; } = [];
			internal Collection<float> RowHeights { get; set; } = [];
		}

		internal static GridLayoutData CalculateColumnMajorGridLayoutData(Vector2[] itemSizes, float containerHeight)
		{
			var gridLayoutData = new GridLayoutData
			{
				RowCount = 1,
				ColumnCount = itemSizes.Length,
				RowHeights = [itemSizes.Max(s => s.Y)],
				ColumnWidths = itemSizes.Select(i => i.X).ToCollection()
			};

			var previousGridLayoutData = new GridLayoutData(gridLayoutData);
			while (gridLayoutData.RowCount < itemSizes.Length)
			{
				int newRowCount = gridLayoutData.RowCount + 1;
				int newColumnCount = (int)Math.Ceiling(itemSizes.Length / (float)newRowCount);
				gridLayoutData = new GridLayoutData
				{
					RowCount = newRowCount,
					RowHeights = new float[newRowCount].ToCollection(),
					ColumnCount = newColumnCount,
					ColumnWidths = new float[newColumnCount].ToCollection()
				};

				for (int columnIndex = 0; columnIndex < gridLayoutData.ColumnCount; columnIndex++)
				{
					int itemBeginIndex = columnIndex * gridLayoutData.RowCount;
					int itemEndIndex = Math.Min(itemBeginIndex + gridLayoutData.RowCount, itemSizes.Length);
					for (int itemIndex = itemBeginIndex; itemIndex < itemEndIndex; itemIndex++)
					{
						int rowIndex = itemIndex - itemBeginIndex;
						var itemSize = itemSizes[itemIndex];
						gridLayoutData.RowHeights[rowIndex] = Math.Max(gridLayoutData.RowHeights[rowIndex], itemSize.Y);
					}

					var itemsInColumn = itemSizes.Skip(itemBeginIndex).Take(gridLayoutData.RowCount);
					float columnWidth = itemsInColumn.Max(i => i.X);
					gridLayoutData.ColumnWidths[columnIndex] = columnWidth;
				}

				if (gridLayoutData.RowHeights.Sum() > containerHeight)
				{
					gridLayoutData = previousGridLayoutData;
					break;
				}
				previousGridLayoutData = new GridLayoutData(gridLayoutData);
			}

			return gridLayoutData;
		}


		/// <summary>
		/// Shows the grid with the specified items and delegates and renders the items ColumnMajor
		/// </summary>
		/// <typeparam name="T">The type of the items.</typeparam>
		/// <param name="id">Id for the grid.</param>
		/// <param name="items">The items to be displayed in the grid.</param>
		/// <param name="measureDelegate">The delegate to measure the size of each item.</param>
		/// <param name="drawDelegate">The delegate to draw each item.</param>
		/// <param name="gridSize">Size of the grid. Will not change the size of the grid cells.</param>
		/// <param name="gridOptions">Options for changing how the grid is laid out.</param>
		public static void ShowColumnMajor<T>(string id, IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, Vector2 gridSize, GridOptions gridOptions)
		{
			if (gridSize.X <= 0)
			{
				gridSize.X = ImGui.GetContentRegionAvail().X;
			}
			if (gridSize.Y <= 0)
			{
				gridSize.Y = ImGui.GetContentRegionAvail().Y;
			}

			var itemSpacing = ImGui.GetStyle().ItemSpacing;
			var itemList = items.ToArray();
			int itemListCount = itemList.Length;
			var itemDimensions = itemList.Select(i => measureDelegate(i)).ToArray();
			var itemDimensionsWithSpacing = itemDimensions.Select(d => d + itemSpacing).ToArray();
			var gridLayoutData = CalculateColumnMajorGridLayoutData(itemDimensionsWithSpacing, gridSize.Y);

			if (gridOptions.HasFlag(GridOptions.FitToContents))
			{
				float width = gridLayoutData.ColumnWidths.Sum();
				float height = gridLayoutData.RowHeights.Sum();
				gridSize = new Vector2(width, height);
			}

			var drawList = ImGui.GetWindowDrawList();
			uint borderColor = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
			if (ImGui.BeginChild($"columnMajorGrid_{id}", gridSize, ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar))
			{
				var gridTopLeft = ImGui.GetCursorScreenPos();
				if (EnableGridDebugDraw)
				{
					drawList.AddRect(gridTopLeft, gridTopLeft + gridSize, borderColor);
				}

				var columnTopLeft = gridTopLeft;
				for (int columnIndex = 0; columnIndex < gridLayoutData.ColumnCount; columnIndex++)
				{
					bool isFirstColumn = columnIndex == 0;
					float previousColumnWidth = isFirstColumn ? 0f : gridLayoutData.ColumnWidths[columnIndex - 1];

					float columnCursorX = columnTopLeft.X + previousColumnWidth;
					float columnCursorY = columnTopLeft.Y;
					columnTopLeft = new Vector2(columnCursorX, columnCursorY);
					ImGui.SetCursorScreenPos(columnTopLeft);

					var cellTopLeft = ImGui.GetCursorScreenPos();
					int itemBeginIndex = columnIndex * gridLayoutData.RowCount;
					int itemEndIndex = Math.Min(itemBeginIndex + gridLayoutData.RowCount, itemListCount);
					for (int itemIndex = itemBeginIndex; itemIndex < itemEndIndex; itemIndex++)
					{
						bool isFirstRow = itemIndex == itemBeginIndex;
						float previousCellHeight = isFirstRow ? 0f : itemDimensionsWithSpacing[itemIndex].Y;

						float cellCursorX = cellTopLeft.X;
						float cellCursorY = cellTopLeft.Y + previousCellHeight;
						cellTopLeft = new Vector2(cellCursorX, cellCursorY);
						ImGui.SetCursorScreenPos(cellTopLeft);

						int rowIndex = itemIndex - itemBeginIndex;
						float cellWidth = gridLayoutData.ColumnWidths[columnIndex];
						float cellHeight = gridLayoutData.RowHeights[rowIndex];
						var cellSize = new Vector2(cellWidth, cellHeight);

						if (EnableGridDebugDraw)
						{
							drawList.AddRect(cellTopLeft, cellTopLeft + cellSize, borderColor);
						}
						drawDelegate(itemList[itemIndex], cellSize, itemDimensions[itemIndex]);
					}
				}
			}
			ImGui.EndChild();
		}
	}
}
