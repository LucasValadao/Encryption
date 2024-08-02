﻿using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;
using UtinyRipper.SerializedFiles;

namespace UtinyRipper.Classes.GUIStyles
{
	public struct GUIStyleState
	{
		public GUIStyleState(bool _)
		{
			Background = default;
			TextColor = default;
			m_scaledBackgrounds = new PPtr<Texture2D>[0];
		}

		public GUIStyleState(GUIStyleState copy)
		{
			Background = copy.Background;
			TextColor = new ColorRGBAf(copy.TextColor);
			m_scaledBackgrounds = new PPtr<Texture2D>[copy.ScaledBackgrounds.Count];
			for(int i = 0; i < copy.ScaledBackgrounds.Count; i++)
			{
				m_scaledBackgrounds[i] = new PPtr<Texture2D>(copy.ScaledBackgrounds[i]);
			}
		}

		public void Read(AssetStream stream)
		{
			Background.Read(stream);
			m_scaledBackgrounds = new PPtr<Texture2D>[0];
			//m_scaledBackgrounds = stream.ReadArray<PPtr<Texture2D>>();
			TextColor.Read(stream);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_Background", Background.ExportYAML(container));
			node.Add("m_ScaledBackgrounds", m_scaledBackgrounds.ExportYAML(container));
			node.Add("m_TextColor", TextColor.ExportYAML(container));
			return node;
		}

		public IEnumerable<Object> FetchDependencies(ISerializedFile file, bool isLog = false)
		{
			yield break;
		}
		
		public IReadOnlyList<PPtr<Texture2D>> ScaledBackgrounds => m_scaledBackgrounds;

		public PPtr<Texture2D> Background;
		public ColorRGBAf TextColor;

		private PPtr<Texture2D>[] m_scaledBackgrounds;
	}
}
