﻿using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes
{
	public struct BitField : IAssetReadable, IYAMLExportable
	{
		public BitField(uint bits)
		{
			Bits = bits;
		}

		/// <summary>
		/// Less than 2.0.0
		/// </summary>
		private static bool Is16Bits(Version version)
		{
			return version.IsLess(2);
		}

		private static int GetSerializedVersion(Version version)
		{
			if (Config.IsExportTopmostSerializedVersion)
			{
				return 2;
			}

			if (version.IsGreaterEqual(2))
			{
				return 2;
			}
			return 1;
		}
		
		public void Read(AssetStream stream)
		{
			Bits = Is16Bits(stream.Version) ? stream.ReadUInt16() : stream.ReadUInt32();
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(GetSerializedVersion(container.Version));
			node.Add("m_Bits", Bits);
			return node;
		}

		public uint Bits { get; private set; }
	}
}
