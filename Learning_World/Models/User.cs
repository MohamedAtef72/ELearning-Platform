using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Learning_World.Models;

public partial class User :IdentityUser<int>
{
    public string? Image { get; set; }

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

}
