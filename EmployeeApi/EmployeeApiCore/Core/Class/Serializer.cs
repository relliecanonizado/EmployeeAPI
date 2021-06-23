using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace EmployeeApiCore.Core.Class
{
	public class Serializer
	{
		public static dynamic Deserialize(Stream xmlStream, System.Type type)
		{

			var data = Activator.CreateInstance(type);
			var reader = new XmlSerializer(type);

			data = Convert.ChangeType(reader.Deserialize(xmlStream), type);

			return data;

		}

		public static dynamic Deserialize(string xmlString, System.Type type)
		{

			try
			{
				var data = Activator.CreateInstance(type);
				var reader = new XmlSerializer(type);
				var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));

				data = Convert.ChangeType(reader.Deserialize(stream), type);

				return data;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;				
			}

		}

		public static dynamic DeserializeFromFile(string filename, System.Type type)
		{

			dynamic result;

			using(var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				result = Serializer.Deserialize(fileStream, type);
			}

			return result;

		}

		public static string Serialize(dynamic data, System.Type type)
		{

			XmlAttributeOverrides overrides = new XmlAttributeOverrides();
			XmlAttributes attribs = new XmlAttributes();
			attribs.XmlIgnore = true;
			attribs.XmlElements.Add(new XmlElementAttribute("Id"));
			overrides.Add(type, "Id", attribs);

			var writer = new XmlSerializer(data.GetType(), overrides);
			var stream = new MemoryStream();

			writer.Serialize(stream, data);

			stream.Position = 0;

			return new StreamReader(stream).ReadToEnd();

		}

		public static bool Serialize(dynamic data, System.Type type, string path)
		{

			StreamWriter streamWriter;

			if(data == null)
			{
				return false;
			}

			try
			{
				streamWriter = File.CreateText(path);
				streamWriter.WriteLine(Serializer.Serialize(data, data.GetType()));
				streamWriter.Flush();
				streamWriter.Dispose();

				return true;
			}
			catch(System.Exception ex)
			{
				throw;
			}

		}

		public static string Serialize(dynamic data)
		{
			var writer = new XmlSerializer(data.GetType());
			var stream = new MemoryStream();

			writer.Serialize(stream, data);

			stream.Position = 0;

			return new StreamReader(stream).ReadToEnd();
		}
	}
}
