using ATMApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATMApp.UI
{
    public  class AppScreen
    {
        internal const string cur = "NRs.";
        internal static void Welcome()
        {

            Console.Clear();

            Console.Title = "ABC ATM App";

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("\n\n-------------------Welcome to ABC ATM App-------------");

            Console.WriteLine("Please Insert your ATM card");

            Console.WriteLine("Note:Only Real ATM accept card number and validate them ");

            Utility.PressEnterToContinue();
        }
         internal static UserAccount UserLoginForm()
        {
            UserAccount tempUserAccount = new UserAccount();
            tempUserAccount.CardNumber = validation.Convert<long>("your card number");
            tempUserAccount.CardPin = Convert.ToInt32(Utility.GetSecretInput("Enter your card PIN"));
            return tempUserAccount;
        }

         internal static void LoginProgress()
        {
            Console.WriteLine("Checking card number and PIN.....");
            Utility.PrintDotAnimation();
        }

        internal static void PrintLockScreen()
        {
            Console.Clear();
            Utility.PrintMessage("Your account is locked.Please go to the nearest branch to unlock your account. Thank you.", true);
            Environment.Exit(1);
        }
        internal static void WelcomeCustomer(string fullName)
        {
            Console.WriteLine($"Welcome Back,{fullName}");
            Utility.PressEnterToContinue();
        }
        
        internal static void DisplayMenu()
        {
            
            Console.WriteLine("---------ABC ATM OPTIONS--------");
            Console.WriteLine(":                               ");
            Console.WriteLine("1. Account Balance             :");
            Console.WriteLine("2. Cash Deposit                :");
            Console.WriteLine("3. Cash Withdraw               :");
            Console.WriteLine("4. Transfer Balance            :");
            Console.WriteLine("5. Transactions Receipt        :");
            Console.WriteLine("6. Logout                      :");
        }
        
        internal static void LogOutProgress()
        {
            Console.WriteLine("Thank your for using ABC ATM.");
            Utility.PrintDotAnimation();
            Console.Clear();
        }

        internal static int SelectAmount()
        {
            Console.WriteLine("");
            Console.WriteLine(":1.{0}500         4.{0}10,000", cur);
            Console.WriteLine(":2.{0}1000        5.{0}15,000", cur);
            Console.WriteLine(":3.Other          6.{0}20,000",cur);
            Console.WriteLine("");

            int selectedAmount = validation.Convert<int>("option:");
            
            switch(selectedAmount)
            {
                case 1:
                    return 500;
                    break;

                case 2:
                    return 1000;
                    break;

                case 3:
                    return 0;
                    break;

                case 4:
                    return 10000;
                    break;

                case 5:
                    return 15000;
                    break;

                case 6:
                    return 20000;
                    break;

                default:
                    Utility.PrintMessage("Invalid Options. Try Again",false);
                    return -1;
                    break;
            }
        }
        internal InternalTransfer InternalTransferForm()
        {
            var internalTransfer = new InternalTransfer();
            internalTransfer.ReceiptBankAccountNumber = validation.Convert<long>("receipent's account number:");
            internalTransfer.TransferAmount = validation.Convert<decimal>($"amount {cur}");
            internalTransfer.ReceiptBankAccountName = Utility.GetUserInput("receipent's name:");
            return internalTransfer;
        }
    }
}
