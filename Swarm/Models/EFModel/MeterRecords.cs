using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Swarm.Models.EFModel
{
    /// <summary>
    /// Describes object of all meter records.
    /// </summary>
    [Table("MeterRecords")]
    public class MeterRecords
    {
        /// <summary>
        /// Factory number of the meter.
        /// </summary>
        public int MeterFactoryNumber { get; set; }
        /// <summary>
        /// Value of the meter when it was checked.
        /// </summary>
        public int MeterValue { get; set; }
        /// <summary>
        /// Date when meter was checked.
        /// </summary>
        public DateTime CheckDate { get; set; }
        /// <summary>
        /// Ref to this meter.
        /// </summary>
        public Meter Meter { get; set; }
    }
}
