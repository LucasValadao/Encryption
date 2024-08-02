﻿using System;
using System.Collections.Generic;
using System.IO;
using UtinyRipper.Classes;

using Object = UtinyRipper.Classes.Object;

namespace UtinyRipper.AssetExporters
{
	public class BinaryAssetExporter : IAssetExporter
	{
		public bool IsHandle(Object asset)
		{
			return true;
		}

		public void Export(IExportContainer container, Object asset, string path)
		{
			Export(container, asset, path, null);
		}

		public void Export(IExportContainer container, Object asset, string path, Action<IExportContainer, Object, string> callback)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
			{
				asset.ExportBinary(container, fileStream);
			}
			callback?.Invoke(container, asset, path);
		}

		public void Export(IExportContainer container, IEnumerable<Object> assets, string path)
		{
			throw new NotSupportedException();
		}

		public void Export(IExportContainer container, IEnumerable<Object> assets, string path, Action<IExportContainer, Object, string> callback)
		{
			throw new NotSupportedException();
		}

		public IExportCollection CreateCollection(Object asset)
		{
			switch(asset.ClassID)
			{
				case ClassIDType.Sprite:
					return TextureExportCollection.CreateExportCollection(this, (Sprite)asset);

				case ClassIDType.Texture2D:
				case ClassIDType.Cubemap:
					return new TextureExportCollection(this, (Texture2D)asset);

				default:
					return new AssetExportCollection(this, asset);
			}
		}

		public AssetType ToExportType(Object asset)
		{
			ToUnknownExportType(asset.ClassID, out AssetType assetType);
			return assetType;
		}

		public bool ToUnknownExportType(ClassIDType classID, out AssetType assetType)
		{
			assetType = AssetType.Meta;
			return true;
		}
	}
}
