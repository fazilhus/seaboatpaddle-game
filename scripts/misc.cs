using Godot;
using System;

public partial class misc : Node
{
    public static Godot.Collections.Array<T> GetChildren<[MustBeVariant] T>(Node _node) where T : Node {
        var res = new Godot.Collections.Array<T>();
        foreach (var child in _node.GetChildren()) {
            if (child is T) {
                res.Add(child as T);
            }
        }

        return res;
    }
}
