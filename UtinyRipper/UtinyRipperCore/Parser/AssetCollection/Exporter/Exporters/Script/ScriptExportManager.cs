﻿using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UtinyRipper.Exporters.Scripts.Mono;

namespace UtinyRipper.Exporters.Scripts
{
	public sealed class ScriptExportManager : IScriptExportManager
	{
		public ScriptExportManager(string exportPath)
		{
			if(string.IsNullOrEmpty(exportPath))
			{
				throw new ArgumentNullException(nameof(exportPath));
			}
			m_exportPath = exportPath;
		}

		public static string ToFullName(string module, string fullname)
		{
			return $"[{module}]{fullname}";
		}

		private static string GetExportSubPath(string assembly, string @namespace, string @class)
		{
			string assFolderName = Path.GetFileNameWithoutExtension(assembly);
			string nsFolder = @namespace.Replace('.', Path.DirectorySeparatorChar);
			string finalFolderPath = Path.Combine(assFolderName, nsFolder);
			string filePath = Path.Combine(finalFolderPath, @class);
			return $"{filePath}.cs";
		}

		private static string GetExportSubPath(ScriptExportType type)
		{
			string typeName = type.Name;
			int index = typeName.IndexOf('<');
			if(index >= 0)
			{
				typeName = typeName.Substring(0, index);
				typeName += $".{typeName.Count(t => t == ',') + 1}";
			}
			return GetExportSubPath(type.Module, type.Namespace, typeName);
		}

		public ScriptExportType CreateExportType(TypeReference type)
		{
			ScriptExportType exportType = RetrieveType(type);
			return exportType;
		}

