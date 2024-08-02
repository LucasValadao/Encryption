﻿using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes.NavMeshDatas
{
	public struct AutoOffMeshLinkData : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetStream stream)
		{
			Start.Read(stream);
			End.Read(stream);
			Radius = stream.ReadSingle();
			LinkType = stream.ReadUInt16();
			Area = stream.ReadByte();
			LinkDirection = stream.ReadByte();
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_Start", Start.ExportYAML(container));
			node.Add("m_End", End.ExportYAML(container));
			node.Add("m_Radius", Radius);
			node.Add("m_LinkType", LinkType);
			node.Add("m_Area", Area);
			node.Add("m_LinkDirection", LinkDirection);
			return node;
		}

		public float Radius { get; private set; }
		public ushort LinkType { get; private set; }
		public byte Area { get; private set; }
		public byte LinkDirection { get; private set; }

		public Vector3f Start;
		public Vector3f End;
	}
}
