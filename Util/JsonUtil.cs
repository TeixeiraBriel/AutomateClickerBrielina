using AutomateClickerBrielina.Entidades;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateClickerBrielina.Util
{
    public class JsonUtil
    {
        public static List<Save> CarregaJsons()
        {
            var file = @"Saves.json";

            return JsonConvert.DeserializeObject<List<Save>>(File.ReadAllText(file, Encoding.UTF8));
        }

        public static bool ModifiarJson(List<Save> newJson)
        {
            try
            {
                List<Save> novo = newJson;
                string output = JsonConvert.SerializeObject(novo);

                File.WriteAllText(@"Saves.json", output);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}
