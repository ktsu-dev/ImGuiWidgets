namespace ktsu.ImGuiWidgets;

using System.Collections.ObjectModel;
using System.Drawing;
using System.Numerics;
using Extensions;
using ImGuiNET;

public static partial class ImGuiWidgets
{
	/// <summary>
	/// An enum to specify the layout direction of the divider container.
	/// </summary>
	public enum DividerLayout
	{
		/// <summary>
		/// The container will be laid out in columns.
		/// </summary>
		Columns,
		/// <summary>
		/// The container will be laid out in rows.
		/// </summary>
		Rows,
	}

	/// <summary>
	/// A container that can be divided into dragable zones.
	/// Useful for creating resizable layouts.
	/// Containers can be nested to create complex layouts.
	/// </summary>
	/// <remarks>
	/// Create a new divider container with a callback for when the container is resized and a specified layout direction.
	/// </remarks>
	/// <param name="id">The ID of the container.</param>
	/// <param name="onResized">A callback for when the container is resized.</param>
	/// <param name="layout">The layout direction of the container.</param>
	/// <param name="zones">The zones to add to the container.</param>
	public class DividerContainer(string id, Action<DividerContainer>? onResized, DividerLayout layout, IEnumerable<DividerZone> zones)
	{
		/// <summary>
		/// An ID for the container.
		/// </summary>
		public string Id { get; init; } = id;
		private int DragIndex { get; set; } = -1;
		private List<DividerZone> Zones { get; init; } = zones.ToList();

		/// <summary>
		/// Create a new divider container with a callback for when the container is resized and a specified layout direction.
		/// </summary>
		/// <param name="id">The ID of the container.</param>
		/// <param name="onResized">A callback for when the container is resized.</param>
		/// <param name="layout">The layout direction of the container.</param>
		public DividerContainer(string id, Action<DividerContainer>? onResized, DividerLayout layout)
			: this(id, onResized, layout, [])
		{
		}

		/// <summary>
		/// Create a new divider container with the default layout direction of columns.
		/// </summary>
		/// <param name="id">The ID of the container.</param>
		public DividerContainer(string id)
			: this(id, null, DividerLayout.Columns)
		{
		}

		/// <summary>
		/// Create a new divider container with a specified layout direction.
		/// </summary>
		/// <param name="id">The ID of the container.</param>
		/// <param name="layout">The layout direction of the container.</param>
		public DividerContainer(string id, DividerLayout layout)
			: this(id, null, layout)
		{
		}

		/// <summary>
		/// Create a new divider container with a callback for when the container is resized and the default layout direction of columns.
		/// </summary>
		/// <param name="id">The ID of the container.</param>
		/// <param name="onResized">A callback for when the container is resized.</param>
		public DividerContainer(string id, Action<DividerContainer>? onResized)
			: this(id, onResized, DividerLayout.Columns)
		{
		}

