﻿using UtinyRipper.Classes;

namespace UtinyRipper.SerializedFiles
{
	/// <summary>
	/// A serialized file may be linked with other serialized files to create shared dependencies.
	/// </summary>
	public class FileIdentifier : ISerializedFileReadable
	{
		/// <summary>
		/// 2.1.0 and greater
		/// </summary>
		public static bool IsReadAssetName(FileGeneration generation)
		{
			return generation >= FileGeneration.FG_210_261;
		}
		/// <summary>
		/// 1.2.0 and greater
		/// </summary>
		public static bool IsReadHash(FileGeneration generation)
		{
			return generation >= FileGeneration.FG_120_200;
		}

		public void Read(SerializedFileStream stream)
		{
			if (IsReadAssetName(stream.Generation))
			{
				AssetPath = stream.ReadStringZeroTerm();
			}
			if (IsReadHash(stream.Generation))
			{
				Hash.Read(stream);
				Type = (AssetType)stream.ReadInt32();
			}
			FilePath = stream.ReadStringZeroTerm();
			FilePathFixed = PathUtils.FixResourcePath(FilePath);
		}

		public bool IsFile(ISerializedFile file)
		{
			return file.Name == FilePathFixed;
		}

		public override string ToString()
		{
			return FilePath ?? base.ToString();
		}

		/// <summary>
		/// Virtual asset path. Used for cached files, otherwise it's empty.
		/// The file with that path usually doesn't exist, so it's probably an alias.
		/// </summary>
		public string AssetPath { get; private set; }
		/// <summary>
		/// The type of the file
		/// </summary>
		public AssetType Type { get; private set; }
		/// <summary>
		/// Actual file path. This path is relative to the path of the current file.
		/// The folder "library" often needs to be translated to "resources" in order to find the file on the file system.
		/// </summary>
		public string FilePath { get; private set; }
		/// <summary>
		/// File path without such constructions as archive:/directory/fileName
		/// </summary>
		public string FilePathFixed { get; private set; }

		/// <summary>
		/// Globally unique identifier of the file (or Hash?), 16 bytes long.
		/// Engine apparently always uses the big endian format and when converted to text,
		/// the GUID is a simple 32 character hex string with swapped characters for each byte.
		/// </summary>
		public Hash128 Hash;
	}
}
