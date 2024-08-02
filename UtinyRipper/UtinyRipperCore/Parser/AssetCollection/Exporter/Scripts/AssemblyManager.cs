﻿using System;
using System.IO;
using UtinyRipper.AssetExporters.Mono;
using UtinyRipper.Exporters.Scripts;

namespace UtinyRipper.AssetExporters
{
	public class AssemblyManager : IAssemblyManager
	{
		public AssemblyManager(Action<string> requestAssemblyCallback)
		{
			if (requestAssemblyCallback == null)
			{
				throw new ArgumentNullException(nameof(requestAssemblyCallback));
			}
			m_requestAssemblyCallback = requestAssemblyCallback;
		}

		public void Load(string filePath)
		{
			if (ScriptingBackEnd == ScriptingBackEnd.Unknown)
			{
				throw new Exception("You have to set backend first");
			}
			m_manager.Load(filePath);
		}

		public void Read(Stream stream, string filePath)
		{
			if (ScriptingBackEnd == ScriptingBackEnd.Unknown)
			{
				throw new Exception("You have to set backend first");
			}
			m_manager.Read(stream, filePath);
		}

		public void Unload(string fileName)
		{
			if (ScriptingBackEnd == ScriptingBackEnd.Unknown)
			{
				throw new Exception("You have to set backend first");
			}
			m_manager.Unload(fileName);
		}

		public static bool IsAssembly(string fileName)
		{
			if(MonoManager.IsMonoAssembly(fileName))
			{
				return true;
			}
			return false;
		}

		public bool IsPresent(string assembly, string name)
		{
			if (ScriptingBackEnd == ScriptingBackEnd.Unknown)
			{
				return false;
			}
			return m_manager.IsPresent(assembly, name);
		}

		public bool IsPresent(string assembly, string @namespace, string name)
		{
			if (ScriptingBackEnd == ScriptingBackEnd.Unknown)
			{
				return false;
			}
			return m_manager.IsPresent(assembly, @namespace, name);
		}

		public bool IsValid(string assembly, string name)
		{
			if (ScriptingBackEnd == ScriptingBackEnd.Unknown)
			{
				return false;
			}
			return m_manager.IsValid(assembly, name);
		}

		public bool IsValid(string assembly, string @namespace, string name)
		{
			if (ScriptingBackEnd == ScriptingBackEnd.Unknown)
			{
				return false;
			}
			return m_manager.IsValid(assembly, @namespace, name);
		}

		public ScriptStructure CreateStructure(string assembly, string name)
		{
			return m_manager.CreateStructure(assembly, name);
		}

		public ScriptStructure CreateStructure(string assembly, string @namespace, string name)
		{
			return m_manager.CreateStructure(assembly, @namespace, name);
		}

		public ScriptExportType CreateExportType(ScriptExportManager exportManager, string assembly, string name)
		{
			return m_manager.CreateExportType(exportManager, assembly, name);
		}
		public ScriptExportType CreateExportType(ScriptExportManager exportManager, string assembly, string @namespace, string name)
		{
			return m_manager.CreateExportType(exportManager, assembly, @namespace, name);
		}

		public void Dispose()
		{
			ScriptingBackEnd = ScriptingBackEnd.Unknown;
		}

		public ScriptingBackEnd ScriptingBackEnd
		{
			get => m_scriptingBackEnd;
			set
			{
				if(m_scriptingBackEnd == value)
				{
					return;
				}

				if(value == ScriptingBackEnd.Unknown)
				{
					if (m_scriptingBackEnd != ScriptingBackEnd.Unknown)
					{
						m_manager.Dispose();
						m_manager = null;
						m_scriptingBackEnd = value;
					}
					return;
				}

				if(m_scriptingBackEnd != ScriptingBackEnd.Unknown)
				{
					throw new Exception("Scripting backend is already set");
				}

				m_scriptingBackEnd = value;
				switch (value)
				{
					case ScriptingBackEnd.Mono:
						m_manager = new MonoManager(m_requestAssemblyCallback);
						break;

					default:
						throw new NotImplementedException($"Scripting backend {value} is not impelented yet");
				}
			}
		}

		private event Action<string> m_requestAssemblyCallback;

		private IAssemblyManager m_manager;
		private ScriptingBackEnd m_scriptingBackEnd;
	}
}