		public string Export(ScriptExportType exportType)
		{
			if (exportType.DeclaringType != null)
			{
				throw new NotSupportedException("You can export only topmost types");
			}

			if (IsBuiltInType(exportType))
			{
				return null;
			}

			string subPath = GetExportSubPath(exportType);
			string filePath = Path.Combine(m_exportPath, subPath);
			string directory = Path.GetDirectoryName(filePath);
			if(!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			using (FileStream file = File.OpenWrite(filePath))
			{
				using (StreamWriter writer = new StreamWriter(file))
				{
					exportType.Export(writer);
				}
			}
			AddExportedType(exportType);
			return filePath;
		}

		public void ExportRest()
		{
			foreach (ScriptExportType type in m_types.Values)
			{
				if(type.DeclaringType != null)
				{
					continue;
				}
				if (m_exported.Contains(type.FullName))
				{
					continue;
				}

				Export(type);
			}

			foreach (ScriptExportEnum @enum in m_enums.Values)
			{
				if(@enum.DeclaringType != null)
				{
					continue;
				}
				if (m_exported.Contains(@enum.FullName))
				{
					continue;
				}

				Export(@enum);
			}

			foreach (ScriptExportDelegate @delegate in m_delegates.Values)
			{
				if (@delegate.DeclaringType != null)
				{
					continue;
				}
				if (m_exported.Contains(@delegate.FullName))
				{
					continue;
				}

				Export(@delegate);
			}
		}
		
		public ScriptExportType RetrieveType(TypeReference type)
		{
			if (type.IsArray)
			{
				return RetrieveArray(type);
			}
			if (type.IsGenericInstance)
			{
				return RetrieveGeneric(type);
			}

			if (type.Module != null)
			{
				TypeDefinition definition = type.Resolve();
				if(definition != null)
				{
					if (definition.IsEnum)
					{
						return RetrieveEnum(definition);
					}
					if (ScriptExportMonoDelegate.IsDelegate(definition))
					{
						return RetrieveDelegate(definition);
					}
				}
			}

			string fullname = ScriptExportMonoType.ToFullName(type);
			if (m_types.TryGetValue(fullname, out ScriptExportType exportType))
			{
				return exportType;
			}
			return CreateType(type);
		}

		public ScriptExportArray RetrieveArray(TypeReference array)
		{
			string fullname = ScriptExportMonoType.ToFullName(array);
			if (m_arrays.TryGetValue(fullname, out ScriptExportArray exportArray))
			{
				return exportArray;
			}
			return CreateArray(array);
		}

		public ScriptExportGeneric RetrieveGeneric(TypeReference generic)
		{
			string fullname = ScriptExportMonoType.ToFullName(generic);
			if (m_generic.TryGetValue(fullname, out ScriptExportGeneric exportGeneric))
			{
				return exportGeneric;
			}
			return CreateGeneric(generic);
		}

		public ScriptExportEnum RetrieveEnum(TypeDefinition @enum)
		{
			string fullname = ScriptExportMonoType.ToFullName(@enum);
			if (m_enums.TryGetValue(fullname, out ScriptExportEnum exportEnum))
			{
				return exportEnum;
			}
			return CreateEnum(@enum);
		}

		public ScriptExportDelegate RetrieveDelegate(TypeDefinition @delegate)
		{
			string fullname = ScriptExportMonoType.ToFullName(@delegate);
			if (m_delegates.TryGetValue(fullname, out ScriptExportDelegate exportDelegate))
			{
				return exportDelegate;
			}
			return CreateDelegate(@delegate);
		}

		public ScriptExportAttribute RetrieveAttribute(CustomAttribute attribute)
		{
			string fullname = ScriptExportMonoAttribute.ToFullName(attribute);
			if (m_attributes.TryGetValue(fullname, out ScriptExportAttribute exportAttribute))
			{
				return exportAttribute;
			}
			return CreateAttribute(attribute);
		}

		public ScriptExportField RetrieveField(FieldDefinition field)
		{
			ScriptExportField exportField = new ScriptExportMonoField(field);
			exportField.Init(this);
			return exportField;
		}

		public ScriptExportParameter RetrieveParameter(ParameterDefinition parameter)
		{
			ScriptExportParameter exportParameter = new ScriptExportMonoParameter(parameter);
			exportParameter.Init(this);
			return exportParameter;
		}

		private ScriptExportType CreateType(TypeReference type)
		{
			ScriptExportType exportType = new ScriptExportMonoType(type);
			m_types.Add(exportType.FullName, exportType);
			exportType.Init(this);
			return exportType;
		}

		public ScriptExportArray CreateArray(TypeReference type)
		{
			ScriptExportArray exportArray = new ScriptExportMonoArray(type);
			m_arrays.Add(exportArray.FullName, exportArray);
			exportArray.Init(this);
			return exportArray;
		}

		public ScriptExportGeneric CreateGeneric(TypeReference type)
		{
			ScriptExportGeneric exportGeneric = new ScriptExportMonoGeneric(type);
			m_generic.Add(exportGeneric.FullName, exportGeneric);
			exportGeneric.Init(this);
			return exportGeneric;
		}

		private ScriptExportEnum CreateEnum(TypeReference @enum)
		{
			ScriptExportMonoEnum exportEnum = new ScriptExportMonoEnum(@enum);
			m_enums.Add(exportEnum.FullName, exportEnum);
			exportEnum.Init(this);
			return exportEnum;
		}

		private ScriptExportDelegate CreateDelegate(TypeDefinition @delegate)
		{
			ScriptExportMonoDelegate exportDelegate = new ScriptExportMonoDelegate(@delegate);
			m_delegates.Add(exportDelegate.FullName, exportDelegate);
			exportDelegate.Init(this);
			return exportDelegate;
		}

		private ScriptExportAttribute CreateAttribute(CustomAttribute attribute)
		{
			ScriptExportMonoAttribute exportAttribute = new ScriptExportMonoAttribute(attribute);
			m_attributes.Add(exportAttribute.FullName, exportAttribute);
			exportAttribute.Init(this);
			return exportAttribute;
		}

		private void AddExportedType(ScriptExportType exportType)
		{
			m_exported.Add(exportType.FullName);
			foreach (ScriptExportEnum nestedEnum in exportType.NestedEnums)
			{
				m_exported.Add(nestedEnum.FullName);
			}
			foreach (ScriptExportDelegate @delegate in exportType.Delegates)
			{
				m_exported.Add(@delegate.FullName);
			}

			foreach (ScriptExportType nestedType in exportType.NestedTypes)
			{
				AddExportedType(nestedType);
			}
		}

		private static bool IsBuiltInType(ScriptExportType type)
		{
			if (IsDotNetLibrary(type.Module))
			{
				return true;
			}
			if (IsUnityLibrary(type.Module))
			{
				//return true;
			}

			return false;
		}

		private static bool IsDotNetLibrary(string module)
		{
			switch(module)
			{
				case MSCoreLibName:
				case SystemName:
					return true;

				default:
					return module.StartsWith($"{SystemName}.", StringComparison.Ordinal);
			}
		}

		private static bool IsUnityLibrary(string module)
		{
			switch (module)
			{
				case UnityEngineName:
				case BooName:
					return true;

				default:
					{
						if (module.StartsWith($"{UnityEngineName}.", StringComparison.Ordinal))
						{
							return true;
						}
						if (module.StartsWith($"{MonoName}.", StringComparison.Ordinal))
						{
							return true;
						}
						return false;
					}
			}
		}

		public IEnumerable<ScriptExportType> Types => m_types.Values;
		public IEnumerable<ScriptExportEnum> Enums => m_enums.Values;
		public IEnumerable<ScriptExportDelegate> Delegates => m_delegates.Values;

		private const string MSCoreLibName = "mscorlib";
		private const string SystemName = "System";
		private const string UnityEngineName = "UnityEngine";
		private const string BooName = "Boo";
		private const string MonoName = "Mono";

		private readonly Dictionary<string, ScriptExportType> m_types = new Dictionary<string, ScriptExportType>();
		private readonly Dictionary<string, ScriptExportArray> m_arrays = new Dictionary<string, ScriptExportArray>();
		private readonly Dictionary<string, ScriptExportGeneric> m_generic = new Dictionary<string, ScriptExportGeneric>();
		private readonly Dictionary<string, ScriptExportEnum> m_enums = new Dictionary<string, ScriptExportEnum>();
		private readonly Dictionary<string, ScriptExportDelegate> m_delegates = new Dictionary<string, ScriptExportDelegate>();
		private readonly Dictionary<string, ScriptExportAttribute> m_attributes = new Dictionary<string, ScriptExportAttribute>();

		private readonly HashSet<string> m_exported = new HashSet<string>();

		private readonly string m_exportPath;
	}
}
