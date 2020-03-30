using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Courses
    {
        public Courses()
        {
            Classes = new HashSet<Classes>();
        }

        public uint CourseId { get; set; }
        public string SubjectAbbr { get; set; }
        public uint CourseNumber { get; set; }
        public string Name { get; set; }

        public virtual Departments SubjectAbbrNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
