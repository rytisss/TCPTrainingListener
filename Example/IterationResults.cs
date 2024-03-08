using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    /// <summary>
    /// Structure example:
    /// {
    /// "model_name": "default",
    ///  "iteration": 11,
    ///  "iteration_count" 30,
    ///  "epoch": 5,
    ///  "parameters": {
    ///    "loss": 0.21460704505443573,
    ///    "binary_accuracy": 0.98210209608078
    ///  }
    ///}
    /// </summary>
    public class IterationResults
    {
        /// <summary>
        /// Name of the model that is training
        /// </summary>
        public string model_name { get; set; }
        /// <summary>
        /// Iteration index
        /// </summary>
        public int iteration { get; set; }
        /// <summary>
        /// Total number of iterations
        /// </summary>
        public int iteration_count { get; set; }
        /// <summary>
        /// Epoch index
        /// </summary>
        public int epoch { get; set; }
        /// <summary>
        /// Other parameters, depends on the training but all the time consists of string key and float value
        /// </summary>
        public Dictionary<string, float> parameters { get; set; }

    }
}
