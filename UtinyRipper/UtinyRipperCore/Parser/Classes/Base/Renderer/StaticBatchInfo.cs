﻿using System;
using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes.MeshRenderers
{
	public struct StaticBatchInfo : IAssetReadable, IYAMLExportable
	{
		public StaticBatchInfo(IReadOnlyList<uint> subsetIndices)
		{
			if (subsetIndices.Count == 0)
			{
				FirstSubMesh = 0;
				SubMeshCount = 0;
			}
			else
			{
				FirstSubMesh = (ushort)subsetIndices[0];
				SubMeshCount = (ushort)subsetIndices.Count;
				for (int i = 0, j = FirstSubMesh; i < SubMeshCount; i++, j++)
				{
					if (subsetIndices[i] != j)
					{
						throw new Exception("Can't create static batch info from subset indices");
					}
				}

			}
		}

		public void Read(AssetStream stream)
		{
			FirstSubMesh = stream.ReadUInt16();
			SubMeshCount = stream.ReadUInt16();
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("firstSubMesh", FirstSubMesh);
			node.Add("subMeshCount", SubMeshCount);
			return node;
		}

		public ushort FirstSubMesh { get; private set; }
		public ushort SubMeshCount { get; private set; }
	}
}
