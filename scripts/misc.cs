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

public class NumberGenerator
{
	private Random random;

	public NumberGenerator()
	{
		// Initialize the random number generator
		random = new Random();
	}

	public int GenerateNumber(int a, int b)
	{
		// Generate a random number between 1 and 5 (exclusive upper bound)
		return random.Next(a, b);
	}

    public float GenerateFloat(float a, float b) {
        return (float)(random.NextDouble() * (b - a) + a);
    }
}
