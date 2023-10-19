using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidPay.Core.Services.Interfaces
{
    public interface IUFEService
    {
        public static UFEService? Instance;

        public decimal CalculateFee();
    }
}
