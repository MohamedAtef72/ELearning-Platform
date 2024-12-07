using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learning_World.Models;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int UserId { get; set; } 

    public int? CourseId { get; set; }
    [ForeignKey(nameof(Payment))]
    public int PaymentID { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? User { get; set; }
    public Payment Payment { get; set; }
}
