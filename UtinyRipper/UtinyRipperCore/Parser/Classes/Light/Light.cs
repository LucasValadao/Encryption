﻿using System;
using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.Classes.Lights;
using UtinyRipper.Exporter.YAML;
using UtinyRipper.SerializedFiles;

namespace UtinyRipper.Classes
{
	public sealed class Light : Behaviour
	{
		/// <summary>
		/// Less than 3.0.0
		/// </summary>
		/// <param name="version"></param>
		/// <returns></returns>
		public static bool IsReadAttenuate(Version version)
		{
			return version.IsLess(3);
		}
		/// <summary>
		/// 2.0.0 and greater
		/// </summary>
		public static bool IsReadIntensity(Version version)
		{
			return version.IsGreaterEqual(2);
		}
		/// <summary>
		/// 3.0.0 and greater
		/// </summary>
		public static bool IsReadCookieSize(Version version)
		{
			return version.IsGreaterEqual(3);
		}
		/// <summary>
		/// 2.0.0 and greater
		/// </summary>
		public static bool IsReadShadows(Version version)
		{
			return version.IsGreaterEqual(2);
		}
		/// <summary>
		/// 3.0.0 to 5.4.0 excludsive
		/// </summary>
		public static bool IsReadActuallyLightmapped(Version version)
		{
			return version.IsGreaterEqual(3) && version.IsLess(5, 4);
		}
		/// <summary>
		/// 5.4.0 to 5.6.0 exclusive
		/// </summary>
		public static bool IsReadBakedIndex(Version version)
		{
			return version.IsGreaterEqual(5, 4) && version.IsLess(5, 6);
		}
		/// <summary>
		/// 5.6.0 and greater
		/// </summary>
		public static bool IsReadBakingOutput(Version version)
		{
			return version.IsGreaterEqual(5, 6);
		}
		/// <summary>
		/// 1.5.0 and greater
		/// </summary>
		public static bool IsReadCullingMask(Version version)
		{
			return version.IsGreaterEqual(1, 5);
		}
		/// <summary>
		/// 3.0.0 and greater
		/// </summary>
		public static bool IsReadLightmapping(Version version)
		{
			return version.IsGreaterEqual(3);
		}
		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		public static bool IsReadAreaSize(Version version)
		{
			return version.IsGreaterEqual(5, 4);
		}
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public static bool IsReadBounceIntensity(Version version)
		{
			return version.IsGreaterEqual(5);
		}
		/// <summary>
		/// 2017.1.0
		/// </summary>
		public static bool IsReadFalloffTable(Version version)
		{
			return version.IsEqual(2017, 1, 0);
		}
		/// <summary>
		/// 5.6.0 and greater
		/// </summary>
		public static bool IsReadColorTemperature(Version version)
		{
			return version.IsGreaterEqual(5, 6);
		}
		/// <summary>
		/// 2.1.0 and greater
		/// </summary>
		private static bool IsAlign(Version version)
		{
			return version.IsGreaterEqual(2, 1);
		}
		
		private static int GetSerializedVersion(Version version)
		{
			if (Config.IsExportTopmostSerializedVersion)
			{
				return 8;
			}

			if (version.IsGreaterEqual(5, 6))
			{
				return 8;
			}
			if (version.IsGreaterEqual(5, 4))
			{
				return 7;
			}
			if (version.IsGreaterEqual(5, 0, 0, VersionType.Final, 4))
			{
				return 6;
			}
			if (version.IsEqual(5, 0, 0, VersionType.Beta, 1))
			{
				return 4;
			}
			if (version.IsGreaterEqual(5))
			{
				throw new NotSupportedException($"Version {version} isn't supported");
			}
			if (version.IsGreaterEqual(3))
			{
				return 3;
			}
			if (version.IsGreaterEqual(2))
			{
				return 2;
			}
			return 1;
		}

