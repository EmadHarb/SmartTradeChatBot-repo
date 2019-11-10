// <auto-generated>
// Code generated by LUISGen CognitiveModels\SmartTrade.json -cs CognitiveModels.SmartTrade -o CognitiveModels
// Tool github: https://github.com/microsoft/botbuilder-tools
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;

namespace CognitiveModels
{
    public partial class SmartTrade: IRecognizerConvert
    {
        //[JsonProperty("text")]
        public string Text;

        //[JsonProperty("alteredText")]
        public string AlteredText;

        public enum Intent {
            getCustomer, 
            getProduct, 
            getProductByCategory, 
            getProductByModel, 
            getProductByNumber, 
            getSalesByCustomer, 
            getSalesByProduct, 
            None
        };
        //[JsonProperty("intents")]
        public Dictionary<Intent, IntentScore> Intents;

        public class _Entities
        {
            // Simple entities
            public string[] Category;

            public string[] Customer;
            public string[] sales;

            public string[] Model;

            public string[] Product;

            // Built-in entities
            public DateTimeSpec[] datetime;

            // Pattern.any
            public string[] ProductNumber;

            // Instance
            public class _Instance
            {
                public InstanceData[] Category;
                public InstanceData[] Customer;
                public InstanceData[] sales;
                public InstanceData[] Model;
                public InstanceData[] Product;
                public InstanceData[] ProductNumber;
                public InstanceData[] datetime;
            }
            //[JsonProperty("$instance")]
            public _Instance _instance;
        }
        //[JsonProperty("entities")]
        public _Entities Entities;

        [JsonExtensionData(ReadData = true, WriteData = true)]
        public IDictionary<string, object> Properties {get; set; }

        public void Convert(dynamic result)
        {
            var app = JsonConvert.DeserializeObject<SmartTrade>(JsonConvert.SerializeObject(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            Text = app.Text;
            AlteredText = app.AlteredText;
            Intents = app.Intents;
            Entities = app.Entities;
            Properties = app.Properties;
        }

        public (Intent intent, double score) TopIntent()
        {
            Intent maxIntent = Intent.None;
            var max = 0.0;
            foreach (var entry in Intents)
            {
                if (entry.Value.Score > max)
                {
                    maxIntent = entry.Key;
                    max = entry.Value.Score.Value;
                }
            }
            return (maxIntent, max);
        }
    }
}
