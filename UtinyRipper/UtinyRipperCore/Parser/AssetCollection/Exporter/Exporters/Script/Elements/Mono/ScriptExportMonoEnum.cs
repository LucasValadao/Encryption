﻿using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace UtinyRipper.Exporters.Scripts.Mono
{
	public sealed class ScriptExportMonoEnum : ScriptExportEnum
	{
		public ScriptExportMonoEnum(TypeReference @enum)
		{
			if (@enum == null)
			{
				throw new ArgumentNullException(nameof(@enum));
			}
			
			Type = @enum;
			if (@enum.Module != null)
			{
				Definition = @enum.Resolve();
			}

			m_module = ScriptExportMonoType.GetModule(Type);
			m_fullName = ScriptExportManager.ToFullName(Module, Type.FullName);
		}

		public override void Init(IScriptExportManager manager)
		{
			if (Type.Module == null)
			{
				m_fields = new ScriptExportField[0];
			}
			else
			{
				m_fields = CreateFields(manager);
			}

			if (Type.IsNested)
			{
				m_declaringType = manager.RetrieveType(Type.DeclaringType);
				AddAsNestedEnum(manager);
			}
		}

		private IReadOnlyList<ScriptExportField> CreateFields(IScriptExportManager manager)
		{
			List<ScriptExportField> fields = new List<ScriptExportField>();
			foreach (FieldDefinition field in Definition.Fields)
			{
				if (field.Name == "value__")
				{
					continue;
				}

				ScriptExportField enumField = manager.RetrieveField(field);
				fields.Add(enumField);
			}
			return fields;
		}

		public override string Name => Type.Name;
		public override string FullName => m_fullName;
		public override string Namespace => Type.Namespace;
		public override string Module => m_module;

		public override ScriptExportType DeclaringType => m_declaringType;
		public override IReadOnlyList<ScriptExportField> Fields => m_fields;

		protected override string Keyword
		{
			get
			{
				if (Definition == null)
				{
					return PublicKeyWord;
				}

				if (Definition.IsPublic || Definition.IsNestedPublic)
				{
					return PublicKeyWord;
				}
				if (Definition.IsNestedPrivate)
				{
					return PrivateKeyWord;
				}
				if (Definition.IsNestedFamily)
				{
					return ProtectedKeyWord;
				}
				return InternalKeyWord;
			}
		}

		private TypeReference Type { get; }
		private TypeDefinition Definition { get; }

		private ScriptExportType m_declaringType;
		private IReadOnlyList<ScriptExportField> m_fields;
		private string m_fullName;
		private string m_module;
	}
}
