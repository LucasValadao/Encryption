﻿using System;
using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.Classes.Sprites;
using UtinyRipper.Exporter.YAML;
using UtinyRipper.SerializedFiles;

namespace UtinyRipper.Classes
{
	public sealed class Sprite : NamedObject
	{
		public Sprite(AssetInfo assetInfo) :
			base(assetInfo)
		{
		}

		/// <summary>
		/// 4.5.0 and greater
		/// </summary>
		public static bool IsReadBorder(Version version)
		{
			return version.IsGreaterEqual(4, 5);
		}
		/// <summary>
		/// 5.4.2 and greater
		/// </summary>
		public static bool IsReadPivot(Version version)
		{
			return version.IsGreaterEqual(5, 4, 2);
		}
		/// <summary>
		/// 5.3.0 and greater
		/// </summary>
		public static bool IsReadPolygon(Version version)
		{
			return version.IsGreaterEqual(5, 3);
		}
		/// <summary>
		/// 2017.1 and greater
		/// </summary>
		public static bool IsReadRendererData(Version version)
		{
			return version.IsGreaterEqual(2017);
		}
		/// <summary>
		/// 2017.1 and greater
		/// </summary>
		public static bool IsReadPhysicsShape(Version version)
		{
			return version.IsGreaterEqual(2017);
		}
		/// <summary>
		/// 2018.1 and greater
		/// </summary>
		public static bool IsReadBones(Version version)
		{
			return version.IsGreaterEqual(2018);
		}

		private static int GetSerializedVersion(Version version)
		{
			if(Config.IsExportTopmostSerializedVersion)
			{
				// return 2;
				return 1;
			}

			if(version.IsGreaterEqual(2018))
			{
				return 2;
			}
			return 1;
		}

		public IReadOnlyList<IReadOnlyList<Vector2f>> GenerateOutline(Rectf rect, Vector2f pivot)
		{
			Vector2f[][] outlines = RD.GenerateOutline(File.Version);
			Vector2f center = RD.TextureRect.Center;
			float pivotShiftX = rect.Width * pivot.X - rect.Width * 0.5f;
			float pivotShiftY = rect.Height * pivot.Y - rect.Height * 0.5f;
			Vector2f pivotShift = new Vector2f(pivotShiftX, pivotShiftY);
			foreach (Vector2f[] outline in outlines)
			{
				for (int i = 0; i < outline.Length; i++)
				{
					Vector2f point = outline[i] * PixelsToUnits;
					outline[i] = point + pivotShift;
				}
			}
			return outlines;
		}

		public IReadOnlyList<IReadOnlyList<Vector2f>> GeneratePhysicsShape()
		{
			if(IsReadPhysicsShape(File.Version))
			{
				Vector2f[][] shape = new Vector2f[PhysicsShape.Count][];
				Vector2f pivotShift = new Vector2f(Rect.Width * Pivot.X - Rect.Width * 0.5f, Rect.Height * Pivot.Y - Rect.Height * 0.5f);
				for (int i = 0; i < PhysicsShape.Count; i++)
				{
					shape[i] = new Vector2f[PhysicsShape[i].Count];
					for(int j = 0; j < PhysicsShape[i].Count; j++)
					{
						Vector2f point = PhysicsShape[i][j] * PixelsToUnits;
						shape[i][j] = point + pivotShift;
					}
				}
				return shape;
			}
			else
			{
				return new Vector2f[0][];
			}
		}

		public override void Read(AssetStream stream)
		{
			base.Read(stream);

			Rect.Read(stream);
			Offset.Read(stream);
			if(IsReadBorder(stream.Version))
			{
				Border.Read(stream);
			}
			PixelsToUnits = stream.ReadSingle();
			if(IsReadPivot(stream.Version))
			{
				Pivot.Read(stream);
			}
			Extrude = stream.ReadUInt32();
			if(IsReadPolygon(stream.Version))
			{
				IsPolygon = stream.ReadBoolean();
				stream.AlignStream(AlignType.Align4);
			}

			if(IsReadRendererData(stream.Version))
			{
				RenderDataKey = stream.ReadTupleTLong<EngineGUID>();
				m_atlasTags = stream.ReadStringArray();
				SpriteAtlas.Read(stream);
			}
			RD.Read(stream);
			stream.AlignStream(AlignType.Align4);

			if(IsReadPhysicsShape(stream.Version))
			{
				int count = stream.ReadInt32();
				m_physicsShape = new Vector2f[count][];
				for (int i = 0; i < count; i++)
				{
					m_physicsShape[i] = stream.ReadArray<Vector2f>();
				}
			}

			if(IsReadBones(stream.Version))
			{
				m_bones = stream.ReadArray<SpriteBone>();
			}
		}

		public override IEnumerable<Object> FetchDependencies(ISerializedFile file, bool isLog = false)
		{
			foreach (Object @object in base.FetchDependencies(file, isLog))
			{
				yield return @object;
			}
			if(!SpriteAtlas.IsNull)
			{
				yield return SpriteAtlas.FetchDependency(file, isLog, ToLogString, "SpriteAtlas");
			}
			foreach (Object @object in RD.FetchDependencies(file))
			{
				yield return @object;
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			throw new NotSupportedException();
		}

		public float PixelsToUnits { get; private set; }
		public uint Extrude { get; private set; }
		public bool IsPolygon { get; private set; }
		public IReadOnlyList<string> AtlasTags => m_atlasTags;
		public IReadOnlyList<IReadOnlyList<Vector2f>> PhysicsShape => m_physicsShape;
		public IReadOnlyList<SpriteBone> Bones => m_bones;

		public Rectf Rect;
		public Vector2f Offset;
		public Vector4f Border;
		public Vector2f Pivot;
		public Tuple<EngineGUID, long> RenderDataKey;
		public PPtr<SpriteAtlas> SpriteAtlas;
		public SpriteRenderData RD;

		private string[] m_atlasTags;
		private Vector2f[][] m_physicsShape;
		private SpriteBone[] m_bones;
	}
}
