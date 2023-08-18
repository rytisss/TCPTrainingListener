namespace Communication
{
    public class PredictionInfo
    {
        /// <summary>
        /// Epoch number
        /// </summary>
        public int epoch { get; set; }
        /// <summary>
        /// Epoch number
        /// </summary>
        public int epoch_count { get; set; }
        /// <summary>
        /// Name of operation, can be 'detectionxai', 'segmentation' or other [currently not implemented apart first two].
        /// According to the operation, derived objects will differ in parameters
        /// </summary>
        public string operation { get; set; }
    }
}
