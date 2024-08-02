﻿using System;
using System.Collections.Generic;

namespace UtinyRipper.Classes.AnimatorControllers
{
	public struct ControllerConstant : IAssetReadable
	{		
		public void Read(AssetStream stream)
		{
			m_layerArray = stream.ReadArray<OffsetPtr<LayerConstant>>();
			m_stateMachineArray = stream.ReadArray<OffsetPtr<StateMachineConstant>>();
			Values.Read(stream);
			DefaultValues.Read(stream);
		}

		public LayerConstant GetLayerByStateMachineIndex(int index)
		{
			foreach(OffsetPtr<LayerConstant> layerPtr in LayerArray)
			{
				LayerConstant layer = layerPtr.Instance;
				if(layer.StateMachineIndex == index && layer.StateMachineMotionSetIndex == 0)
				{
					return layer;
				}
			}
			throw new ArgumentOutOfRangeException(nameof(index));
		}
		
		public int GetLayerIndexByStateMachineIndex(int index)
		{
			for (int i = 0; i < LayerArray.Count; i++)
			{
				LayerConstant layer = LayerArray[i].Instance;
				if (layer.StateMachineIndex == index && layer.StateMachineMotionSetIndex == 0)
				{
					return i;
				}
			}
			throw new ArgumentOutOfRangeException(nameof(index));
		}

		public int GetLayerIndex(LayerConstant layer)
		{
			for(int i = 0; i < LayerArray.Count; i++)
			{
				LayerConstant checkLayer = LayerArray[i].Instance;
				if(checkLayer.StateMachineIndex == layer.StateMachineIndex && checkLayer.StateMachineMotionSetIndex == layer.StateMachineMotionSetIndex)
				{
					return i;
				}
			}
			throw new ArgumentException("Layer wasn't found", nameof(layer));
		}

		public IReadOnlyList<OffsetPtr<LayerConstant>> LayerArray => m_layerArray;
		public IReadOnlyList<OffsetPtr<StateMachineConstant>> StateMachineArray => m_stateMachineArray;

		public OffsetPtr<ValueArrayConstant> Values;
		public OffsetPtr<ValueArray> DefaultValues;

		/// <summary>
		/// m_HumanLayerArray previously
		/// </summary>
		private OffsetPtr<LayerConstant>[] m_layerArray;
		private OffsetPtr<StateMachineConstant>[] m_stateMachineArray;
	}
}
