using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

namespace Battleships.Data
{
	//Trida pro serializaci a deserializaci dat
	static class DataManager
	{
		//Deserializuje json
		public static OfType DeserializeJson<OfType>(string json)
		{
			//Pokus o prevod
			try
			{
				return JsonSerializer.Deserialize<OfType>(json);
			}
			catch (Exception e)
			{
				//Neuspech
				return default;
			}
		}
		//Serializuje json
		public static string SerializeJon<OfType>(OfType obj)
		{
			//Pokus o prevod
			try
			{
				return JsonSerializer.Serialize(obj);
			}
			catch (Exception e)
			{
				//Neuspech
				return default;
			}
		}
	}
}