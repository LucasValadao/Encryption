﻿using UtinyRipper.AssetExporters;
using UtinyRipper.Classes.BoxCollider2Ds;
using UtinyRipper.Classes.PolygonCollider2Ds;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes
{
	public sealed class PolygonCollider2D : Collider2D
	{
		public PolygonCollider2D(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		/// <summary>
		/// 2017.1 and greater
		/// </summary>
		public static bool IsReadSpriteTilingProperty(Version version)
		{
			return version.IsGreaterEqual(2017);
		}

		public override void Read(AssetStream stream)
		{
			base.Read(stream);
			
			if (IsReadSpriteTilingProperty(stream.Version))
			{
				SpriteTilingProperty.Read(stream);
				AutoTiling = stream.ReadBoolean();
				stream.AlignStream(AlignType.Align4);
			}
			Points.Read(stream);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.Add("m_SpriteTilingProperty", SpriteTilingProperty.ExportYAML(container));
			node.Add("m_AutoTiling", AutoTiling);
			node.Add("m_Points", Points.ExportYAML(container));
			return node;
		}

		public bool AutoTiling { get; private set; }

		public SpriteTilingProperty SpriteTilingProperty;
		/// <summary>
		/// Poly previously
		/// </summary>
		public Polygon2D Points;
	}
}
