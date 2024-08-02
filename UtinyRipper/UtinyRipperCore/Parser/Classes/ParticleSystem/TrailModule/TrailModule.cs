﻿using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes.ParticleSystems
{
	public sealed class TrailModule : ParticleSystemModule
	{
		public TrailModule()
		{
		}

		public TrailModule(bool _)
		{
			Ratio = 1.0f;
			Lifetime = new MinMaxCurve(1.0f, 1.0f, 1.0f, 1.0f);
			MinVertexDistance = 0.2f;
			RibbonCount = 1;
			DieWithParticles = true;
			SizeAffectsWidth = true;
			InheritParticleColor = true;
			ColorOverLifetime = new MinMaxGradient(true);
			WidthOverTrail = new MinMaxCurve(1.0f, 1.0f, 1.0f, 1.0f);
			ColorOverTrail = new MinMaxGradient(true);
		}

		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		public static bool IsReadMode(Version version)
		{
			return version.IsGreaterEqual(2017, 3);
		}
		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		public static bool IsReadRibbonCount(Version version)
		{
			return version.IsGreaterEqual(2017, 3);
		}
		/// <summary>
		/// 2017.1.0b2
		/// </summary>
		public static bool IsReadGenerateLightingData(Version version)
		{
			return version.IsGreaterEqual(2017, 1, 0, VersionType.Beta, 2);
		}
		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		public static bool IsReadSplitSubEmitterRibbons(Version version)
		{
			return version.IsGreaterEqual(2017, 3);
		}

		private int GetExportRibbonCount(Version version)
		{
			return IsReadRibbonCount(version) ? RibbonCount : 1;
		}

		public override void Read(AssetStream stream)
		{
			base.Read(stream);

			if (IsReadMode(stream.Version))
			{
				Mode = (ParticleSystemTrailMode)stream.ReadInt32();
			}
			Ratio = stream.ReadSingle();
			Lifetime.Read(stream);
			MinVertexDistance = stream.ReadSingle();
			TextureMode = stream.ReadInt32();
			if (IsReadRibbonCount(stream.Version))
			{
				RibbonCount = stream.ReadInt32();
			}
			WorldSpace = stream.ReadBoolean();
			DieWithParticles = stream.ReadBoolean();
			SizeAffectsWidth = stream.ReadBoolean();
			SizeAffectsLifetime = stream.ReadBoolean();
			InheritParticleColor = stream.ReadBoolean();
			if (IsReadGenerateLightingData(stream.Version))
			{
				GenerateLightingData = stream.ReadBoolean();
			}
			if (IsReadSplitSubEmitterRibbons(stream.Version))
			{
				SplitSubEmitterRibbons = stream.ReadBoolean();
			}
			stream.AlignStream(AlignType.Align4);
			
			ColorOverLifetime.Read(stream);
			WidthOverTrail.Read(stream);
			ColorOverTrail.Read(stream);
		}

		public override YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = (YAMLMappingNode)base.ExportYAML(container);
			node.Add("mode", (int)Mode);
			node.Add("ratio", Ratio);
			node.Add("lifetime", Lifetime.ExportYAML(container));
			node.Add("minVertexDistance", MinVertexDistance);
			node.Add("textureMode", TextureMode);
			node.Add("ribbonCount", GetExportRibbonCount(container.Version));
			node.Add("worldSpace", WorldSpace);
			node.Add("dieWithParticles", DieWithParticles);
			node.Add("sizeAffectsWidth", SizeAffectsWidth);
			node.Add("sizeAffectsLifetime", SizeAffectsLifetime);
			node.Add("inheritParticleColor", InheritParticleColor);
			node.Add("generateLightingData", GenerateLightingData);
			node.Add("splitSubEmitterRibbons", SplitSubEmitterRibbons);
			node.Add("colorOverLifetime", ColorOverLifetime.ExportYAML(container));
			node.Add("widthOverTrail", WidthOverTrail.ExportYAML(container));
			node.Add("colorOverTrail", ColorOverTrail.ExportYAML(container));
			return node;
		}

		public ParticleSystemTrailMode Mode { get; private set; }
		public float Ratio { get; private set; }
		public float MinVertexDistance { get; private set; }
		public int TextureMode { get; private set; }
		public int RibbonCount { get; private set; }
		public bool WorldSpace { get; private set; }
		public bool DieWithParticles { get; private set; }
		public bool SizeAffectsWidth { get; private set; }
		public bool SizeAffectsLifetime { get; private set; }
		public bool InheritParticleColor { get; private set; }
		public bool GenerateLightingData { get; private set; }
		public bool SplitSubEmitterRibbons { get; private set; }

		public MinMaxCurve Lifetime;
		public MinMaxGradient ColorOverLifetime;
		public MinMaxCurve WidthOverTrail;
		public MinMaxGradient ColorOverTrail;
	}
}
