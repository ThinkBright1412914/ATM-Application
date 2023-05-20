using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.Domain.Interface
{
    public  interface UserAccountAction
    {
        void checkBalance();
        void PlaceDeposit();

        void MakeWithdrawal();

        
    }
}
