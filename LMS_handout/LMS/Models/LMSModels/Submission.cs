using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public uint AssignmentId { get; set; }
        public string UId { get; set; }
        public uint? Score { get; set; }
        public string Contents { get; set; }
        public DateTime Time { get; set; }

        public virtual Assignments Assignment { get; set; }
        public virtual Students U { get; set; }
    }
}
