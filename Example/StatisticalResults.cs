﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Communication
{
    /// <summary>
    /// Structure example for segmentation statistics:
    /// "Statistics": {
    ///  "epoch": 11,
    ///  "best_weights": false,
    ///  "parameters": {
    ///    "loss": 0.21460704505443573,
    ///    "dice_eval": 0.98210209608078,
    ///    "val_loss": 0.21035005152225494,
    ///    "val_dice_eval": 0.96921932697296143
    ///  }
    ///}
    /// </summary>
    class StatisticalResults
    {
        /// <summary>
        /// Epoch
        /// </summary>
        public int epoch { get; set; }
        /// <summary>
        /// Flag is this epoch gives the best-performaning weights
        /// </summary>
        public bool best_weights { get; set; }
        /// <summary>
        /// Other parameters, depends on the training but all the time consists of string key and float value
        /// </summary>
        public Dictionary<string, float> parameters { get; set; }
    }
}
