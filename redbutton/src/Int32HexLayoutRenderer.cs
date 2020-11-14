using NLog.LayoutRenderers;
using NLog.LayoutRenderers.Wrappers;

namespace redbutton
{
	[LayoutRenderer("int32hex")]
	public sealed class Int32HexLayoutRenderer : WrapperLayoutRendererBase
	{
		protected override string Transform(string text)
		{
			if(int.TryParse(text, out var value))
				return value.ToString("x8");
			return text;
		}
	}
}
