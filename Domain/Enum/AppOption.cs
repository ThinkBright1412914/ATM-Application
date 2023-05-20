using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.Domain.Enum
{
    public  enum AppOption
    {
        CheckBalance =1,
        PlaceDeposit,
        MakeWithdrawal,
        InternalTransfer,
        ViewTransaction,
        Logout
    }
}
