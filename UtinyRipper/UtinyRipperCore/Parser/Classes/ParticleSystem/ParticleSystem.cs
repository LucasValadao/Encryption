﻿using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.Classes.ParticleSystems;
using UtinyRipper.Exporter.YAML;
using UtinyRipper.SerializedFiles;

namespace UtinyRipper.Classes
{
	public sealed class ParticleSystem : Component
	{
		public ParticleSystem(AssetInfo assetInfo):
			base(assetInfo)
		{
		}
		
		/// <summary>
		/// Less than 5.3.0
		/// </summary>
		public static bool IsReadStartDelaySingle(Version version)
		{
			return version.IsLess(5, 3);
		}
		/// <summary>
		/// 2017.2 and greater
		/// </summary>
		public static bool IsReadStopAction(Version version)
		{
			return version.IsGreaterEqual(2017, 2);
		}
		/// <summary>
		/// 2017.1.0b2 and greater
		/// </summary>
		public static bool IsReadUseUnscaledTime(Version version)
		{
			return version.IsGreaterEqual(2017, 1, 0, VersionType.Beta, 2);
		}
		/// <summary>
		/// 5.4.1 and greater
		/// </summary>
		public static bool IsReadAutoRandomSeed(Version version)
		{
			return version.IsGreaterEqual(5, 4, 0, VersionType.Patch, 4);
		}
		/// <summary>
		/// 2017.1.0f1 and greater
		/// </summary>
		public static bool IsReadUseRigidbodyForVelocity(Version version)
		{
			return version.IsGreaterEqual(2017, 1, 0, VersionType.Final);
		}
		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		public static bool IsReadMoveWithCustomTransform(Version version)
		{
			return version.IsGreaterEqual(5, 5);
		}
		/// <summary>
		/// 5.3.0 and greater
		/// </summary>
		public static bool IsReadScalingMode(Version version)
		{
			return version.IsGreaterEqual(5, 3);
		}
		
		/// <summary>
		/// 5.3.0 and greater
		/// </summary>
		public static bool IsReadInheritVelocityModule(Version version)
		{
			return version.IsGreaterEqual(5, 3);
		}
		/// <summary>
		/// 4.0.0 and greater
		/// </summary>
		public static bool IsReadExternalForcesModule(Version version)
		{
			return version.IsGreaterEqual(4);
		}
		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		public static bool IsReadNoiseModule(Version version)
		{
			return version.IsGreaterEqual(5, 5);
		}
		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		public static bool IsReadTriggerModule(Version version)
		{
			return version.IsGreaterEqual(5, 4);
		}
		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		public static bool IsReadLightsModule(Version version)
		{
			return version.IsGreaterEqual(5, 5);
		}
		/// <summary>
		/// 5.6.0 and greater
		/// </summary>
		public static bool IsReadCustomDataModule(Version version)
		{
			return version.IsGreaterEqual(5, 6);
		}
		
		/// <summary>
		/// Less than 5.5.0
		/// </summary>
		private static bool IsStartDelayFirst(Version version)
		{
			return version.IsLess(5, 5);
		}
		/// <summary>
		/// Less than 5.4.1
		/// </summary>
		private static bool IsRandomSeedFirst(Version version)
		{
			return version.IsLess(5, 4, 0, VersionType.Patch, 4);
		}
		/// <summary>
		/// Less than 5.5.0
		/// </summary>
		private static bool IsMoveWithTransformBool(Version version)
		{
			return version.IsLess(5, 5);
		}
		/// <summary>
		/// 5.4.1 and greater
		/// </summary>
		private static bool IsAlign(Version version)
		{
			return version.IsGreaterEqual(5, 4, 1);
		}
		
		private static int GetSerializedVersion(Version version)
		{
			if (Config.IsExportTopmostSerializedVersion)
			{
				return 5;
			}

			if (version.IsGreaterEqual(5, 5))
			{
				return 5;
			}
			if (version.IsGreaterEqual(5, 4, 0, VersionType.Patch, 4))
			{
				return 4;
			}
			// there is no 3rd version
			if (version.IsGreaterEqual(5, 3))
			{
				return 2;
			}
			return 1;
		}
		
