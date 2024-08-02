﻿using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes.ParticleSystems
{
	public sealed class ExternalForcesModule : ParticleSystemModule
	{
		public ExternalForcesModule()
		{
		}

		public ExternalForcesModule(bool _)
		{
			Multiplier = 1.0f;
		}

		public override void Read(AssetStream stream)
		{
			base.Read(stream);
			
			Multiplier = stream.ReadSingle();
		}

		public override YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = (YAMLMappingNode)base.ExportYAML(container);
			node.Add("multiplier", Multiplier);
			return node;
		}

		public float Multiplier { get; private set; }
	}
}
