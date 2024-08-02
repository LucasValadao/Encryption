﻿using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.SerializedFiles;

namespace UtinyRipper
{
	public interface IFileCollection
	{
		ISerializedFile GetSerializedFile(FileIdentifier fileRef);
		ISerializedFile FindSerializedFile(FileIdentifier fileRef);
		ResourcesFile FindResourcesFile(ISerializedFile file, string fileName);

		AssetFactory AssetFactory { get; }
		IReadOnlyList<ISerializedFile> Files { get; }
		IAssemblyManager AssemblyManager { get; }
	}
}
