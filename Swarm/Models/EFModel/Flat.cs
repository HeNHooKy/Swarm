using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Swarm.Models.EFModel
{
    /// <summary>
    /// Describes flat objects which could conatin one meter.
    /// </summary>
    [Table("Flat")]
    public class Flat
    {
        /// <summary>
        /// Building street name.
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// Building number on the street.
        /// </summary>
        public int Building { get; set; }
        /// <summary>
        /// Flat number in the building.
        /// </summary>
        public int FlatNumber { get; set; }
        /// <summary>
        /// Factory number of meter installed in the flat.
        /// </summary>
        public int? MeterFactoryNumber { get; set; }
        /// <summary>
        /// Meter which installed in the flat.
        /// </summary>
        public Meter Meter { get; set; }
    }
}
