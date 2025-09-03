namespace BitsMVCProject.Models
{
    public class ContactModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public bool Married { get; set; }
        public string Phone { get; set; } = null!;
        public decimal Salary { get; set; }
    }
}
