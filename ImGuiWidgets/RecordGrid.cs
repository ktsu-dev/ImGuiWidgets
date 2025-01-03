//namespace ktsu.ImGuiWidgets;

//using System.Numerics;

///// <summary>
///// Provides custom ImGui widgets.
///// </summary>
//public static partial class ImGuiWidgets
//{
//	public delegate object SelectRecordGridProperty<TRecord>(TRecord property);
//	public delegate Vector2 MeasureRecordGridProperty(object property);
//	public delegate void DrawRecordGridProperty(object property, Vector2 cellSize, Vector2 itemSize);

//	public class RecordGridColumn<TRecord>
//	{
//		public SelectRecordGridProperty<TRecord> SelectRecordGridProperty { get; init; } = record => record!;
//		public MeasureRecordGridProperty MeasureRecordGridProperty { get; init; } = property => Vector2.Zero;
//		public DrawRecordGridProperty DrawRecordGridProperty { get; init; } = (property, cellSize, itemSize) => { };
//	}

//	public static void RecordGrid<TRecord>(IEnumerable<TRecord> items, IEnumerable<RecordGridColumn<TRecord>> columns) =>
//		RecordGridImpl.Show(items, columns);

//	internal class RecordGridImpl
//	{
//		public static void Show<TRecord>(IEnumerable<TRecord> items, IEnumerable<RecordGridColumn<TRecord>> columns)
//		{

//		}
//	}
//}