		public override void Read(AssetStream stream)
		{
			base.Read(stream);
			
			LengthInSec = stream.ReadSingle();
			if (IsStartDelayFirst(stream.Version))
			{
				if (IsReadStartDelaySingle(stream.Version))
				{
					StartDelaySingle = stream.ReadSingle();
				}
				else
				{
					StartDelay.Read(stream);
				}
			}
			
			SimulationSpeed = stream.ReadSingle();
			if (IsReadStopAction(stream.Version))
			{
				StopAction = stream.ReadInt32();
			}

			if (IsRandomSeedFirst(stream.Version))
			{
				RandomSeed = unchecked((int)stream.ReadUInt32());
			}
			
			Looping = stream.ReadBoolean();
			Prewarm = stream.ReadBoolean();
			PlayOnAwake = stream.ReadBoolean();
			if (IsReadUseUnscaledTime(stream.Version))
			{
				UseUnscaledTime = stream.ReadBoolean();
			}
			if (IsMoveWithTransformBool(stream.Version))
			{
				MoveWithTransform = stream.ReadBoolean() ? 1 : 0;
			}
			if (IsReadAutoRandomSeed(stream.Version))
			{
				AutoRandomSeed = stream.ReadBoolean();
			}
			if (IsReadUseRigidbodyForVelocity(stream.Version))
			{
				UseRigidbodyForVelocity = stream.ReadBoolean();
			}
			if (IsAlign(stream.Version))
			{
				stream.AlignStream(AlignType.Align4);
			}

			if (!IsStartDelayFirst(stream.Version))
			{
				StartDelay.Read(stream);
				stream.AlignStream(AlignType.Align4);
			}
			if (!IsMoveWithTransformBool(stream.Version))
			{
				MoveWithTransform = stream.ReadInt32();
				stream.AlignStream(AlignType.Align4);
			}

			if (IsReadMoveWithCustomTransform(stream.Version))
			{
				MoveWithCustomTransform.Read(stream);
			}
			if (IsReadScalingMode(stream.Version))
			{
				ScalingMode = (ParticleSystemScalingMode)stream.ReadInt32();
			}
			if (!IsRandomSeedFirst(stream.Version))
			{
				RandomSeed = stream.ReadInt32();
			}

			InitialModule.Read(stream);
			ShapeModule.Read(stream);
			EmissionModule.Read(stream);
			SizeModule.Read(stream);
			RotationModule.Read(stream);
			ColorModule.Read(stream);
			UVModule.Read(stream);
			VelocityModule.Read(stream);
			if (IsReadInheritVelocityModule(stream.Version))
			{
				InheritVelocityModule.Read(stream);
			}
			ForceModule.Read(stream);
			if (IsReadExternalForcesModule(stream.Version))
			{
				ExternalForcesModule.Read(stream);
			}
			ClampVelocityModule.Read(stream);
			if (IsReadNoiseModule(stream.Version))
			{
				NoiseModule.Read(stream);
			}
			SizeBySpeedModule.Read(stream);
			RotationBySpeedModule.Read(stream);
			ColorBySpeedModule.Read(stream);
			CollisionModule.Read(stream);
			if (IsReadTriggerModule(stream.Version))
			{
				TriggerModule.Read(stream);
			}
			SubModule.Read(stream);
			if (IsReadLightsModule(stream.Version))
			{
				LightsModule.Read(stream);
				TrailModule.Read(stream);
			}
			if (IsReadCustomDataModule(stream.Version))
			{
				CustomDataModule.Read(stream);
			}
		}

		public override IEnumerable<Object> FetchDependencies(ISerializedFile file, bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(file, isLog))
			{
				yield return @object;
			}
			
			yield return MoveWithCustomTransform.FetchDependency(file, isLog, ToLogString, "moveWithCustomTransform");
			foreach(Object @object in CollisionModule.FetchDependencies(file, isLog))
			{
				yield return @object;
			}
			foreach(Object @object in SubModule.FetchDependencies(file, isLog))
			{
				yield return @object;
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
#warning TODO: values acording to read version (current 2017.3.0f3)
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.AddSerializedVersion(GetSerializedVersion(container.Version));
			node.Add("lengthInSec", LengthInSec);
			node.Add("simulationSpeed", SimulationSpeed);
			node.Add("stopAction", StopAction);
			node.Add("looping", Looping);
			node.Add("prewarm", Prewarm);
			node.Add("playOnAwake", PlayOnAwake);
			node.Add("useUnscaledTime", UseUnscaledTime);
			node.Add("autoRandomSeed", GetAutoRandomSeed(container.Version));
			node.Add("useRigidbodyForVelocity", GetUseRigidbodyForVelocity(container.Version));
			node.Add("startDelay", GetStartDelay(container.Version).ExportYAML(container));
			node.Add("moveWithTransform", MoveWithTransform);
			node.Add("moveWithCustomTransform", MoveWithCustomTransform.ExportYAML(container));
			node.Add("scalingMode", (int)GetScalingMode(container.Version));
			node.Add("randomSeed", RandomSeed);
			node.Add("InitialModule", InitialModule.ExportYAML(container));
			node.Add("ShapeModule", ShapeModule.ExportYAML(container));
			node.Add("EmissionModule", EmissionModule.ExportYAML(container));
			node.Add("SizeModule", SizeModule.ExportYAML(container));
			node.Add("RotationModule", RotationModule.ExportYAML(container));
			node.Add("ColorModule", ColorModule.ExportYAML(container));
			node.Add("UVModule", UVModule.ExportYAML(container));
			node.Add("VelocityModule", VelocityModule.ExportYAML(container));
			node.Add("InheritVelocityModule", GetInheritVelocityModule(container.Version).ExportYAML(container));
			node.Add("ForceModule", ForceModule.ExportYAML(container));
			node.Add("ExternalForcesModule", GetExternalForcesModule(container.Version).ExportYAML(container));
			node.Add("ClampVelocityModule", ClampVelocityModule.ExportYAML(container));
			node.Add("NoiseModule", GetNoiseModule(container.Version).ExportYAML(container));
			node.Add("SizeBySpeedModule", SizeBySpeedModule.ExportYAML(container));
			node.Add("RotationBySpeedModule", RotationBySpeedModule.ExportYAML(container));
			node.Add("ColorBySpeedModule", ColorBySpeedModule.ExportYAML(container));
			node.Add("CollisionModule", CollisionModule.ExportYAML(container));
			node.Add("TriggerModule", GetTriggerModule(container.Version).ExportYAML(container));
			node.Add("SubModule", SubModule.ExportYAML(container));
			node.Add("LightsModule", GetLightsModule(container.Version).ExportYAML(container));
			node.Add("TrailModule", GetTrailModule(container.Version).ExportYAML(container));
			node.Add("CustomDataModule", GetCustomDataModule(container.Version).ExportYAML(container));
			return node;
		}

