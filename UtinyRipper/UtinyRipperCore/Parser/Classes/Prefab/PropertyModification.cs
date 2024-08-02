﻿using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes.Prefabs
{
	public struct PropertyModification : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetStream stream)
		{
			Target.Read(stream);
			PropertyPath = stream.ReadStringAligned();
			Value = stream.ReadStringAligned();
			ObjectReference.Read(stream);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("target", Target.ExportYAML(container));
			node.Add("propertyPath", PropertyPath);
			node.Add("value", Value);
			node.Add("objectReference", ObjectReference.ExportYAML(container));
			return node;
		}

		public string PropertyPath { get; private set; }
		public string Value { get; private set; }

		public PPtr<Object> Target;
		public PPtr<Object> ObjectReference;
	}
}
