﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
            Enrolled = new HashSet<Enrolled>();
        }

        public uint ClassId { get; set; }
        public uint CourseId { get; set; }
        public string Semester { get; set; }
        public string Location { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string Professor { get; set; }

        public virtual Courses Course { get; set; }
        public virtual Professors ProfessorNavigation { get; set; }
        public virtual ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual ICollection<Enrolled> Enrolled { get; set; }
    }
}