		private bool GetAutoRandomSeed(Version version)
		{
			return IsReadAutoRandomSeed(version) ? AutoRandomSeed : true;
		}
		public bool GetUseRigidbodyForVelocity(Version version)
		{
			return IsReadUseRigidbodyForVelocity(version) ? UseRigidbodyForVelocity : true;
		}
		private MinMaxCurve GetStartDelay(Version version)
		{
			return IsReadStartDelaySingle(version) ? new MinMaxCurve(StartDelaySingle) : StartDelay;
		}
		private ParticleSystemScalingMode GetScalingMode(Version version)
		{
			return IsReadScalingMode(version) ? ScalingMode : ParticleSystemScalingMode.Local;
		}
		private InheritVelocityModule GetInheritVelocityModule(Version version)
		{
			return IsReadInheritVelocityModule(version) ? InheritVelocityModule : new InheritVelocityModule(true);
		}
		private ExternalForcesModule GetExternalForcesModule(Version version)
		{
			return IsReadExternalForcesModule(version) ? ExternalForcesModule : new ExternalForcesModule(true);
		}
		public NoiseModule GetNoiseModule(Version version)
		{
			return IsReadNoiseModule(version) ? NoiseModule : new NoiseModule(true);
		}
		public TriggerModule GetTriggerModule(Version version)
		{
			return IsReadTriggerModule(version) ? TriggerModule : new TriggerModule(true);
		}
		public LightsModule GetLightsModule(Version version)
		{
			return IsReadLightsModule(version) ? LightsModule : new LightsModule(true);
		}
		public TrailModule GetTrailModule(Version version)
		{
			return IsReadLightsModule(version) ? TrailModule : new TrailModule(true);
		}
		public CustomDataModule GetCustomDataModule(Version version)
		{
			return IsReadCustomDataModule(version) ? CustomDataModule : new CustomDataModule(true);
		}

		public float LengthInSec { get; private set; }
		public float StartDelaySingle { get; private set; }
		/// <summary>
		/// Speed previously
		/// </summary>
		public float SimulationSpeed { get; private set; }
		public int StopAction { get; private set; }
		public bool Looping { get; private set; }
		public bool Prewarm { get; private set; }
		public bool PlayOnAwake { get; private set; }
		public bool UseUnscaledTime { get; private set; }
		public bool AutoRandomSeed { get; private set; }
		public bool UseRigidbodyForVelocity { get; private set; }
		public int MoveWithTransform { get; private set; }
		public ParticleSystemScalingMode ScalingMode { get; private set; }
		public int RandomSeed { get; private set; }
		public InitialModule InitialModule { get; } = new InitialModule();
		public ShapeModule ShapeModule { get; } = new ShapeModule();
		public EmissionModule EmissionModule { get; } = new EmissionModule();
		public SizeModule SizeModule { get; } = new SizeModule();
		public RotationModule RotationModule { get; } = new RotationModule();
		public ColorModule ColorModule { get; } = new ColorModule();
		public UVModule UVModule { get; } = new UVModule();
		public VelocityModule VelocityModule { get; } = new VelocityModule();
		public InheritVelocityModule InheritVelocityModule { get; } = new InheritVelocityModule();
		public ForceModule ForceModule { get; } = new ForceModule();
		public ExternalForcesModule ExternalForcesModule { get; } = new ExternalForcesModule();
		public ClampVelocityModule ClampVelocityModule { get; } = new ClampVelocityModule();
		public NoiseModule NoiseModule { get; } = new NoiseModule();
		public SizeBySpeedModule SizeBySpeedModule { get; } = new SizeBySpeedModule();
		public RotationBySpeedModule RotationBySpeedModule { get; } = new RotationBySpeedModule();
		public ColorBySpeedModule ColorBySpeedModule { get; } = new ColorBySpeedModule();
		public CollisionModule CollisionModule { get; } = new CollisionModule();
		public TriggerModule TriggerModule { get; } = new TriggerModule();
		public SubModule SubModule { get; } = new SubModule();
		public LightsModule LightsModule { get; } = new LightsModule();
		public TrailModule TrailModule { get; } = new TrailModule();
		public CustomDataModule CustomDataModule { get; } = new CustomDataModule();
		
		public MinMaxCurve StartDelay;
		public PPtr<Transform> MoveWithCustomTransform;
	}
}
