using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Entities
{
    [Table("Branches")]
    public class Branch
    {
        public int BranchId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
