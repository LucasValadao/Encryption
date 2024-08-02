﻿using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes
{
	public sealed class OcclusionPortal : Component
	{
		public OcclusionPortal(AssetInfo assetInfo):
			base(assetInfo)
		{
		}
		
		public override void Read(AssetStream stream)
		{
			base.Read(stream);

			Open = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
			
			Center.Read(stream);
			Size.Read(stream);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.Add("m_Open", Open);
			node.Add("m_Center", Center.ExportYAML(container));
			node.Add("m_Size", Size.ExportYAML(container));
			return node;
		}

		public bool Open { get; private set; }

		public Vector3f Center;
		public Vector3f Size;
	}
}
