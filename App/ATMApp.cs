using ATMApp.Domain.Entities;
using ATMApp.Domain.Enum;
using ATMApp.Domain.Interface;
using ATMApp.UI;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATMApp
{
    public class ATMApp : UserLogin, UserAccountAction, ITransaction
    {
        private List<UserAccount> UserAccountList;
        private UserAccount selectedAccount;
        private List<Transaction> _listOfTransactions;
        private const decimal minimumKeptAmount = 500;
        private readonly AppScreen screen;

        public ATMApp()
        {
            screen = new AppScreen();
        }

        public void Run()
        {
            AppScreen.Welcome();   
            CheckUserCardNumAndPswd();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);

            
                AppScreen.DisplayMenu();
                ProcessMenuOption();
            
        }

     

        public void InitializedData()
        {
            UserAccountList = new List<UserAccount>
            {
                new UserAccount { Id = 1, FullName = "Krishna Thapa", AccountNumber = 150677, CardNumber = 12347, CardPin = 1000, AccountBalance = 3000000, IsLocked = false },
                new UserAccount { Id = 2, FullName = "Sharda Thapa", AccountNumber = 150678, CardNumber = 12346, CardPin = 4321, AccountBalance = 2000000, IsLocked = false },
                new UserAccount { Id = 3, FullName = "Nabin Kishor", AccountNumber = 150679, CardNumber = 12345, CardPin = 1234, AccountBalance = 5000000, IsLocked = false }
            };

            _listOfTransactions = new List<Transaction>();
           

            
        }

        public void CheckUserCardNumAndPswd()
        {
            bool isCorrectLogin = false;
            while (isCorrectLogin == false)
            {
                UserAccount inputAccount = AppScreen.UserLoginForm();
                AppScreen.LoginProgress();
                foreach (UserAccount account in UserAccountList)
                {
                    selectedAccount = account;
                    if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                    {
                        selectedAccount.TotalLogin++;

                        if (inputAccount.CardPin.Equals(selectedAccount.CardPin))
                        {
                            selectedAccount = account;

                            if (selectedAccount.IsLocked || selectedAccount.TotalLogin > 3)
                            {
                                // Print a lock message

                                AppScreen.PrintLockScreen();
                            }
                            else
                            {
                                selectedAccount.TotalLogin = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }
                    }
                    if (isCorrectLogin == false)
                    {
                        Utility.PrintMessage("\n Invalid card number or Invalid PIN.", false);
                        selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                        if (selectedAccount.IsLocked)
                        {
                            AppScreen.PrintLockScreen();
                        }
                    }
                    Console.Clear();
                }
            }
            
        }
        private void ProcessMenuOption()
        {
             switch(validation.Convert<int>("An option:"))
            {
                case (int) AppOption.CheckBalance:
                    checkBalance();
                    break;

                case (int)AppOption.PlaceDeposit:
                    PlaceDeposit();
                    break;

                case (int)AppOption.MakeWithdrawal:
                    MakeWithdrawal();               
                    break;

                case (int)AppOption.InternalTransfer:
                    var internalTransfer = screen.InternalTransferForm();
                    ProcessInternalTransfer(internalTransfer);
                    break;

                case (int)AppOption.ViewTransaction:
                    ViewTransaction();  
                    break;

                case (int)AppOption.Logout:
                    AppScreen.LogOutProgress();
                    Utility.PrintMessage("You have successfully logout.");
                    Run();
                    break;

                default:
                    Utility.PrintMessage("Invalid Option",false);                  
                    break;

            }
        }

        public void checkBalance()
        {
            Utility.PrintMessage($"Your account balance is: NRs. {Utility.FormatAmount(selectedAccount.AccountBalance)}");
        }

        public void PlaceDeposit()
        {
            Console.WriteLine("\n Only multiples of 500 and 1000 Nepali Rupees are allowed.");
            var transaction_amt = validation.Convert<int>($"amount {AppScreen.cur}");


            //For counting
            Console.WriteLine("\nChecking and Counting Bank Notes.");
            Utility.PrintDotAnimation();
            Console.WriteLine("");

            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than zero.");
                return;
            }
            if (transaction_amt % 500 != 0)
            {
                Utility.PrintMessage($"Enter deposit amount in multiple of 500 or 1000.Try Again", false);

                return;
            }

            if (PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.PrintMessage($"You have cancelled your action.", false);
                return;
            }

            InsertTransaction(selectedAccount.Id, TransactionType.Deposit, transaction_amt, "");

            selectedAccount.AccountBalance += transaction_amt;

            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was succesful.", true);
        }

        public void MakeWithdrawal()
        {
           var transaction_amt = 0;
            int selectedAmount = AppScreen.SelectAmount();
            
            if(selectedAmount == -1)
            {
                MakeWithdrawal();
                return;
            }
            else if(selectedAmount != 0)
            {
                transaction_amt = selectedAmount;
            }
            else
            {
                transaction_amt = validation.Convert<int>($"amount {AppScreen.cur}");
            }

            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than 0. Try Again", false);
                return;
            }

            if (transaction_amt % 500 != 0)
            {
                Utility.PrintMessage("You can only withdraw amount in multiple of 500 or 1000. Try Again", false);
                return;
            }

            if(transaction_amt > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Withdraw failed. Your balance is too low to withdraw." + $"{Utility.FormatAmount(transaction_amt)}", false);
                return;
            }
            
            if((selectedAccount.AccountBalance - transaction_amt) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account needs to have " + $"minimum {Utility.FormatAmount(minimumKeptAmount)}",false);
                return;
            }

            InsertTransaction(selectedAccount.Id,TransactionType.Withdrawl,transaction_amt,"");

            selectedAccount.AccountBalance -= transaction_amt;

            Utility.PrintMessage("You have successfully withdraw" + $"{Utility.FormatAmount(transaction_amt)}", true);
        }
        
        private bool PreviewBankNotesCount(int amount)
        {
            int thousandNotesCount = amount / 1000;
            int fiveHundredNotesCount = (amount % 1000 ) / 500;

            Console.WriteLine("\nSummary");
            Console.WriteLine("-----");
            Console.WriteLine($"{AppScreen.cur}1000 X {thousandNotesCount} = { 1000 * thousandNotesCount}");
            Console.WriteLine($"{AppScreen.cur}500 x {fiveHundredNotesCount} = {500 * fiveHundredNotesCount}");
            Console.WriteLine($"Total Amount : {Utility.FormatAmount(amount)}\n\n");

            int opt = validation.Convert<int>("1 to Confirm");
            return opt.Equals(1);        }

        public void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc)
        {
            var transaction = new Transaction()
            {
                TransactionId = Utility.GetTransactionId(),
                UserBankAccountId = _UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _tranType,
                TransactionAmount = _tranAmount,
                Description = _desc                   
            };

            _listOfTransactions.Add(transaction);
        }

        public void ViewTransaction()
        {
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountId == selectedAccount.Id).ToList();
            //check if there's a transaction
            if (filteredTransactionList.Count <= 0)
            {
                Utility.PrintMessage("You have no transaction yet.", true);
            }
            else
            {
                var table = new ConsoleTable("Id", "Transaction Date", "Type", "Descriptions", "Amount " + AppScreen.cur);
                foreach (var tran in filteredTransactionList)
                {
                    table.AddRow(tran.TransactionId, tran.TransactionDate, tran.TransactionType, tran.Description, tran.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)", true);
            }
        }

        private void ProcessInternalTransfer(InternalTransfer internalTransfer)
        {
            if( internalTransfer.TransferAmount <= 0 ) {
                Utility.PrintMessage("Amount needs to be more than zero.Try Again");
                return;
            }

            if (internalTransfer.TransferAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Transfer failed. You do not have enough balance" + $" to transfer {Utility.FormatAmount(internalTransfer.TransferAmount)}");
                return;
            }

            if((selectedAccount.AccountBalance - internalTransfer.TransferAmount) < minimumKeptAmount )
            {
                Utility.PrintMessage($"Transfer failed. Your account don't have sufficient balance" + $"{Utility.FormatAmount(minimumKeptAmount)}",false);
                return;
            }

            
            var selectedBankAccountReceiver = (from userAcc in UserAccountList
                                               where userAcc.AccountNumber == internalTransfer.ReceiptBankAccountNumber
                                               select userAcc).FirstOrDefault();
            if(selectedBankAccountReceiver == null )
            {
                Utility.PrintMessage("Transfer failed. Receiver Bank account number haven't been registered.",false );
                return;
            }

            if ( selectedBankAccountReceiver.FullName != internalTransfer.ReceiptBankAccountName ) {
                Utility.PrintMessage("Transfer failed. Recipient's bank account name haven't been registered", false);
                return;
            }

            // add transaction to transaction record-sender
            InsertTransaction(selectedAccount.Id, TransactionType.Transfer, internalTransfer.TransferAmount, "Transfer" +
                $"to {selectedBankAccountReceiver.AccountNumber} ({selectedBankAccountReceiver.FullName})");

            //update sender's account balance
            selectedAccount.AccountBalance -= internalTransfer.TransferAmount;


            //add transaction record-receiver

            InsertTransaction(selectedBankAccountReceiver.Id, TransactionType.Transfer, internalTransfer.TransferAmount, "Transferred from" + $"{selectedAccount.AccountNumber}({selectedAccount.FullName})");

            //update receiver's account balance

            selectedBankAccountReceiver.AccountBalance += internalTransfer.TransferAmount;

            //print success message

            Utility.PrintMessage($"You have successfully transferred" + $"{Utility.FormatAmount(internalTransfer.TransferAmount)}to " +
                $"{internalTransfer.ReceiptBankAccountName}",true);
        }

       

     
    }
} 
