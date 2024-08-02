﻿using Mono.Cecil;
using System.Collections.Generic;

namespace UtinyRipper.AssetExporters.Mono
{
	public class MonoStructure : ScriptStructure
	{
		public MonoStructure(TypeDefinition type):
			base(type.Namespace, type.Name, CreateBase(type), CreateFields(type))
		{
		}

		protected MonoStructure(MonoStructure copy) :
			base(copy)
		{
		}

		private static IScriptStructure CreateBase(TypeDefinition type)
		{
			if (MonoType.IsPrime(type.BaseType))
			{
				return null;
			}

			TypeDefinition definition = type.BaseType.Resolve();
			return new MonoStructure(definition);
		}

		private static IEnumerable<IScriptField> CreateFields(TypeDefinition type)
		{
			List<IScriptField> fields = new List<IScriptField>();
			foreach (FieldDefinition field in type.Fields)
			{
				if (field.HasConstant)
				{
					continue;
				}
				if (field.IsStatic)
				{
					continue;
				}
				if(field.IsInitOnly)
				{
					continue;
				}
				if (!MonoField.IsSerializableField(field))
				{
					continue;
				}

				MonoField monoField = new MonoField(field);
				fields.Add(monoField);
			}
			return fields;
		}

		public override IScriptStructure CreateCopy()
		{
			return new MonoStructure(this);
		}
	}
}
