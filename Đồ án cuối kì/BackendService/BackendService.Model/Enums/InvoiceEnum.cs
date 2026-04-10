using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendService.Model.Enums
{
    public enum InvoiceEnum
    {
        Pending = 0,
        Confirmed = 1,
        Canceled = 2,
        Completed = 3
    }
}
