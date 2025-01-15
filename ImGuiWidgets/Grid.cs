namespace ktsu.ImGuiWidgets;

using System.Drawing;
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
	/// <param name="gridSize">Size of the grid. This will not change the size of the grid cells. Setting any axis to 0 will use the available space.</param>
	/// <param name="gridOptions">Options for changing how the grid is laid out.</param>
	public static void Grid<T>(string id, IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, GridOrder gridOrder, Vector2 gridSize, GridOptions gridOptions)
	{
		ArgumentNullException.ThrowIfNull(items);
		ArgumentNullException.ThrowIfNull(measureDelegate);
		ArgumentNullException.ThrowIfNull(drawDelegate);

		switch (gridOrder)
		{
			case GridOrder.RowMajor:
				GridImpl.ShowRowMajor(id, items, measureDelegate, drawDelegate, gridSize, gridOptions);
				break;
			case GridOrder.ColumnMajor:
				GridImpl.ShowColumnMajor(id, items, measureDelegate, drawDelegate, gridSize, gridOptions);
				break;
			default:
				throw new NotImplementedException($"Unable to draw grid as {gridOrder} is not implemented");
		}
	}

	/// <summary>
	/// Contains the implementation details for rendering grids.
	/// </summary>
	internal static class GridImpl
	{
		internal class GridLayout()
		{
			internal Point Dimensions { private get; init; }
			internal float[] ColumnWidths { get; init; } = [];
			internal float[] RowHeights { get; init; } = [];
			internal int ColumnCount => Dimensions.X;
			internal int RowCount => Dimensions.Y;
		}

		#region RowMajor
		private static Point CalculateRowMajorCellLocation(int itemIndex, int columnCount)
		{
			int columnIndex = itemIndex % columnCount;
			int rowIndex = itemIndex / columnCount;
			return new Point(columnIndex, rowIndex);
		}

		internal static GridLayout CalculateRowMajorGridLayout(IList<Vector2> itemSizes, float containerWidth)
		{
			float widestElementHeight = itemSizes.Max(itemSize => itemSize.X);
			GridLayout currentGridLayout = new()
			{
				Dimensions = new(1, itemSizes.Count),
				ColumnWidths = [widestElementHeight],
				RowHeights = itemSizes.Select(itemSize => itemSize.Y).ToArray()
			};

			var previousGridLayout = currentGridLayout;
			while (currentGridLayout.ColumnCount < itemSizes.Count)
			{
				int newColumnCount = currentGridLayout.ColumnCount + 1;
				int newRowCount = (int)MathF.Ceiling(itemSizes.Count / (float)newColumnCount);
				currentGridLayout = new()
				{
					Dimensions = new(newColumnCount, newRowCount),
					ColumnWidths = new float[newColumnCount],
					RowHeights = new float[newRowCount],
				};

				for (int i = 0; i < itemSizes.Count; i++)
				{
					var itemSize = itemSizes[i];
					var cellLocation = CalculateRowMajorCellLocation(i, newColumnCount);

					float maxColumnWidth = currentGridLayout.ColumnWidths[cellLocation.X];
					maxColumnWidth = Math.Max(maxColumnWidth, itemSize.X);
					currentGridLayout.ColumnWidths[cellLocation.X] = maxColumnWidth;

					float maxRowHeight = currentGridLayout.RowHeights[cellLocation.Y];
					maxRowHeight = Math.Max(maxRowHeight, itemSize.Y);
					currentGridLayout.RowHeights[cellLocation.Y] = maxRowHeight;
				}

				float totalWidth = currentGridLayout.ColumnWidths.Sum();
				if (totalWidth > containerWidth)
				{
					currentGridLayout = previousGridLayout;
					break;
				}
				previousGridLayout = currentGridLayout;
			}

			return currentGridLayout;
		}

		internal static void ShowRowMajor<T>(string id, IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, Vector2 gridSize, GridOptions gridOptions)
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
			var gridLayout = CalculateRowMajorGridLayout(itemDimensionsWithSpacing, gridSize.X);

			if (gridOptions.HasFlag(GridOptions.FitToContents))
			{
				float width = gridLayout.ColumnWidths.Sum();
				float height = gridLayout.RowHeights.Sum();
				gridSize = new Vector2(width, height);
			}

			var drawList = ImGui.GetWindowDrawList();
			uint borderColor = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
			if (ImGui.BeginChild($"rowMajorGrid_{id}", gridSize, ImGuiChildFlags.None))
			{
				var gridTopLeft = ImGui.GetCursorScreenPos();
				if (EnableGridDebugDraw)
				{
					drawList.AddRect(gridTopLeft, gridTopLeft + gridSize, borderColor);
				}

				var rowTopleft = gridTopLeft;
				for (int rowIndex = 0; rowIndex < gridLayout.RowCount; rowIndex++)
				{
					bool isFirstRow = rowIndex == 0;
					float previousRowHeight = isFirstRow ? 0f : gridLayout.RowHeights[rowIndex - 1];

					float columnCursorX = rowTopleft.X;
					float columnCursorY = rowTopleft.Y + previousRowHeight;
					rowTopleft = new(columnCursorX, columnCursorY);
					ImGui.SetCursorScreenPos(rowTopleft);

					var cellTopLeft = ImGui.GetCursorScreenPos();
					int itemBeginIndex = rowIndex * gridLayout.ColumnCount;
					int itemEndIndex = Math.Min(itemBeginIndex + gridLayout.ColumnCount, itemListCount);
					for (int itemIndex = itemBeginIndex; itemIndex < itemEndIndex; itemIndex++)
					{
						int columnIndex = itemIndex - itemBeginIndex;
						bool isFirstColumn = itemIndex == itemBeginIndex;
						float previousCellWidth = isFirstColumn ? 0f : gridLayout.ColumnWidths[columnIndex - 1];

						float cellCursorX = cellTopLeft.X + previousCellWidth;
						float cellCursorY = cellTopLeft.Y;
						cellTopLeft = new(cellCursorX, cellCursorY);
						ImGui.SetCursorScreenPos(cellTopLeft);

						float cellWidth = gridLayout.ColumnWidths[columnIndex];
						float cellHeight = gridLayout.RowHeights[rowIndex];
						Vector2 cellSize = new(cellWidth, cellHeight);

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
		#endregion

		#region ColumnMajor
		private static Point CalculateColumnMajorCellLocation(int itemIndex, int rowCount)
		{
			int columnIndex = itemIndex / rowCount;
			int rowIndex = itemIndex % rowCount;
			return new Point(columnIndex, rowIndex);
		}

		internal static GridLayout CalculateColumnMajorGridLayout(IList<Vector2> itemSizes, float containerHeight)
		{
			float tallestElementHeight = itemSizes.Max(itemSize => itemSize.Y);
			GridLayout currentGridLayout = new()
			{
				Dimensions = new(itemSizes.Count, 1),
				ColumnWidths = itemSizes.Select(itemSize => itemSize.X).ToArray(),
				RowHeights = [tallestElementHeight],
			};

			var previousGridLayout = currentGridLayout;
			while (currentGridLayout.RowCount < itemSizes.Count)
			{
				int newRowCount = currentGridLayout.RowCount + 1;
				int newColumnCount = (int)MathF.Ceiling(itemSizes.Count / (float)newRowCount);
				currentGridLayout = new()
				{
					Dimensions = new(newColumnCount, newRowCount),
					ColumnWidths = new float[newColumnCount],
					RowHeights = new float[newRowCount],
				};

				for (int i = 0; i < itemSizes.Count; i++)
				{
					var itemSize = itemSizes[i];
					var cellLocation = CalculateColumnMajorCellLocation(i, newRowCount);

					float maxColumnWidth = currentGridLayout.ColumnWidths[cellLocation.X];
					maxColumnWidth = Math.Max(maxColumnWidth, itemSize.X);
					currentGridLayout.ColumnWidths[cellLocation.X] = maxColumnWidth;

					float maxRowHeight = currentGridLayout.RowHeights[cellLocation.Y];
					maxRowHeight = Math.Max(maxRowHeight, itemSize.Y);
					currentGridLayout.RowHeights[cellLocation.Y] = maxRowHeight;
				}

				if (currentGridLayout.RowHeights.Sum() > containerHeight)
				{
					currentGridLayout = previousGridLayout;
					break;
				}
				previousGridLayout = currentGridLayout;
			}

			return currentGridLayout;
		}

		internal static void ShowColumnMajor<T>(string id, IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, Vector2 gridSize, GridOptions gridOptions)
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
			var gridLayout = CalculateColumnMajorGridLayout(itemDimensionsWithSpacing, gridSize.Y);

			if (gridOptions.HasFlag(GridOptions.FitToContents))
			{
				float width = gridLayout.ColumnWidths.Sum();
				float height = gridLayout.RowHeights.Sum();
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
				for (int columnIndex = 0; columnIndex < gridLayout.ColumnCount; columnIndex++)
				{
					bool isFirstColumn = columnIndex == 0;
					float previousColumnWidth = isFirstColumn ? 0f : gridLayout.ColumnWidths[columnIndex - 1];

					float columnCursorX = columnTopLeft.X + previousColumnWidth;
					float columnCursorY = columnTopLeft.Y;
					columnTopLeft = new Vector2(columnCursorX, columnCursorY);
					ImGui.SetCursorScreenPos(columnTopLeft);

					var cellTopLeft = ImGui.GetCursorScreenPos();
					int itemBeginIndex = columnIndex * gridLayout.RowCount;
					int itemEndIndex = Math.Min(itemBeginIndex + gridLayout.RowCount, itemListCount);
					for (int itemIndex = itemBeginIndex; itemIndex < itemEndIndex; itemIndex++)
					{
						int rowIndex = itemIndex - itemBeginIndex;
						bool isFirstRow = itemIndex == itemBeginIndex;
						float previousCellHeight = isFirstRow ? 0f : itemDimensionsWithSpacing[rowIndex - 1].Y;

						float cellCursorX = cellTopLeft.X;
						float cellCursorY = cellTopLeft.Y + previousCellHeight;
						cellTopLeft = new(cellCursorX, cellCursorY);
						ImGui.SetCursorScreenPos(cellTopLeft);

						float cellWidth = gridLayout.ColumnWidths[columnIndex];
						float cellHeight = gridLayout.RowHeights[rowIndex];
						Vector2 cellSize = new(cellWidth, cellHeight);

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
		#endregion
	}
}
