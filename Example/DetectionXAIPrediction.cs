using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public class DetectionXAIPrediction : PredictionInfo
    {
        /// <summary>
        /// Array of binary prediction from defect detection model
        /// </summary>
        public DetectionXAIResource[] predictions { get; set; }
    }
}
