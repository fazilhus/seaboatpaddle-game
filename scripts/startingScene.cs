using Godot;
using System;

public partial class startingScene : Node3D
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("change_window_mode")) {
			var win = GetWindow();
			switch (win.Mode) {
				case Window.ModeEnum.Windowed: {
					win.Mode = Window.ModeEnum.Fullscreen;
					break;
				}
				case Window.ModeEnum.Fullscreen: {
					win.Mode = Window.ModeEnum.Windowed;
					break;
				}
			}
        }
    }
}
