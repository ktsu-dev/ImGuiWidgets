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
	/// <param name="gridOrder">What ordering should grid items use</param>
	/// <param name="size">Size of the grid</param>
	public static void Grid<T>(string id, IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, GridOrder gridOrder, Vector2 size)
	{
		ArgumentNullException.ThrowIfNull(items);
		ArgumentNullException.ThrowIfNull(measureDelegate);
		ArgumentNullException.ThrowIfNull(drawDelegate);

		switch (gridOrder)
		{
			case GridOrder.RowMajor:
				GridImpl.ShowRowMajor(id, items, measureDelegate, drawDelegate, size);
				break;
			case GridOrder.ColumnMajor:
				GridImpl.ShowColumnMajor(id, items, measureDelegate, drawDelegate, size);
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
		/// Shows the grid with the specified items and delegates and renders the items RowMajor
		/// </summary>
		/// <typeparam name="T">The type of the items.</typeparam>
		/// <param name="id">Id for the grid.</param>
		/// <param name="items">The items to be displayed in the grid.</param>
		/// <param name="measureDelegate">The delegate to measure the size of each item.</param>
		/// <param name="drawDelegate">The delegate to draw each item.</param>
		/// <param name="size"></param>
		public static void ShowRowMajor<T>(string id, IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, Vector2 size)
		{
			var itemSpacing = ImGui.GetStyle().ItemSpacing;
			var itemList = items.ToArray();
			var itemDimensions = itemList.Select(i => measureDelegate(i)).ToArray();
			var itemDimensionsWithSpacing = itemDimensions.Select(d => d + itemSpacing).ToArray();
			float gridWidth = size.X;
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

			if (ImGui.BeginChild($"rowMajorGrid_{id}", size, ImGuiChildFlags.Borders, ImGuiWindowFlags.HorizontalScrollbar))
			{
				var marginTopLeftCursor = ImGui.GetCursorScreenPos();
				float gridHeight = rowHeights.Sum(h => h);

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
						uint borderColor = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
						var drawList = ImGui.GetWindowDrawList();
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


		/// <summary>
		/// Shows the grid with the specified items and delegates and renders the items ColumnMajor
		/// </summary>
		/// <typeparam name="T">The type of the items.</typeparam>
		/// <param name="id">Id for the grid.</param>
		/// <param name="items">The items to be displayed in the grid.</param>
		/// <param name="measureDelegate">The delegate to measure the size of each item.</param>
		/// <param name="drawDelegate">The delegate to draw each item.</param>
		/// <param name="size"></param>
		public static void ShowColumnMajor<T>(string id, IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate, Vector2 size)
		{
			var itemSpacing = ImGui.GetStyle().ItemSpacing;
			var itemList = items.ToArray();
			int itemListCount = itemList.Length;
			var itemDimensions = itemList.Select(i => measureDelegate(i)).ToArray();
			var itemDimensionsWithSpacing = itemDimensions.Select(d => d + itemSpacing).ToArray();
			// Assumption: All items are roughly the same size and we can simply
			// select the max height and use that for every item.
			float rowHeight = itemDimensionsWithSpacing.Max(d => d.Y);
			float heightAvailable = (int)size.Y;

			Collection<float> columnWidths = [];
			int maxItemsPerColumn = Math.Max(1, (int)(heightAvailable / rowHeight));
			int numColumns = (int)Math.Ceiling(itemListCount / (float)maxItemsPerColumn);
			for (int columnIndex = 0; columnIndex < numColumns; columnIndex++)
			{
				int itemCountToSkip = columnIndex * maxItemsPerColumn;
				var itemsInColumn = itemDimensionsWithSpacing.Skip(itemCountToSkip).Take(maxItemsPerColumn);
				columnWidths.Add(itemsInColumn.Max(d => d.X));
			}

			if (ImGui.BeginChild($"columnMajorGrid_{id}", size, ImGuiChildFlags.Borders, ImGuiWindowFlags.HorizontalScrollbar))
			{
				var gridTopLeft = ImGui.GetCursorScreenPos();
				var previousColumnTopLeft = gridTopLeft;
				float previousColumnWidth = 0f;
				for (int columnIndex = 0; columnIndex < numColumns; columnIndex++)
				{
					float columnCursorX = previousColumnTopLeft.X + previousColumnWidth;
					float columnCursorY = previousColumnTopLeft.Y;
					var columnTopLeft = new Vector2(columnCursorX, columnCursorY);
					ImGui.SetCursorScreenPos(columnTopLeft);

					int beginIndex = columnIndex * maxItemsPerColumn;
					int endIndex = Math.Min(beginIndex + maxItemsPerColumn, itemListCount);
					for (int itemIndex = beginIndex; itemIndex < endIndex; itemIndex++)
					{
						int columnItemIndex = itemIndex - beginIndex;
						float cellCursorX = columnCursorX;
						float cellCursorY = columnCursorY + (columnItemIndex * rowHeight);
						var cellTopLeft = new Vector2(cellCursorX, cellCursorY);
						ImGui.SetCursorScreenPos(cellTopLeft);

						var cellSize = new Vector2(columnWidths[columnIndex], rowHeight);

						if (EnableGridDebugDraw)
						{
							uint borderColor = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
							var drawList = ImGui.GetWindowDrawList();
							drawList.AddRect(cellTopLeft, cellTopLeft + cellSize, ImGui.GetColorU32(borderColor));
						}
						drawDelegate(itemList[itemIndex], cellSize, itemDimensions[itemIndex]);
					}

					previousColumnTopLeft = columnTopLeft;
					previousColumnWidth = columnWidths[columnIndex];
				}
			}
			ImGui.EndChild();
		}
	}
}
