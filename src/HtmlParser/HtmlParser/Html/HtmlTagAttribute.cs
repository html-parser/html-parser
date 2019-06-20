using System;

namespace HtmlParser
{
	/// <summary>
	/// Represents single attribute of HTML Tag.
	/// </summary>
	public class HtmlTagAttribute
	{
		public string UnparsedLiteral;
		
		public string Name;
		public string Value;

		public void ParseFromUnparsedLiteral()
		{
			throw new NotImplementedException();
		}
	}
}
