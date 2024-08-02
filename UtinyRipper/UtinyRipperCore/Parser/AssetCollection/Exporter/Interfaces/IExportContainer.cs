﻿using UtinyRipper.AssetExporters.Classes;
using UtinyRipper.Classes;

namespace UtinyRipper.AssetExporters
{
	public interface IExportContainer
	{
		Object FindObject(int fileIndex, long pathID);
		ulong GetExportID(Object asset);
		AssetType ToExportType(ClassIDType classID);
		ExportPointer CreateExportPointer(Object @object);

		string SceneIDToString(int sceneID);
		string TagIDToString(int tagID);

		IExportCollection CurrentCollection { get; }
		Version Version { get; }
		Platform Platform { get; }
		TransferInstructionFlags Flags { get; }
	}
}
