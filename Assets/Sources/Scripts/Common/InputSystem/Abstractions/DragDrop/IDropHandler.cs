﻿using UnityEngine;

namespace Potman.Common.InputSystem.Abstractions.DragDrop
{
	public interface IDropHandler
	{
		/// <summary>
		/// Target drop handler view it can be raycast
		/// </summary>
		GameObject DropHandlerView { get; }
		
		/// <summary>
		/// Is it allowed to drop the current IDraggable into the current IDropHandler.
		/// </summary>
		/// <param name="draggable">to Drop into current IDropHandler</param>
		/// <returns></returns>
		bool CanDrop(IDraggable draggable);
	}
}