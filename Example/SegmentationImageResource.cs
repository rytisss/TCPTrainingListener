using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public class SegmentationImageResource
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
