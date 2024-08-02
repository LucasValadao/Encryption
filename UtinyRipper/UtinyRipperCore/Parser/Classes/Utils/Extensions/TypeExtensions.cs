﻿using System;

namespace UtinyRipper.Classes
{
	public static class TypeExtensions
	{
		public static ClassIDType ToClassIDType(this Type _this)
		{
			if (Enum.TryParse(_this.Name, out ClassIDType classID))
			{
				return classID;
			}

			throw new Exception($"{_this} is not Engine class type");
		}
	}
}
