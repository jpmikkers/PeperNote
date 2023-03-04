namespace PeperNote;
using System.Windows;
using System.Windows.Media;

public class ScreenBoundsConverter
{
	private Matrix _transform;

	public ScreenBoundsConverter(Visual visual)
	{
		_transform =
		  PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
	}

	public Rect ConvertBounds(System.Drawing.Rectangle bounds)
	{
		var result = new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height);
		result.Transform(_transform);
		return result;
	}
}
