﻿namespace UtinyRipper.Classes.Shaders
{
	public struct MatrixParameter : IAssetReadable
	{
		public MatrixParameter(string name, ShaderParamType type, int index, int rowCount)
		{
			Name = name;
			NameIndex = -1;
			Index = index;
			ArraySize = 0;
			Type = type;
			RowCount = (byte)rowCount;
		}

		public MatrixParameter(string name, ShaderParamType type, int index, int arraySize, int rowCount) :
			this(name, type, index, rowCount)
		{
			ArraySize = arraySize;
		}

		public void Read(AssetStream stream)
		{
			NameIndex = stream.ReadInt32();
			Index = stream.ReadInt32();
			ArraySize = stream.ReadInt32();
			Type = (ShaderParamType)stream.ReadByte();
			RowCount = stream.ReadByte();
			stream.AlignStream(AlignType.Align4);
		}

		public string Name { get; private set; }
		public int NameIndex { get; private set; }
		public int Index { get; private set; }
		public int ArraySize { get; private set; }
		public ShaderParamType Type { get; private set; }
		public byte RowCount { get; private set; }
	}
}
