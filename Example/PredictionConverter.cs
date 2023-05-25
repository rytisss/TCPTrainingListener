using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Communication
{
    public class PredictionConverter : JsonConverter<PredictionResults>
    {
        public override void WriteJson(JsonWriter writer, PredictionResults value, JsonSerializer serializer)
        {
            switch (value.operation)
            {
                case "segmentation":
                    {
                        serializer.Serialize(writer, (SegmentationPredictionResults)value);
                        break;
                    }
                default:
                    {
                        serializer.Serialize(writer, value);
                        break;
                    }
            }
        }

        public override PredictionResults ReadJson(JsonReader reader, Type objectType, PredictionResults existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            var operation = jsonObject["operation"]?.Value<string>();

            PredictionResults prediction;
            switch (operation)
            {
                case "segmentation":
                    {
                        prediction = new SegmentationPredictionResults();
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
            serializer.Populate(jsonObject.CreateReader(), prediction);
            return prediction;
        }

        public override bool CanRead => true;

        public override bool CanWrite => true;
    }
}
