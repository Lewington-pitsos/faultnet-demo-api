using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace faultnet_demo_api {
    public class FaultRecord {
        [Key]      public int Row { get; set; }
        //[Required] public DateTime Date { get; set; }
        [Required] public double Latitude { get; set; }
        [Required] public double Longitude { get; set; }
        [Required] public int Accuracy { get; set; }
        [Required] public double Speed { get; set; }                   // Meters per second
        [Required] public double Bearing { get; set; }                 // Bearing (degrees north)
        [Required] public double LatCrackConfidence { get; set; }
        [Required] public double LongCrackConfidence { get; set; }
        [Required] public double CrocodileCrackConfidence { get; set; }
        [Required] public double PotholeConfidence { get; set; }
        [Required] public double LineblurConfidence { get; set; }
        [Required] public bool IsLatCrackFault { get; set; }
        [Required] public bool IsLongCrackFault { get; set; }
        [Required] public bool IsCrocodileCrackFault { get; set; }
        [Required] public bool IsPotholeFault { get; set; }
        [Required] public bool IsLineblurFault { get; set; }
        [Required] public String ImageFileName { get; set; }
    }
}