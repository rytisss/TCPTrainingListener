using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Communication
{
    public class PredictionConverter : JsonConverter<PredictionInfo>
    {
        public override void WriteJson(JsonWriter writer, PredictionInfo value, JsonSerializer serializer)
        {
            switch (value.operation)
            {
                case "segmentation":
                    {
                        serializer.Serialize(writer, (SegmentationPrediction)value);
                        break;
                    }
                case "detectionxai":
                    {
                        serializer.Serialize(writer, (DetectionXAIPrediction)value);
                        break;
                    }
                default:
                    {
                        serializer.Serialize(writer, value);
                        break;
                    }
            }
        }

        public override PredictionInfo ReadJson(JsonReader reader, Type objectType, PredictionInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            var operation = jsonObject["operation"]?.Value<string>();

            PredictionInfo prediction = null;
            switch (operation)
            {
                case "segmentation":
                    {
                        prediction = new SegmentationPrediction();
                        break;
                    }
                case "detectionxai":
                    {
                        prediction = new DetectionXAIPrediction();
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
