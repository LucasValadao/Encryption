﻿using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.Classes.LightmapSettingss;
using UtinyRipper.Exporter.YAML;
using UtinyRipper.SerializedFiles;

namespace UtinyRipper.Classes
{
	public sealed class LightmapSettings : LevelGameManager
	{
		public LightmapSettings(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		/// <summary>
		/// (5.0.0 and greater) and Release
		/// </summary>
		public static bool IsReadEnlightenSceneMapping(Version version, TransferInstructionFlags flags)
		{
			return version.IsGreaterEqual(5) && flags.IsSerializeGameRelease();
		}
		/// <summary>
		/// 5.0.0 and greater and Not Release
		/// </summary>
		public static bool IsReadGIWorkflowMode(Version version, TransferInstructionFlags flags)
		{
			return version.IsGreaterEqual(5) && !flags.IsSerializeGameRelease();
		}
		/// <summary>
		/// 3.5.0 and greater and (Release or Resource)
		/// </summary>
		public static bool IsReadLightProbes(Version version, TransferInstructionFlags flags)
		{
			return version.IsGreaterEqual(3, 5) && (flags.IsSerializeGameRelease() || flags.IsBuiltinResourcesFile());
		}
		/// <summary>
		/// Release or Resource
		/// </summary>
		public static bool IsReadLightmaps(TransferInstructionFlags flags)
		{
			return flags.IsSerializeGameRelease() || flags.IsBuiltinResourcesFile();
		}
		/// <summary>
		/// 3.0.0 and greater and Release
		/// </summary>
		public static bool IsReadLightmapsMode(Version version, TransferInstructionFlags flags)
		{
			return version.IsGreaterEqual(3) && flags.IsSerializeGameRelease();
		}
		/// <summary>
		/// 3.2.0 to 5.0.0 exclusive
		/// </summary>
		public static bool IsReadUseDualLightmapsInForward(Version version)
		{
			return version.IsGreaterEqual(3, 2) && version.IsLess(5);
		}
		/// <summary>
		/// 3.5.0 to 5.0.0 exclusive
		/// </summary>
		public static bool IsReadBakedColorSpace(Version version)
		{
			return version.IsGreaterEqual(3, 5) && version.IsLess(5);
		}
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public static bool IsReadGISettings(Version version)
		{
			return version.IsGreaterEqual(5);
		}		
		/// <summary>
		/// Not Release
		/// </summary>
		public static bool IsReadLightmapEditorSettings(Version version, TransferInstructionFlags flags)
		{
#warning unknown version (random)
			return version.IsGreaterEqual(2017) && !flags.IsSerializeGameRelease();
		}
		/// <summary>
		/// Not Release
		/// </summary>
		public static bool IsReadLightingDataAsset(Version version, TransferInstructionFlags flags)
		{
#warning unknown version (random)
			return version.IsGreaterEqual(2017) && !flags.IsSerializeGameRelease();
		}
		/// <summary>
		/// 5.0.0 to  exclusive
		/// </summary>
		public static bool IsReadRuntimeCPUUsage(Version version)
		{
			return version.IsGreaterEqual(5) && version.IsLessEqual(5, 6, 0, VersionType.Beta, 6);
		}
		/// <summary>
		/// 5.6.0b2 and greater
		/// </summary>
		public static bool IsReadUseShadowmask(Version version)
		{
			return version.IsGreaterEqual(5, 6, 0, VersionType.Beta, 2);
		}

		/// <summary>
		/// 2017.1 and greater
		/// </summary>
		private static bool IsBoolShadowmask(Version version)
		{
			return version.IsGreaterEqual(2017);
		}

		/// <summary>
		/// 2017.1 and (Release or Resource)
		/// </summary>
		private static bool IsAlign1(Version version, TransferInstructionFlags flags)
		{
			return version.IsGreaterEqual(2017) && (flags.IsSerializeGameRelease() || flags.IsBuiltinResourcesFile());
		}
		/// <summary>
		/// 3.2.0 and greater
		/// </summary>
		private static bool IsAlign2(Version version)
		{
			return version.IsGreaterEqual(3, 2);
		}

		private static int GetSerializedVersion(Version version)
		{
			if (Config.IsExportTopmostSerializedVersion)
			{
				return 11;
			}

#warning unknown
			if (version.IsGreaterEqual(2017, 0, 0, VersionType.Beta))
			{
				return 11;
			}
			if (version.IsGreaterEqual(2017))
			{
				return 10;
			}
			if (version.IsGreaterEqual(5, 6, 0, VersionType.Beta, 2))
			{
				return 9;
			}
#warning unknown
			/*if (version.IsGreaterEqual(5, 6, 0, VersionType.Beta, ))
			{
				return 8;
			}*/
			if (version.IsGreaterEqual(5, 4))
			{
				return 7;
			}
			if (version.IsGreaterEqual(5, 3))
			{
				return 6;
			}
#warning unknown
			if (version.IsGreater(5, 0, 0, VersionType.Beta))
			{
				return 5;
			}
#warning unknown
			if (version.IsGreater(5, 0, 0, VersionType.Beta, 1))
			{
				return 4;
			}
#warning unknown
			if (version.IsGreaterEqual(5, 0, 0, VersionType.Beta))
			{
				return 3;
			}
			if (version.IsGreaterEqual(5))
			{
				return 2;
			}
			return 1;
		}

		public override void Read(AssetStream stream)
		{
			base.Read(stream);

			if(IsReadEnlightenSceneMapping(stream.Version, stream.Flags))
			{
				EnlightenSceneMapping.Read(stream);
			}
			if(IsReadGIWorkflowMode(stream.Version, stream.Flags))
			{
				GIWorkflowMode = stream.ReadInt32();
			}

			if (IsReadLightProbes(stream.Version, stream.Flags))
			{
				LightProbes.Read(stream);
			}
			if(IsReadLightmaps(stream.Flags))
			{
				m_lightmaps = stream.ReadArray<LightmapData>();
			}
			if (IsAlign1(stream.Version, stream.Flags))
			{
				stream.AlignStream(AlignType.Align4);
			}

			if(IsReadLightmapsMode(stream.Version, stream.Flags))
			{
				LightmapsMode = (LightmapsMode)stream.ReadInt32();
			}
			if (IsReadBakedColorSpace(stream.Version))
			{
				BakedColorSpace = stream.ReadInt32();
			}
			if (IsReadUseDualLightmapsInForward(stream.Version))
			{
				UseDualLightmapsInForward = stream.ReadBoolean();
			}
			if (IsAlign2(stream.Version))
			{
				stream.AlignStream(AlignType.Align4);
			}

			if (IsReadGISettings(stream.Version))
			{
				GISettings.Read(stream);
			}

			if (IsReadLightmapEditorSettings(stream.Version, stream.Flags))
			{
				LightmapEditorSettings.Read(stream);
			}
			if(IsReadLightingDataAsset(stream.Version, stream.Flags))
			{
				LightingDataAsset.Read(stream);
			}
			if(IsReadRuntimeCPUUsage(stream.Version))
			{
				RuntimeCPUUsage = stream.ReadInt32();
			}
			if(IsReadUseShadowmask(stream.Version))
			{
				if(IsBoolShadowmask(stream.Version))
				{
					UseShadowmask = stream.ReadBoolean();
				}
				else
				{
					UseShadowmask = stream.ReadInt32() == 0 ? false : true;
				}
			}
		}

		public override IEnumerable<Object> FetchDependencies(ISerializedFile file, bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(file, isLog))
			{
				yield return @object;
			}

			if (IsReadEnlightenSceneMapping(file.Version, file.Flags))
			{
				foreach (Object @object in EnlightenSceneMapping.FetchDependencies(file, isLog))
				{
					yield return @object;
				}
			}
			if (IsReadLightProbes(file.Version, file.Flags))
			{
				yield return LightProbes.FetchDependency(file, isLog, ToLogString, "m_LightProbes");
				foreach (LightmapData lightmap in Lightmaps)
				{
					foreach (Object @object in lightmap.FetchDependencies(file, isLog))
					{
						yield return @object;
					}
				}
			}
			if (IsReadLightmapEditorSettings(file.Version, file.Flags))
			{
				foreach (Object @object in LightmapEditorSettings.FetchDependencies(file, isLog))
				{
					yield return @object;
				}
			}
			if (IsReadLightingDataAsset(file.Version, file.Flags))
			{
				yield return LightingDataAsset.FetchDependency(file, isLog, ToLogString, "m_LightingDataAsset");
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
#warning TODO: values acording to read version (current 2017.3.0f3)
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.AddSerializedVersion(GetSerializedVersion(container.Version));
			node.Add("m_GIWorkflowMode", GetExportGIWorkflowMode(container.Version, container.Flags));
			node.Add("m_GISettings", GetExportGISettings(container.Version).ExportYAML(container));
			node.Add("m_LightmapEditorSettings", GetExportLightmapEditorSettings(container.Version, container.Flags).ExportYAML(container));
#warning is that possible to somehow create LightingDataAsset with Release data?
			node.Add("m_LightingDataAsset", LightingDataAsset.ExportYAML(container));
			node.Add("m_UseShadowmask", GetExportUseShadowmask(container.Version));
			return node;
		}

		private int GetExportGIWorkflowMode(Version version, TransferInstructionFlags flags)
		{
			return IsReadGIWorkflowMode(version, flags) ? GIWorkflowMode : 1;
		}
		private GISettings GetExportGISettings(Version version)
		{
			return IsReadGISettings(version) ? GISettings : new GISettings(true);
		}
		private LightmapEditorSettings GetExportLightmapEditorSettings(Version version, TransferInstructionFlags flags)
		{
			return IsReadLightmapEditorSettings(version, flags) ? LightmapEditorSettings : new LightmapEditorSettings(true);
		}
		private bool GetExportUseShadowmask(Version version)
		{
			return IsReadUseShadowmask(version) ? UseShadowmask : true;
		}

		public int GIWorkflowMode { get; private set; }
		public IReadOnlyList<LightmapData> Lightmaps => m_lightmaps;
		public LightmapsMode LightmapsMode { get; private set; }
		public int BakedColorSpace { get; private set; }
		public bool UseDualLightmapsInForward { get; private set; }
		public int RuntimeCPUUsage { get; private set; }
		/// <summary>
		/// ShadowMaskMode previously
		/// </summary>
		public bool UseShadowmask { get; private set; }

		public EnlightenSceneMapping EnlightenSceneMapping;
		public PPtr<LightProbes> LightProbes;
		public GISettings GISettings;
		public LightmapEditorSettings LightmapEditorSettings;
		public PPtr<LightingDataAsset> LightingDataAsset;

		private LightmapData[] m_lightmaps;
	}
}
