using Newtonsoft.Json;
using OpenCvSharp;

namespace Communication
{
    public class DetectionXAIResource
    {
        /// <summary>
        /// Sample image from test set
        /// </summary>
        [JsonConverter(typeof(ImageDataConverter))]
        public Mat image { get; set; }
        /// <summary>
        /// Label: 0.0 - negative (defect-free), 1.0 - defect
        /// </summary>
        public float label { get; set; } = 0.0f;
        /// <summary>
        /// Prediction score - in range [0.0;1.0]
        /// </summary>
        public float prediction { get; set; } = 0.0f;
    }
}
