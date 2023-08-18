namespace Communication
{
    public class SegmentationPrediction : PredictionInfo
    {
        /// <summary>
        /// List of segmentation resource images: original image, label and prediction
        /// </summary>
        public SegmentationResource[] predictions { get; set; }

    }
}
