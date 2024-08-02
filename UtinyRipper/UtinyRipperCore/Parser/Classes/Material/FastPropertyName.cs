﻿using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes.Materials
{
	public struct FastPropertyName : IAssetReadable, IYAMLExportable
	{
		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		private static bool IsPlainString(Version version)
		{
			return version.IsGreaterEqual(2017, 3);
		}

		public void Read(AssetStream stream)
		{
			Value = stream.ReadStringAligned();
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
			//if(IsPlainString)
			{
				return new YAMLScalarNode(Value);
			}
			/*else
			{
				YAMLMappingNode node = new YAMLMappingNode();
				node.Add("name", Value);
				return node;
			}*/
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public string Value { get; private set; }		
	}
}