		public Light(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		public override void Read(AssetStream stream)
		{
			base.Read(stream);

			Type = (LightType)stream.ReadInt32();
			Color.Read(stream);
			if (IsReadAttenuate(stream.Version))
			{
				Attenuate = stream.ReadBoolean();
				if (IsAlign(stream.Version))
				{
					stream.AlignStream(AlignType.Align4);
				}
			}
			if (IsReadIntensity(stream.Version))
			{
				Intensity = stream.ReadSingle();
			}
			Range = stream.ReadSingle();
			SpotAngle = stream.ReadSingle();
			if (IsReadCookieSize(stream.Version))
			{
				CookieSize = stream.ReadSingle();
			}
			if (IsReadShadows(stream.Version))
			{
				Shadows.Read(stream);
			}
			Cookie.Read(stream);
			DrawHalo = stream.ReadBoolean();
			if (IsReadActuallyLightmapped(stream.Version))
			{
				ActuallyLightmapped = stream.ReadBoolean();
			}
			if (IsAlign(stream.Version))
			{
				stream.AlignStream(AlignType.Align4);
			}

			if (IsReadBakedIndex(stream.Version))
			{
				BakedIndex = stream.ReadInt32();
			}
			if (IsReadBakingOutput(stream.Version))
			{
				BakingOutput.Read(stream);
			}
			Flare.Read(stream);
			RenderMode = (LightRenderMode)stream.ReadInt32();
			if (IsReadCullingMask(stream.Version))
			{
				CullingMask.Read(stream);
			}
			if (IsReadLightmapping(stream.Version))
			{
				Lightmapping = (LightmappingMode)stream.ReadInt32();
			}
			if (IsReadAreaSize(stream.Version))
			{
				AreaSize.Read(stream);
			}
			if (IsReadBounceIntensity(stream.Version))
			{
				BounceIntensity = stream.ReadSingle();
			}
			if(IsReadFalloffTable(stream.Version))
			{
				FalloffTable.Read(stream);
			}
			if (IsReadColorTemperature(stream.Version))
			{
				ColorTemperature = stream.ReadSingle();
				UseColorTemperature = stream.ReadBoolean();
				stream.AlignStream(AlignType.Align4);
			}
		}

		public override IEnumerable<Object> FetchDependencies(ISerializedFile file, bool isLog = false)
		{
			foreach (Object @object in base.FetchDependencies(file, isLog))
			{
				yield return @object;
			}
			
			yield return Cookie.FetchDependency(file, isLog, ToLogString, "m_Cookie");
			yield return Flare.FetchDependency(file, isLog, ToLogString, "m_Flare");
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
			YAMLMappingNode node =  base.ExportYAMLRoot(container);
			node.AddSerializedVersion(GetSerializedVersion(container.Version));
			node.Add("m_Type", (int)Type);
			node.Add("m_Color", Color.ExportYAML(container));
			node.Add("m_Intensity", Intensity);
			node.Add("m_Range", Range);
			node.Add("m_SpotAngle", SpotAngle);
			node.Add("m_CookieSize", CookieSize);
			node.Add("m_Shadows", Shadows.ExportYAML(container));
			node.Add("m_Cookie", Cookie.ExportYAML(container));
			node.Add("m_DrawHalo", DrawHalo);
			node.Add("m_Flare", Flare.ExportYAML(container));
			node.Add("m_RenderMode", (int)RenderMode);
			node.Add("m_CullingMask", CullingMask.ExportYAML(container));
			node.Add("m_Lightmapping", (int)Lightmapping);
			node.Add("m_AreaSize", AreaSize.ExportYAML(container));
			node.Add("m_BounceIntensity", BounceIntensity);
			node.Add("m_ColorTemperature", ColorTemperature);
			node.Add("m_UseColorTemperature", UseColorTemperature);
#warning ???
			node.Add("m_ShadowRadius", 0);
			node.Add("m_ShadowAngle", 0);
			return node;
		}

		public LightType Type { get; private set; }
		public bool Attenuate { get; private set; }
		public float Intensity { get; private set; }
		public float Range { get; private set; }
		public float SpotAngle { get; private set; }
		public float CookieSize { get; private set; }
		public bool DrawHalo { get; private set; }
		public bool ActuallyLightmapped { get; private set; }
		public int BakedIndex { get; private set; }
		public LightRenderMode RenderMode { get; private set; }
		public LightmappingMode Lightmapping { get; private set; }
		/// <summary>
		/// IndirectIntensity in 5.0.0beta
		/// </summary>
		public float BounceIntensity { get; private set; }
		public float ColorTemperature { get; private set; }
		public bool UseColorTemperature { get; private set; }

		public ColorRGBAf Color;
		public ShadowSettings Shadows;
		public PPtr<Texture> Cookie;
		public LightBakingOutput BakingOutput;
		public PPtr<Flare> Flare;
		public BitField CullingMask;
		public Vector2f AreaSize;
		public FalloffTable FalloffTable;
	}
}
