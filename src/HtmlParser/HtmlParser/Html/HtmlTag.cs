using System;
using System.Collections.Generic;

namespace HtmlParser
{
	/// <summary>
	/// Represents single Html Tag with its subtags and attributes.
	/// </summary>
	public class HtmlTag
	{
		public string UnparsedLiteral;
		
		public string Name;
		
		/// <summary>
		/// Contains attributes of this HTML tag, keys are names of attributes, Values are values of attributes
		/// (everything after "=" following attribue's name in source HTML)
		/// </summary>
		public Dictionary<string,HtmlTagAttribute> Attributes = new Dictionary<string, HtmlTagAttribute>();
		
		/// <summary>
		/// Literal contents are also included here as "tags" of type "Literal_Contents"
		/// </summary>
		public List<HtmlTag> NestedTags = new List<HtmlTag>();
		
		public void ParseFromUnparsedLiteral()
		{
			if (UnparsedLiteral != null && UnparsedLiteral.Length > 0)
			{
				int index = 0;
				int closingBracketIndex = -1;
				while (index < UnparsedLiteral.Length)
				{
					if (UnparsedLiteral[index] == Serialize.HtmlTagClosingBracket)
					{
						closingBracketIndex = index;
						break;
					}
					index++;
				}
				
				if (closingBracketIndex < 0) //if this segment of HTML is incorrect
				{
					return;
				}
				
				string tagHeader = UnparsedLiteral.Substring(0,closingBracketIndex + 1);
				ParseNameAndAttributesFromTagHeader(tagHeader);
				
				string tagNestedContents = UnparsedLiteral.Substring(closingBracketIndex + 1); //remove opening tag
				if (tagNestedContents.Length > 0) //remove closing tag
				{
					int indexOfClosingTagStart = tagNestedContents.LastIndexOf(Serialize.HtmlClosingTagNamePrefix) - 1;
					if (indexOfClosingTagStart >= 0)
					{
						tagNestedContents = tagNestedContents.Substring(0, indexOfClosingTagStart);
					}
				}
				
				List<string> tagsStrings = Serialize.SplitByRootTags(tagNestedContents);
				
				NestedTags.Clear(); //in case it is not first parsing
				foreach(string tagString in tagsStrings)
				{
					HtmlTag tag = new HtmlTag();
					tag.UnparsedLiteral = tagString;
					tag.ParseFromUnparsedLiteral();
					NestedTags.Add(tag);
				}
			}
		}
		
		public void ParseNameAndAttributesFromTagHeader(string tagHeader)
		{
			tagHeader = tagHeader.Trim(Serialize.HtmlTagAttributeSeparator, Serialize.HtmlTagOpeningBracket, Serialize.HtmlTagClosingBracket);
			//TODO: parse tag attributes
			//TODO: parse elements of styles nested in HTML tag headers
			//Console.WriteLine(tagHeader);
		}
	}
}
