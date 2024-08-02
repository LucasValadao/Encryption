﻿namespace UtinyRipper.Classes.Shaders
{
	public struct SerializedShaderFloatValue : IAssetReadable
	{
		public void Read(AssetStream stream)
		{
			Val = stream.ReadSingle();
			Name = stream.ReadStringAligned();
		}

		public bool IsZero => Val == 0.0f;
		public bool IsMax => Val == 255.0f;

		public float Val { get; private set; }
		public string Name { get; private set; }
	}
}
