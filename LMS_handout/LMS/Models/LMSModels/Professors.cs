﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professors
    {
        public Professors()
        {
            Classes = new HashSet<Classes>();
        }

        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Department { get; set; }

        public virtual Departments DepartmentNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
