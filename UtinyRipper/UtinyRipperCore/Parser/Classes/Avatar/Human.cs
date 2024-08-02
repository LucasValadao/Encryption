﻿using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes.Avatars
{
	public struct Human : IAssetReadable, IYAMLExportable
	{
		/// <summary>
		/// Less than 2018.2
		/// </summary>
		public static bool IsReadHandles(Version version)
		{
			return version.IsLess(2018, 2);
		}
		/// <summary>
		/// Less than 2018.2
		/// </summary>
		public static bool IsReadColliderIndex(Version version)
		{
			return version.IsLess(2018, 2);
		}
		/// <summary>
		/// 5.2.0 and greater
		/// </summary>
		public static bool IsReadHasTDoF(Version version)
		{
			return version.IsGreaterEqual(5, 2);
		}

		private static int GetSerializedVersion(Version version)
		{
			if (Config.IsExportTopmostSerializedVersion)
			{
				return 2;
			}

			if (version.IsGreaterEqual(5, 6, 1))
			{
				return 2;
			}
			return 1;
		}

		private IReadOnlyList<Handle> GetExportHandles(Version version)
		{
			return IsReadHandles(version) ? Handles : new Handle[0];
		}
		private IReadOnlyList<Collider> GetExportColliderArray(Version version)
		{
			return IsReadHandles(version) ? ColliderArray : new Collider[0];
		}
		private IReadOnlyList<int> GetExportColliderIndex(Version version)
		{
			return IsReadColliderIndex(version) ? ColliderIndex : new int[0];
		}

		public void Read(AssetStream stream)
		{
			RootX.Read(stream);
			Skeleton.Read(stream);
			SkeletonPose.Read(stream);
			LeftHand.Read(stream);
			RightHand.Read(stream);
			if(IsReadHandles(stream.Version))
			{
				m_handles = stream.ReadArray<Handle>();
				m_colliderArray = stream.ReadArray<Collider>();
			}
			m_humanBoneIndex = stream.ReadInt32Array();
			m_humanBoneMass = stream.ReadSingleArray();
			if (IsReadColliderIndex(stream.Version))
			{
				m_colliderIndex = stream.ReadInt32Array();
			}
			Scale = stream.ReadSingle();
			ArmTwist = stream.ReadSingle();
			ForeArmTwist = stream.ReadSingle();
			UpperLegTwist = stream.ReadSingle();
			LegTwist = stream.ReadSingle();
			ArmStretch = stream.ReadSingle();
			LegStretch = stream.ReadSingle();
			FeetSpacing = stream.ReadSingle();
			HasLeftHand = stream.ReadBoolean();
			HasRightHand = stream.ReadBoolean();
			if (IsReadHasTDoF(stream.Version))
			{
				HasTDoF = stream.ReadBoolean();
			}
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
#warning TODO: values acording to read version (current 2017.3.0f3)
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(GetSerializedVersion(container.Version));
			node.Add("m_RootX", RootX.ExportYAML(container));
			node.Add("m_Skeleton", Skeleton.ExportYAML(container));
			node.Add("m_SkeletonPose", SkeletonPose.ExportYAML(container));
			node.Add("m_LeftHand", LeftHand.ExportYAML(container));
			node.Add("m_RightHand", RightHand.ExportYAML(container));
			node.Add("m_Handles", GetExportHandles(container.Version).ExportYAML(container));
			node.Add("m_ColliderArray", GetExportColliderArray(container.Version).ExportYAML(container));
			node.Add("m_HumanBoneIndex", HumanBoneIndex.ExportYAML(true));
			node.Add("m_HumanBoneMass", HumanBoneMass.ExportYAML());
			node.Add("m_ColliderIndex", GetExportColliderIndex(container.Version).ExportYAML(true));
			node.Add("m_Scale", Scale);
			node.Add("m_ArmTwist", ArmTwist);
			node.Add("m_ForeArmTwist", ForeArmTwist);
			node.Add("m_UpperLegTwist", UpperLegTwist);
			node.Add("m_LegTwist", LegTwist);
			node.Add("m_ArmStretch", ArmStretch);
			node.Add("m_LegStretch", LegStretch);
			node.Add("m_FeetSpacing", FeetSpacing);
			node.Add("m_HasLeftHand", HasLeftHand);
			node.Add("m_HasRightHand", HasRightHand);
			node.Add("m_HasTDoF", HasTDoF);
			return node;
		}

		public IReadOnlyList<Handle> Handles => m_handles;
		public IReadOnlyList<Collider> ColliderArray => m_colliderArray;
		public IReadOnlyList<int> HumanBoneIndex => m_humanBoneIndex;
		public IReadOnlyList<float> HumanBoneMass => m_humanBoneMass;
		public IReadOnlyList<int> ColliderIndex => m_colliderIndex;
		public float Scale { get; private set; }
		public float ArmTwist { get; private set; }
		public float ForeArmTwist { get; private set; }
		public float UpperLegTwist { get; private set; }
		public float LegTwist { get; private set; }
		public float ArmStretch { get; private set; }
		public float LegStretch { get; private set; }
		public float FeetSpacing { get; private set; }
		public bool HasLeftHand { get; private set; }
		public bool HasRightHand { get; private set; }
		public bool HasTDoF { get; private set; }

		public XForm RootX;
		public OffsetPtr<Skeleton> Skeleton;
		public OffsetPtr<SkeletonPose> SkeletonPose;
		public OffsetPtr<Hand> LeftHand;
		public OffsetPtr<Hand> RightHand;
		
		private Handle[] m_handles;
		private Collider[] m_colliderArray;
		private int[] m_humanBoneIndex;
		private float[] m_humanBoneMass;
		private int[] m_colliderIndex;
	}
}
