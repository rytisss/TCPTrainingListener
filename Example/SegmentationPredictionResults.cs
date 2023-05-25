using Newtonsoft.Json;
using OpenCvSharp;
using System.Drawing;

namespace Communication
{
    public class SegmentationPredictionResults : PredictionResults
    {
        /// <summary>
        /// List of segmentation resource images: original image, label and prediction
        /// </summary>
        public SegmentationImageResource[] predictions { get; set; } 
    }
}
