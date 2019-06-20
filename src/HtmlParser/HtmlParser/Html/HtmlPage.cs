using System;
using System.Collections.Generic;

namespace HtmlParser
{
	/// <summary>
	/// Represents whole HTML page.
	/// </summary>
	public class HtmlPage
	{
		public string UnparsedLiteral;

		public List<HtmlTag> Tags = new List<HtmlTag>();
		
		/// <summary>
		/// Fills this HtmlPage's Tags list by parsing UnparsedLiteral field value
		/// </summary>
		public void ParseFromUnparsedLiteral()
		{
			if (UnparsedLiteral != null && UnparsedLiteral.Length > 0)
			{
				List<string> tagsStrings = Serialize.SplitByRootTags(UnparsedLiteral);
				
				Tags.Clear(); //in case it is not first parsing
				foreach(string tagString in tagsStrings)
				{
					HtmlTag tag = new HtmlTag();
					tag.UnparsedLiteral = tagString;
					tag.ParseFromUnparsedLiteral();
					Tags.Add(tag);
				}
			}
		}
	}
}
