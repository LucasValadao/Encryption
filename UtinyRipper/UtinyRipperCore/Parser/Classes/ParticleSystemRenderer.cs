﻿using System;
using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;
using UtinyRipper.SerializedFiles;

namespace UtinyRipper.Classes
{
	public sealed class ParticleSystemRenderer : Renderer
	{
		public ParticleSystemRenderer(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		/// <summary>
		/// 5.3.0 and greater
		/// </summary>
		public static bool IsReadMinParticleSize(Version version)
		{
			return version.IsGreaterEqual(5, 3);
		}
		/// <summary>
		/// 4.0.0 and greater
		/// </summary>
		public static bool IsReadNormalDirection(Version version)
		{
			return version.IsGreaterEqual(4);
		}
		/// <summary>
		/// 5.3.0 and greater
		/// </summary>
		public static bool IsReadRenderAlignment(Version version)
		{
			return version.IsGreaterEqual(5, 3);
		}
		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		public static bool IsReadUseCustomVertexStreams(Version version)
		{
			return version.IsGreaterEqual(5, 5);
		}
		/// <summary>
		/// 5.5.0 to 5.6.0 exclusive
		/// </summary>
		public static bool IsReadVertexStreamMask(Version version)
		{
			return version.IsGreaterEqual(5, 5) && version.IsLess(5, 6);
		}
		/// <summary>
		/// 5.6.0 and greater
		/// </summary>
		public static bool IsReadVertexStreams(Version version)
		{
			return version.IsGreaterEqual(5, 6);
		}
		
		/// <summary>
		/// 4.0.0 and greater
		/// </summary>
		public static bool IsReadMeshes(Version version)
		{
			return version.IsGreaterEqual(4);
		}
		/// <summary>
		/// 2017.1.0b2 and greater
		/// </summary>
		public static bool IsReadMaskInteraction(Version version)
		{
			return version.IsGreaterEqual(2017, 1, 0, VersionType.Beta, 2);
		}
		
		/// <summary>
		/// 5.3.0 and greater
		/// </summary>
		private static bool IsModeShort(Version version)
		{
			return version.IsGreaterEqual(5, 3);
		}
		/// <summary>
		/// 5.3.0 and greater
		/// </summary>
		private static bool IsSortModeFirst(Version version)
		{
			return version.IsGreaterEqual(5, 3);
		}

		private static int GetSerializedVersion(Version version)
		{
			if (Config.IsExportTopmostSerializedVersion)
			{
				return 4;
			}

			if (version.IsGreaterEqual(2017, 1, 0, VersionType.Beta, 2))
			{
				return 4;
			}
			if (version.IsGreaterEqual(5, 6))
			{
				return 3;
			}
			if (version.IsGreaterEqual(5, 5))
			{
				return 2;
			}
			return 1;
		}

		public override void Read(AssetStream stream)
		{
			base.Read(stream);

			if (IsModeShort(stream.Version))
			{
				RenderMode = stream.ReadUInt16();
			}
			else
			{
				RenderMode = stream.ReadInt32();
			}
			if (IsSortModeFirst(stream.Version))
			{
				SortMode = stream.ReadUInt16();
			}

			if (IsReadMinParticleSize(stream.Version))
			{
				MinParticleSize = stream.ReadSingle();
			}
			MaxParticleSize = stream.ReadSingle();
			CameraVelocityScale = stream.ReadSingle();
			VelocityScale = stream.ReadSingle();
			LengthScale = stream.ReadSingle();
			SortingFudge = stream.ReadSingle();

			if (IsReadNormalDirection(stream.Version))
			{
				NormalDirection = stream.ReadSingle();
			}
			if (!IsSortModeFirst(stream.Version))
			{
				SortMode = stream.ReadInt32();
			}

			if (IsReadRenderAlignment(stream.Version))
			{
				RenderAlignment = stream.ReadInt32();
				Pivot.Read(stream);
			}
			if (IsReadUseCustomVertexStreams(stream.Version))
			{
				UseCustomVertexStreams = stream.ReadBoolean();
				stream.AlignStream(AlignType.Align4);
			}
			if (IsReadVertexStreamMask(stream.Version))
			{
				VertexStreamMask = stream.ReadInt32();
			}
			if (IsReadVertexStreams(stream.Version))
			{
				m_vertexStreams = stream.ReadByteArray();
				stream.AlignStream(AlignType.Align4);
			}

			Mesh.Read(stream);
			if (IsReadMeshes(stream.Version))
			{
				Mesh1.Read(stream);
				Mesh2.Read(stream);
				Mesh3.Read(stream);
			}
			if (IsReadMaskInteraction(stream.Version))
			{
				MaskInteraction = stream.ReadInt32();
			}
		}

		public override IEnumerable<Object> FetchDependencies(ISerializedFile file, bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(file, isLog))
			{
				yield return @object;
			}
			
			yield return Mesh.FetchDependency(file, isLog, ToLogString, "m_Mesh");
			if (IsReadMeshes(file.Version))
			{
				yield return Mesh1.FetchDependency(file, isLog, ToLogString, "m_Mesh1");
				yield return Mesh2.FetchDependency(file, isLog, ToLogString, "m_Mesh2");
				yield return Mesh3.FetchDependency(file, isLog, ToLogString, "m_Mesh3");
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
#warning TODO: values acording to read version (current 2017.3.0f3)
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.InsertSerializedVersion(GetSerializedVersion(container.Version));
			node.Add("m_RenderMode", RenderMode);
			node.Add("m_SortMode", SortMode);
			node.Add("m_MinParticleSize", MinParticleSize);
			node.Add("m_MaxParticleSize", MaxParticleSize);
			node.Add("m_CameraVelocityScale", CameraVelocityScale);
			node.Add("m_VelocityScale", VelocityScale);
			node.Add("m_LengthScale", LengthScale);
			node.Add("m_SortingFudge", SortingFudge);
			node.Add("m_NormalDirection", NormalDirection);
			node.Add("m_RenderAlignment", RenderAlignment);
			node.Add("m_Pivot", Pivot.ExportYAML(container));
			node.Add("m_UseCustomVertexStreams", UseCustomVertexStreams);
			node.Add("m_VertexStreams", IsReadVertexStreams(container.Version) ? VertexStreams.ExportYAML() : YAMLScalarNode.Empty);
			node.Add("m_Mesh", Mesh.ExportYAML(container));
			node.Add("m_Mesh1", Mesh1.ExportYAML(container));
			node.Add("m_Mesh2", Mesh2.ExportYAML(container));
			node.Add("m_Mesh3", Mesh3.ExportYAML(container));
			node.Add("m_MaskInteraction", MaskInteraction);
			return node;
		}

		public int RenderMode { get; private set; }
		public int SortMode { get; private set; }
		public float MinParticleSize { get; private set; }
		public float MaxParticleSize { get; private set; }
		public float CameraVelocityScale { get; private set; }
		public float VelocityScale { get; private set; }
		public float LengthScale { get; private set; }
		public float SortingFudge { get; private set; }
		public float NormalDirection { get; private set; }
		public int RenderAlignment { get; private set; }
		public bool UseCustomVertexStreams { get; private set; }
		public int VertexStreamMask { get; private set; }
		public IReadOnlyList<byte> VertexStreams => m_vertexStreams;
		public int MaskInteraction { get; private set; }

		public Vector3f Pivot;
		public PPtr<Mesh> Mesh;
		public PPtr<Mesh> Mesh1;
		public PPtr<Mesh> Mesh2;
		public PPtr<Mesh> Mesh3;

		private byte[] m_vertexStreams;
	}
}
