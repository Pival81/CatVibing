using System;
using System.IO;
using System.Xml;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace WebApplication
{
	public class Utils
	{
		public static readonly String InputFile = Path.Combine(Startup.ContentRoot, "input.mp4");
		
		public static string GetXmlFromObject(object obj)
		{
			IExtendedXmlSerializer serializer = new ConfigurationContainer().UseAutoFormatting()
				.UseOptimizedNamespaces()
				.EnableImplicitTyping(obj.GetType())
				.Create();

			var document = serializer.Serialize(new XmlWriterSettings {Indent = true},
				obj);
			return document;
		}
	}
}