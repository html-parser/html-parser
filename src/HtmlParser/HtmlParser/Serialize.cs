using System;
using System.Collections.Generic;

namespace HtmlParser
{
	/// <summary>
	/// Serializes to and from different text-based formats. 
	/// Only deseralization from Html is partially implemented so far.
	/// </summary>
	public static class Serialize
	{
		public const char HtmlTagOpeningBracket = '<';
		public const char HtmlTagClosingBracket = '>';
		public const char HtmlTagAttributeSeparator = ' ';
		public const string HtmlClosingTagNamePrefix = "/";
		
		public static HtmlPage FromHtml(string html)
		{
			HtmlPage result = new HtmlPage();
			result.UnparsedLiteral = html;
			result.ParseFromUnparsedLiteral();
			
			return result;
		}
				
		/// <summary>
		/// returns list of strings, one per root tag in input htmlSegment
		/// </summary>
		/// <param name="htmlSegment"></param>
		/// <returns></returns>
		public static List<string> SplitByRootTags(string htmlSegment)
		{
			if (htmlSegment == null || htmlSegment.Length <= 0)
			{
				throw new NullReferenceException("input htmlSegment is null or of zero length");
			}
			
			htmlSegment = htmlSegment.Trim(' ', '\r', '\n');
			List<string> result = new List<string>();
			int index = 0;
			string tagName = null;
			
			while (index < htmlSegment.Length)
			{
				int indexOfOpeningTag = GetIndexOfNextOpeningTag(htmlSegment, ref index, out tagName);
				if (indexOfOpeningTag < 0)
				{ //if opening tag was not found then store (incorrect) HTML as HtmlTag object anyway
					indexOfOpeningTag = index;
				}
				int indexOfMatchingClosingTag = GetIndexOfMatchingClosingTag(htmlSegment, ref index, tagName);
				int indexOfClosingTagEnd = indexOfMatchingClosingTag;
				if (tagName != null)
				{
					indexOfClosingTagEnd += tagName.Length + HtmlClosingTagNamePrefix.Length;
				}
				indexOfClosingTagEnd = Math.Min(indexOfClosingTagEnd, htmlSegment.Length);
				if (indexOfMatchingClosingTag < 0)
				{
					indexOfClosingTagEnd = htmlSegment.Length;
				}
				string tagString = htmlSegment.Substring(indexOfOpeningTag, indexOfClosingTagEnd - indexOfOpeningTag);
				result.Add(tagString);
			}
						
			return result;
		}
		
		private static int GetIndexOfNextOpeningTag(string htmlSegment, ref int index, out string tagName)
		{
			int openingBracketIndex = -1;
			int closingBracketIndex = -1;
			
			while (index < htmlSegment.Length - 1)
			{
				if (htmlSegment[index] == HtmlTagOpeningBracket 
				    && htmlSegment[index] + 1 != HtmlClosingTagNamePrefix[0]) //to also ignore closing tags
				{
					openingBracketIndex = index;
					break;
				}
				index++;
			}
			
			while (index < htmlSegment.Length)
			{
				if (htmlSegment[index] == HtmlTagClosingBracket || htmlSegment[index] == HtmlTagAttributeSeparator)
				{
					closingBracketIndex = index;
					break;
				}
				index++;
			}
			
			if (openingBracketIndex >= 0 && closingBracketIndex >= 0)
			{
				tagName = htmlSegment.Substring(openingBracketIndex, closingBracketIndex - openingBracketIndex + 1);
				return openingBracketIndex;
			}
			
			tagName = null;
			return -1;
		}
		
		private static int GetIndexOfMatchingClosingTag(string htmlSegment, ref int index, string tagName)
		{
			if (tagName == null || tagName.Length <= 0)
			{ //if tag name is empty or HTML is otherwise incorrect then skip to next correct tag of html
				while (index < htmlSegment.Length)
				{
					if (htmlSegment[index] == HtmlTagOpeningBracket || htmlSegment[index] == HtmlTagAttributeSeparator)
					{
						return index;
					}
					index++;
				}
				return index;
			}
			
			int openingBracketIndex = -1;
			int indexOfClosingTag = -2;
			int nestingLevel = 0; //to catch other tags with same name nested inside each other
			string tagNameWithoutClosingBracket = tagName.Substring(0, tagName.Length - 1);
			string tagNameForMatchingClosingTag = tagNameWithoutClosingBracket.Insert(1,HtmlClosingTagNamePrefix).Replace(HtmlTagAttributeSeparator, HtmlTagClosingBracket);
			while (indexOfClosingTag == -2) 
			{
				index++; //to avoid checking same opening bracket indefinitely
				while (index < htmlSegment.Length)
				{
					if (htmlSegment[index] == HtmlTagOpeningBracket)
					{
						openingBracketIndex = index;
						break;
					}
					index++;
				}
				if (index >= htmlSegment.Length 
				    || htmlSegment.Length < openingBracketIndex + tagNameForMatchingClosingTag.Length)
				{
					indexOfClosingTag = -1; //to break from outer while loop
				}
				else if (htmlSegment.Substring(openingBracketIndex, tagNameForMatchingClosingTag.Length) == tagNameForMatchingClosingTag)
				{ //if matching closing tag for tag tagName was found
					nestingLevel--;
					if (nestingLevel < 0)
					{ //if all nested tags with same name as tagName were closed earlier or not encountered
						indexOfClosingTag = openingBracketIndex;
						index += tagNameForMatchingClosingTag.Length + 1; //set to next character after closing tag
					}
				}
				else if (htmlSegment.Substring(openingBracketIndex, tagNameWithoutClosingBracket.Length) == tagNameWithoutClosingBracket)
				{ //if another opening tag with same name as tagName was found
					nestingLevel++;
				}
			}
			
			return indexOfClosingTag;
		}
	}
}
