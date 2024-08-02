﻿using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes
{
	public struct DateTime : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetStream stream)
		{
			HighSeconds = stream.ReadUInt16();
			Fraction = stream.ReadUInt16();
			LowSeconds = stream.ReadUInt32();
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("highSeconds", HighSeconds);
			node.Add("fraction", Fraction);
			node.Add("lowSeconds", LowSeconds);
			return node;
		}

		public ushort HighSeconds { get; private set; }
		public ushort Fraction { get; private set; }
		public uint LowSeconds { get; private set; }
	}
}
