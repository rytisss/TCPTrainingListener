using Newtonsoft.Json;
using OpenCvSharp;

namespace Communication
{
    public class SegmentationResource
    {
        /// <summary>
        /// Sample image from test set
        /// </summary>
        [JsonConverter(typeof(ImageDataConverter))]
        public Mat image { get; set; }
        /// <summary>
        /// Predicted segmentation image
        /// </summary>
        [JsonConverter(typeof(ImageDataConverter))]
        public Mat prediction { get; set; }
        /// <summary>
        /// Label image
        /// </summary>
        [JsonConverter(typeof(ImageDataConverter))]
        public Mat label { get; set; }
    }
}
