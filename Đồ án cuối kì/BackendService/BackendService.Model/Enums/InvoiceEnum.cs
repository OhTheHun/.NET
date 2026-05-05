using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendService.Model.Enums
{
    public enum InvoiceEnum
    {
        Confirmed = 0,
        Processing = 1,
        Delivering = 2,
        Completed = 3,
        Canceled = 4,
    }
}