		/// <summary>
		/// Tick the container to update and draw its contents.
		/// </summary>
		/// <param name="dt">The delta time since the last tick.</param>
		/// <exception cref="NotImplementedException">Thrown if the layout direction is not supported.</exception>
		public void Tick(float dt)
		{
			var style = ImGui.GetStyle();
			var windowPadding = style.WindowPadding;
			var drawList = ImGui.GetWindowDrawList();
			var containerSize = ImGui.GetWindowSize() - (windowPadding * 2.0f);

			var layoutMask = layout switch
			{
				DividerLayout.Columns => new Vector2(1, 0),
				DividerLayout.Rows => new Vector2(0, 1),
				_ => throw new NotImplementedException(),
			};

			var layoutMaskInverse = layout switch
			{
				DividerLayout.Columns => new Vector2(0, 1),
				DividerLayout.Rows => new Vector2(1, 0),
				_ => throw new NotImplementedException(),
			};

			var windowPos = ImGui.GetWindowPos();
			var advance = windowPos + windowPadding;

			Vector2 CalculateAdvance(DividerZone z)
			{
				var advance = containerSize * z.Size * layoutMask;

				var first = Zones.First();
				var last = Zones.Last();
				if (first != last && z == first)
				{
					advance += windowPadding * 0.5f * layoutMask;
				}

				return new Vector2((float)Math.Round(advance.X), (float)Math.Round(advance.Y));
			}

			Vector2 CalculateZoneSize(DividerZone z)
			{
				var zoneSize = (containerSize * z.Size * layoutMask) + (containerSize * layoutMaskInverse);

				var first = Zones.First();
				var last = Zones.Last();
				if (first != last)
				{
					if (z == first || z == last)
					{
						zoneSize -= windowPadding * 0.5f * layoutMask;
					}
					else
					{
						zoneSize -= windowPadding * layoutMask;
					}
				}

				return new Vector2((float)Math.Floor(zoneSize.X), (float)Math.Floor(zoneSize.Y));
			}

			ImGui.SetNextWindowPos(advance);
			ImGui.BeginChild(Id, containerSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoSavedSettings);

			foreach (var z in Zones)
			{
				var zoneSize = CalculateZoneSize(z);
				ImGui.SetNextWindowPos(advance);
				ImGui.BeginChild(z.Id, zoneSize, ImGuiChildFlags.Border, ImGuiWindowFlags.NoSavedSettings);
				z.Tick(dt);
				ImGui.EndChild();

				advance += CalculateAdvance(z);
			}

			ImGui.EndChild();

			//render the handles last otherwise they'll be covered by the other zones windows and wont receive hover events

			//reset the advance to the top left of the container
			advance = windowPos + windowPadding;
			float resize = 0;
			var mousePos = ImGui.GetMousePos();
			bool resetSize = false;
			foreach (var (z, i) in Zones.WithIndex())
			{
				//draw the grab handle if we're not the last zone
				if (z != Zones.Last())
				{
					var zoneSize = CalculateZoneSize(z);
					var lineA = advance + (zoneSize * layoutMask) + (windowPadding * 0.5f * layoutMask);
					var lineB = lineA + (zoneSize * layoutMaskInverse);
					float lineWidth = style.WindowPadding.X * 0.5f;
					float grabWidth = style.WindowPadding.X * 2;
					var grabBox = new Vector2(grabWidth, grabWidth) * 0.5f;
					var grabMin = lineA - (grabBox * layoutMask);
					var grabMax = lineB + (grabBox * layoutMask);
					var grabSize = grabMax - grabMin;
					RectangleF handleRect = new(grabMin.X, grabMin.Y, grabSize.X, grabSize.Y);
					bool handleHovered = handleRect.Contains(mousePos.X, mousePos.Y);
					bool mouseClickedThisFrame = ImGui.IsMouseClicked(ImGuiMouseButton.Left);
					bool handleClicked = handleHovered && mouseClickedThisFrame;
					bool handleDoubleClicked = handleHovered && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left);

					if (handleClicked)
					{
						DragIndex = i;
					}

					if (handleDoubleClicked)
					{
						resetSize = true;
					}
					else if (DragIndex == i)
					{
						if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
						{
							var mousePosLocal = mousePos - advance;

							var first = Zones.First();
							var last = Zones.Last();
							if (first != last && z != first)
							{
								mousePosLocal += windowPadding * 0.5f * layoutMask;
							}

							float requestedSize = layout switch
							{
								DividerLayout.Columns => mousePosLocal.X / containerSize.X,
								DividerLayout.Rows => mousePosLocal.Y / containerSize.Y,
								_ => throw new NotImplementedException(),
							};
							resize = Math.Clamp(requestedSize, 0.1f, 0.9f);
						}
						else
						{
							DragIndex = -1;
						}
					}

					var lineColor = DragIndex == i ? new Vector4(1, 1, 1, 0.7f) : handleHovered ? new Vector4(1, 1, 1, 0.5f) : new Vector4(1, 1, 1, 0.3f);
					//drawList.AddRectFilled(grabMin, grabMax, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 0.3f)));
					drawList.AddLine(lineA, lineB, ImGui.ColorConvertFloat4ToU32(lineColor), lineWidth);

					if (handleHovered || DragIndex == i)
					{
						ImGui.SetMouseCursor(layout switch
						{
							DividerLayout.Columns => ImGuiMouseCursor.ResizeEW,
							DividerLayout.Rows => ImGuiMouseCursor.ResizeNS,
							_ => throw new NotImplementedException(),
						});
					}
				}

				advance += CalculateAdvance(z);
			}

