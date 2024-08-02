﻿using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes.OcclusionCullingDatas
{
	public struct OcclusionScene : IAssetReadable, IYAMLExportable
	{
		public OcclusionScene(EngineGUID scene, int renderSize, int portalSize)
		{
			Scene = scene;
			IndexRenderers = 0;
			SizeRenderers = renderSize;
			IndexPortals = 0;
			SizePortals = portalSize;
		}

		public void Read(AssetStream stream)
		{
			IndexRenderers = stream.ReadInt32();
			SizeRenderers = stream.ReadInt32();
			IndexPortals = stream.ReadInt32();
			SizePortals = stream.ReadInt32();
			Scene.Read(stream);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("indexRenderers", IndexRenderers);
			node.Add("sizeRenderers", SizeRenderers);
			node.Add("indexPortals", IndexPortals);
			node.Add("sizePortals", SizePortals);
			node.Add("scene", Scene.ExportYAML(container));
			return node;
		}

		public int IndexRenderers { get; private set; }
		public int SizeRenderers { get; private set; }
		public int IndexPortals { get; private set; }
		public int SizePortals { get; private set; }

		public EngineGUID Scene;
	}
}