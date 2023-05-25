namespace Communication
{
    public class PredictionResults
    {
        /// <summary>
        /// Epoch number
        /// </summary>
        public int epoch { get; set; }
        /// <summary>
        /// Name of operation, can be 'classification', 'segmentation' or other.
        /// According to the operation, derived objects will differ in parameters
        /// </summary>
        public string operation { get; set; }
    }
}
