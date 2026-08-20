namespace WoF.Editor
{
	/// <summary>
	/// A named capture resolution. Declared landscape first, rotated on demand for portrait.
	/// </summary>
	public readonly struct AspectResolution
	{
		public readonly string Label;
		public readonly int Width;
		public readonly int Height;

		public AspectResolution(string label, int width, int height)
		{
			Label = label;
			Width = width;
			Height = height;
		}

		public AspectResolution Rotated => new AspectResolution(Label, Height, Width);

		public string FileLabel => Label.Replace(':', '-');
	}
}
