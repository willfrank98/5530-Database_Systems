using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrolled
    {
        public uint ClassId { get; set; }
        public string UId { get; set; }
        public string Grade { get; set; }

        public virtual Classes Class { get; set; }
        public virtual Students U { get; set; }
    }
}
