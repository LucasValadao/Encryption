﻿using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes
{
	public struct XForm : IAssetReadable, IYAMLExportable
	{
		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		public static bool IsVector3(Version version)
		{
			return version.IsGreaterEqual(5, 4);
		}

		public void Read(AssetStream stream)
		{
			if(IsVector3(stream.Version))
			{
				T.Read3(stream);
			}
			else
			{
				T.Read(stream);
			}
			Q.Read(stream);
			if (IsVector3(stream.Version))
			{
				S.Read3(stream);
			}
			else
			{
				S.Read(stream);
			}
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("t", IsVector3(container.Version) ? T.ExportYAML3(container) : T.ExportYAML(container));
			node.Add("q", Q.ExportYAML(container));
			node.Add("s", IsVector3(container.Version) ? S.ExportYAML3(container) : S.ExportYAML(container));
			return node;
		}

		public Vector4f T;
		public Vector4f Q;
		public Vector4f S;
	}
}
