using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Swarm.Models.EFModel
{
    /// <summary>
    /// Describes object of meter.
    /// </summary>
    [Table("Meter")]
    public class Meter
    {
        /// <summary>
        /// Factory number of meter (it can't be changed) [Primary Key]
        /// </summary>
        public int FactoryNumber { get; set; }
        /// <summary>
        /// Date when the meter was checked last time.
        /// </summary>
        public DateTime? LastCheck { get; set; }
        /// <summary>
        /// Date when the meter will be checked.
        /// </summary>
        public DateTime NextCheck { get; set; }
        /// <summary>
        /// All meter records.
        /// </summary>
        public IEnumerable<MeterRecords> MeterRecords { get; set; }
    }
}
