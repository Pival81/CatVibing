using System;
using System.Collections.Generic;
using System.IO;
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
	}
}