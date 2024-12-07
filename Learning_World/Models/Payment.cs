using System.ComponentModel.DataAnnotations;

namespace Learning_World.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }
        public string Country { get; set; }
        public string CardName { get; set; } // Nullable for PayPal
        public string CardNumber { get; set; } // Nullable for PayPal
        public string ExpiryDate { get; set; } // Format: MM/YYYY
        public string CVC { get; set; } // Nullable for PayPal
        public Enrollment Enrollment { get; set; }
    }
}
