using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Swarm.Models.EFModel
{
    /// <summary>
    /// Describes object of replacement hisotry of meter in some flat
    /// </summary>
    [Table("MeterReplacementHistory")]
    public class MeterReplacementHistory
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
        /// Date when meter was changed.
        /// </summary>
        public DateTime SetupDate { get; set; }
        /// <summary>
        /// Value of old meter before it was changed.
        /// </summary>
        public int? OldMeterValue { get; set; }
        /// <summary>
        /// Factory number of new meter.
        /// </summary>
        public int? NewMeterFactoryNumber { get; set; }
    }
}
