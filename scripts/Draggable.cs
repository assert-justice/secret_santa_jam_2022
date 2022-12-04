using Godot;
using System;

public class Draggable : StaticBody2D
{
	private bool dragging = false;

	public override void _Process(float delta)
	{
		if(dragging){
			var mousePos = GetViewport().GetMousePosition();
			this.Position = mousePos;
		}
	}
	// private void _on_StaticBody2D_input_event(object viewport, object @event, int shape_idx)
	// {
	// 	if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed){
	// 		if (mouseEvent.ButtonIndex == 1) dragging = !dragging;
	// 	GD.Print(dragging);
	// 	}
	// }
	private void _on_Control_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed){
			if (mouseEvent.ButtonIndex == 1) dragging = !dragging;
		GD.Print(dragging);
		}

	}
}


