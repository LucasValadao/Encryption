﻿namespace UtinyRipper.Exporter.YAML
{
	/// <summary>
	/// Specifies the style of a sequence.
	/// </summary>
	public enum SequenceStyle
	{
		/// <summary>
		/// The block sequence style.
		/// </summary>
		Block,

		/// <summary>
		/// The flow sequence style.
		/// </summary>
		Flow,

		/// <summary>
		/// SIngle line with hex data
		/// </summary>
		Raw,
	}

	public static class SequenceStyleExtensions
	{
		/// <summary>
		/// Get scalar style corresponding to current sequence style
		/// </summary>
		/// <param name="_this">Sequence style</param>
		/// <returns>Corresponding scalar style</returns>
		public static ScalarStyle ToScalarStyle(this SequenceStyle _this)
		{
			if(_this == SequenceStyle.Raw)
			{
				return ScalarStyle.Hex;
			}
			return ScalarStyle.Plain;
		}
	}
}
