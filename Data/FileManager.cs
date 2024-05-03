using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Resources;
using System.Globalization;

namespace Battleships.Data
{
	//Trida pro nacitani a ukladani slozek a souboru
	static class FileManager
	{
		//Ziska vsechen obsah souboru, ktery se nachazi ve zdrojich
		public static string? GetResource(string resourceName)
		{
			//Nacteni streamu
			Assembly assembly = Assembly.GetExecutingAssembly();
			Stream stream = assembly.GetManifestResourceStream(resourceName);
			//Kontrola, ze zdroj existuje
			if (stream is null) return null;

			//Cteni obsahu
			StreamReader reader = new(stream);
			string content = reader.ReadToEnd();

			//Ukonceni streamu
			reader.Close();
			stream.Close();
			return content;
		}
		//Ziska obsah vsech zdroju na dane urovni
		public static Dictionary<string, string> GetResources(string resourceLocation = "Battleships", bool includeExtension = false, bool descendants = false)
		{
			//Pokud neni v urovni tecka na konci, tak se automaticky prida
			if (!resourceLocation.EndsWith('.')) resourceLocation += '.';

			//Nacteni nazvu vsech zdroju
			Assembly assembly = Assembly.GetExecutingAssembly();
			IEnumerable<string> resourceNames = assembly.GetManifestResourceNames();
			//Vysledne obsahy
			Dictionary<string, string> resources = new();
			//Nacteni zdroju
			foreach (string fullResourceName in resourceNames)
			{
				//Musi byt na patricne urovni
				if (!fullResourceName.StartsWith(resourceLocation)) continue;
				//Odebrani cesty k urovni
				string resourceName = fullResourceName[resourceLocation.Length..];
				//Odebrani pripony
				int extensionIndex = resourceName.LastIndexOf('.');
				string extension = (extensionIndex != -1 ? resourceName[(extensionIndex + 1)..] : "");
				resourceName = (extensionIndex != -1 ? resourceName[..extensionIndex] : resourceName);

				//Kontrola urovne
				if (!descendants && resourceName.Contains('.')) continue;
				//Nacteni obsahu
				resources[resourceName + (includeExtension ? '.' + extension : "")] = GetResource(fullResourceName);
			}
			return resources;
		}
	}
}