			//do the actual resize at the end of the tick so that we don't mess with the dimensions of the layout mid rendering
			if (DragIndex > -1)
			{
				if (resetSize)
				{
					resize = Zones[DragIndex].InitialSize;
				}
				var resizedZone = Zones[DragIndex];
				var neighbourZone = Zones[DragIndex + 1];
				float combinedSize = resizedZone.Size + neighbourZone.Size;
				float maxSize = combinedSize - 0.1f;
				resize = Math.Clamp(resize, 0.1f, maxSize);
				bool sizeDidChange = resizedZone.Size != resize;
				resizedZone.Size = resize;
				neighbourZone.Size = combinedSize - resize;
				if (sizeDidChange)
				{
					onResized?.Invoke(this);
				}

				if (resetSize)
				{
					DragIndex = -1;
				}
			}
		}

		/// <summary>
		/// Add a zone to the container.
		/// </summary>
		/// <param name="id">The ID of the zone.</param>
		/// <param name="size">The size of the zone.</param>
		/// <param name="resizable">Whether the zone is resizable.</param>
		/// <param name="tickDelegate">The delegate to call when the zone is ticked.</param>
		public void Add(string id, float size, bool resizable, Action<float> tickDelegate) => Zones.Add(new(id, size, resizable, tickDelegate));

		/// <summary>
		/// Add a zone to the container.
		/// </summary>
		/// <param name="id">The ID of the zone.</param>
		/// <param name="size">The size of the zone.</param>
		/// <param name="tickDelegate">The delegate to call when the zone is ticked.</param>
		public void Add(string id, float size, Action<float> tickDelegate) => Zones.Add(new(id, size, tickDelegate));

		/// <summary>
		/// Add a zone to the container.
		/// </summary>
		/// <param name="id">The ID of the zone.</param>
		/// <param name="tickDelegate">The delegate to call when the zone is ticked.</param>
		public void Add(string id, Action<float> tickDelegate)
		{
			float size = 1.0f / (Zones.Count + 1);
			Zones.Add(new(id, size, tickDelegate));
		}

		/// <summary>
		/// Add a zone to the container.
		/// </summary>
		/// <param name="zone">The zone to add</param>
		public void Add(DividerZone zone) => Zones.Add(zone);

		/// <summary>
		/// Remove a zone from the container.
		/// </summary>
		/// <param name="id">The ID of the zone to remove.</param>
		public void Remove(string id)
		{
			var zone = Zones.FirstOrDefault(z => z.Id == id);
			if (zone != null)
			{
				Zones.Remove(zone);
			}
		}

		/// <summary>
		/// Remome all zones from the container.
		/// </summary>
		public void Clear() => Zones.Clear();

		/// <summary>
		/// Set the size of a zone by its ID.
		/// </summary>
		/// <param name="id">The ID of the zone to set the size of.</param>
		/// <param name="size">The size to set.</param>
		public void SetSize(string id, float size)
		{
			var zone = Zones.FirstOrDefault(z => z.Id == id);
			if (zone != null)
			{
				zone.Size = size;
			}
		}

		/// <summary>
		/// Set the size of a zone by its index.
		/// </summary>
		/// <param name="index">The index of the zone to set the size of.</param>
		/// <param name="size">The size to set.</param>
		public void SetSize(int index, float size)
		{
			if (index >= 0 && index < Zones.Count)
			{
				Zones[index].Size = size;
			}
		}

		/// <summary>
		/// Set the sizes of all zones in this container from a collection of sizes.
		/// </summary>
		/// <param name="sizes">The collection of sizes to set.</param>
		/// <exception cref="ArgumentException"></exception>
		public void SetSizesFromList(ICollection<float> sizes)
		{
			ArgumentNullException.ThrowIfNull(sizes, nameof(sizes));

			if (sizes.Count != Zones.Count)
			{
				throw new ArgumentException("List of sizes must be the same length as the zones list");
			}

			foreach (var (s, i) in sizes.WithIndex())
			{
				Zones[i].Size = s;
			}
		}

		/// <summary>
		/// Get a collection of the sizes of all zones in this container.
		/// </summary>
		/// <returns>A collection of the sizes of all zones in this container.</returns>
		public Collection<float> GetSizes() => Zones.Select(z => z.Size).ToCollection();
	}
}
