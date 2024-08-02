﻿using System;

namespace UtinyRipper
{
	[Flags]
	public enum TransferInstructionFlags : uint
	{
		NoTransferInstructionFlags				= 0x0,
		NeedsInstanceIDRemapping				= 0x1,
		AssetMetaDataOnly						= 0x2,
		YamlGlobalPPtrReference					= 0x4,
		Unknown1								= 0x8,
		Unknown2								= 0x10,
		IgnoreDebugPropertiesForIndex			= 0x20,
		/// <summary>
		/// Has this file been built for release game?
		/// </summary>
		SerializeGameRelease = 0x100,
		SwapEndianess							= 0x200,
		DontReadObjectsFromDiskBeforeWriting	= 0x800,
		SerializeMonoReload						= 0x1000,
		DontRequireAllMetaFlags					= 0x2000,
		/// <summary>
		/// Is prefab's format read?
		/// </summary>
		SerializeForPrefabSystem				= 0x4000,
		ThreadedSerialization					= 0x800000,
		IsBuiltinResourcesFile					= 0x1000000,
		PerformUnloadDependencyTracking			= 0x2000000,
		DisableWriteTypeTree					= 0x4000000,
		AutoreplaceEditorWindow					= 0x8000000,
		DontCreateMonoBehaviourScriptWrapper	= 0x10000000,
		SerializeForInspector					= 0x20000000,
		SerializedAssetBundleVersion			= 0x40000000,
		AllowTextSerialization					= 0x80000000,
	}

	public static class TransferInstructionFlagsExtensions
	{
		public static bool IsUnknown1(this TransferInstructionFlags _this)
		{
			return (_this & TransferInstructionFlags.Unknown1) != 0;
		}
		public static bool IsUnknown2(this TransferInstructionFlags _this)
		{
			return (_this & TransferInstructionFlags.Unknown2) != 0;
		}
		public static bool IsSerializeGameRelease(this TransferInstructionFlags _this)
		{
			return (_this & TransferInstructionFlags.SerializeGameRelease) != 0;
		}
		public static bool IsSerializeForPrefabSystem(this TransferInstructionFlags _this)
		{
			return (_this & TransferInstructionFlags.SerializeForPrefabSystem) != 0;
		}
		public static bool IsBuiltinResourcesFile(this TransferInstructionFlags _this)
		{
			return (_this & TransferInstructionFlags.IsBuiltinResourcesFile) != 0;
		}
		public static bool IsSerializeForInspector(this TransferInstructionFlags _this)
		{
			return (_this & TransferInstructionFlags.SerializeForInspector) != 0;
		}
	}
}
