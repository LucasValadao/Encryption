﻿using System.Collections.Generic;
using UtinyRipper.SerializedFiles;

namespace UtinyRipper.Classes
{
	/// <summary>
	/// CubemapTexture previously
	/// </summary>
	public sealed class Cubemap : Texture2D
	{
		public Cubemap(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		/// <summary>
		/// 4.0.0 and greater
		/// </summary>
		public static bool IsReadSourceTextures(Version version)
		{
			return version.IsGreaterEqual(4);
		}

		public override void Read(AssetStream stream)
		{
			base.Read(stream);

			if (IsReadSourceTextures(stream.Version))
			{
				m_sourceTextures = stream.ReadArray<PPtr<Texture2D>>();
				stream.AlignStream(AlignType.Align4);
			}
		}

		public override IEnumerable<Object> FetchDependencies(ISerializedFile file, bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(file, isLog))
			{
				yield return @object;
			}

			if (IsReadSourceTextures(file.Version))
			{
				foreach(PPtr<Texture2D> texture in m_sourceTextures)
				{
					yield return texture.FetchDependency(file, isLog, ToLogString, "sourceTextures");
				}
			}
		}

		public IReadOnlyList<PPtr<Texture2D>> SourceTextures => m_sourceTextures;

		private PPtr<Texture2D>[] m_sourceTextures;
	}
}
