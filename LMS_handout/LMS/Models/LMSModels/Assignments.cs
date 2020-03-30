using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submission = new HashSet<Submission>();
        }

        public uint AssignmentId { get; set; }
        public uint AssignCatId { get; set; }
        public string Name { get; set; }
        public uint MaxPoints { get; set; }
        public string Contents { get; set; }
        public DateTime? DueDate { get; set; }

        public virtual AssignmentCategories AssignCat { get; set; }
        public virtual ICollection<Submission> Submission { get; set; }
    }
}
