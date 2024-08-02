﻿using System;
using UtinyRipper.AssetExporters;

namespace UtinyRipper.Classes
{
	public sealed class PrefabPtr : InnerPPtr<Prefab>
	{
		public PrefabPtr(Prefab prefab)
		{
			if (prefab == null)
			{
				throw new ArgumentNullException(nameof(prefab));
			}
			m_prefab = prefab;
		}

		protected override ulong GetPathID(IExportContainer container)
		{
			return container.GetExportID(m_prefab);
		}

		private readonly Prefab m_prefab;
	}
}
