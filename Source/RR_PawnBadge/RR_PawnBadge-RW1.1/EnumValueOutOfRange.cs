using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR_PawnBadge
{
	public class EnumValueOutOfRange : Exception
	{
		public EnumValueOutOfRange()
		{

		}

		public EnumValueOutOfRange(string message)
			: base(message)
		{

		}

		public EnumValueOutOfRange(string message, Exception inner)
			: base(message, inner)
		{

		}

		public EnumValueOutOfRange(Type type, object value, string paramName)
			: this($"Value of {paramName} ({value}) of type {type.FullName} is out of range of expected values.")
		{ }
	}
}
