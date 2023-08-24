using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Domain.Entities.Common
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        virtual public DateTime UpdatedDate { get; set; }
    }
}
