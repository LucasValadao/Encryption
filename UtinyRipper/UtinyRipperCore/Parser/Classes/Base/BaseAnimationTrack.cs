﻿using System;
using System.IO;
using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes
{
	public abstract class BaseAnimationTrack : NamedObject
	{
		protected BaseAnimationTrack(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		public sealed override void ExportBinary(IExportContainer container, Stream stream)
		{
			throw new NotSupportedException();
		}

		protected sealed override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			throw new NotImplementedException();
		}
	}
}