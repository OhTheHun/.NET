using System.ComponentModel.DataAnnotations;

namespace BeeExamPro.BackendService.Model.Common
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool DeleteFlag { get; set; }
    }
}
