using BeeExamPro.BackendService.Model.Common;
namespace BackendService.Model
{
    public class SanPham: BaseEntity
    {
        public Guid DanhMucId { get; set; }
        public string TenSanPham {  get; set; } = string.Empty;
        public int Gia { get; set;  }

        
    }
}
