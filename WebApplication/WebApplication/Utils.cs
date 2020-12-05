using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace WebApplication
{
	public class Utils
	{
		public static readonly String InputFile = Path.Combine(Startup.ContentRoot, "input.mp4");
		public static String FilePath(Meme meme) => Path.Combine(Startup.ContentRoot, "Videos", $"{meme.Guid}.mp4");
		
		public static IExtendedXmlSerializer DefaultXmlSerializer() => new ConfigurationContainer()
			.UseAutoFormatting()
			.UseOptimizedNamespaces()
			.EnableImplicitTyping(typeof(List<Meme>))
			.Type<Meme>()
			.EnableReferences(p => p.Guid)
			.Create();
		
		public static string GetXmlFromObject(object obj)
		{
			IExtendedXmlSerializer serializer = DefaultXmlSerializer();

			var document = serializer.Serialize(new XmlWriterSettings {Indent = true},
				obj);
			return document;
		}
		
		public static String MD5(string input)
		{
			// Step 1, calculate MD5 hash from input
			MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hashBytes = md5.ComputeHash(inputBytes);
     
			// Step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hashBytes.Length; i++)
			{
				sb.Append(hashBytes[i].ToString("X2"));
			}
			return sb.ToString();
		}
	}
}