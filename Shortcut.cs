
using Newtonsoft.Json;

namespace TelaFacilLauncher
{
    public class Shortcut
    {
        [JsonProperty("id_atalho")]
        public int id_atalho { get; set; }

        [JsonProperty("nome_atalho")]
        public string nome_atalho { get; set; } 

        [JsonProperty("caminho_atalho")]
        public string caminho_atalho { get; set; } 
    }
}
