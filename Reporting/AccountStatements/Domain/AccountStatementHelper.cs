/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Helper methods                          *
*  Type     : VouchersByAccountHelper                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Helper methods to build vouchers by account information.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.Data;
using Empiria.FinancialAccounting.Reporting.Domain;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Helper methods to build vouchers by account information.</summary>
  internal class AccountStatementHelper {

    private readonly AccountStatementQuery AccountStatementCommand;

    internal AccountStatementHelper(AccountStatementQuery accountStatementCommand) {
      Assertion.Require(accountStatementCommand, "accountStatementCommand");

      AccountStatementCommand = accountStatementCommand;
    }


    #region Public methods


    internal FixedList<AccountStatementEntry> CombineInitialAccountBalanceWithVouchers(
                                                FixedList<AccountStatementEntry> orderingVouchers,
                                                AccountStatementEntry initialAccountBalance) {

      var totalBalanceAndVouchers = new List<AccountStatementEntry>();
      if (initialAccountBalance != null) {
        totalBalanceAndVouchers.Add(initialAccountBalance);
      }
      totalBalanceAndVouchers.AddRange(orderingVouchers);

      return totalBalanceAndVouchers.ToFixedList();
    }


    internal FixedList<AccountStatementEntry> GetOrderingVouchers(
                                              FixedList<AccountStatementEntry> voucherEntries) {

      List<AccountStatementEntry> returnedVouchers = voucherEntries
                                                      .OrderBy(a => a.AccountingDate)
                                                      .ThenBy(a=>a.Ledger.Number)
                                                      .ThenBy(a => a.AccountNumber)
                                                      .ThenBy(a => a.SubledgerAccountNumber)
                                                      .ThenBy(a => a.VoucherNumber)
                                                      .ToList();
      return returnedVouchers.ToFixedList();
    }

    internal AccountStatementEntry GetInitialAccountBalance(FixedList<AccountStatementEntry> orderingVouchers) {

      List<AccountStatementEntry> returnedVouchersWithCurrentBalance =
                                   new List<AccountStatementEntry>(orderingVouchers).ToList();

      decimal initialBalance = AccountStatementCommand.Entry.CurrentBalanceForBalances;

      foreach (var voucher in returnedVouchersWithCurrentBalance) {
        if (voucher.DebtorCreditor == "A") {
          initialBalance = initialBalance + (voucher.Debit - voucher.Credit);
        } else {
          initialBalance = initialBalance + (voucher.Credit - voucher.Debit);
        }
      }

      AccountStatementEntry initialAccountBalance = GetInitialOrCurrentAccountBalance(initialBalance);
      return initialAccountBalance;
    }

    internal AccountStatementEntry GetInitialOrCurrentAccountBalance(
                                      decimal balance, bool isCurrentBalance = false,
                                      decimal debit=0, decimal credit=0) {

      var initialBalanceEntry = new AccountStatementEntry();

      initialBalanceEntry.Ledger = Ledger.Empty;
      initialBalanceEntry.Currency = Currency.Empty;
      //initialBalanceEntry.Account = StandardAccount.Empty;
      initialBalanceEntry.StandardAccountId = StandardAccount.Empty.Id;
      initialBalanceEntry.Sector = Sector.Empty;
      initialBalanceEntry.SubledgerAccountNumber = "";
      initialBalanceEntry.VoucherNumber = "";
      initialBalanceEntry.Concept = "";
      if (isCurrentBalance) {
        initialBalanceEntry.Debit = debit;
        initialBalanceEntry.Credit = credit;
      }
      initialBalanceEntry.CurrentBalance = balance;
      initialBalanceEntry.ItemType = TrialBalanceItemType.Total;
      initialBalanceEntry.IsCurrentBalance = isCurrentBalance;

      return initialBalanceEntry;
    }


    internal string GetTitle() {
      var accountNumber = AccountStatementCommand.Entry.AccountNumberForBalances;
      var accountName = AccountStatementCommand.Entry.AccountName;
      var subledgerAccountNumber = AccountStatementCommand.Entry.SubledgerAccountNumber;

      var title = "";
      
      if (accountNumber != string.Empty || accountNumber != "Empty") {
        title = $"{accountNumber} ";
      }

      if (accountName != string.Empty && accountName != "Empty") {
        title += $"{accountName} ";
      }

      if (subledgerAccountNumber.Length > 1) {

        if (accountNumber == string.Empty || accountNumber == "Empty") {

          title = $"{subledgerAccountNumber}: {accountName}";

        } else {
          title += $"({subledgerAccountNumber})";
        }
        
      }

      return title;
    }


    internal FixedList<AccountStatementEntry> GetVouchersListWithCurrentBalance(
                                                FixedList<AccountStatementEntry> orderingVouchers, 
                                                AccountStatementEntry initialAccountBalance) {
      
      List<AccountStatementEntry> returnedVouchersWithCurrentBalance =
                                    new List<AccountStatementEntry>(orderingVouchers).ToList();

      decimal initialBalance = initialAccountBalance.CurrentBalance;
      decimal currentBalance = initialBalance;
      decimal debit = 0, credit = 0;

      foreach (var voucher in returnedVouchersWithCurrentBalance) {
        if (voucher.DebtorCreditor == "D") {
          voucher.CurrentBalance = currentBalance + (voucher.Debit - voucher.Credit);
          currentBalance = currentBalance + (voucher.Debit - voucher.Credit);
        } else {
          voucher.CurrentBalance = currentBalance + (voucher.Credit - voucher.Debit);
          currentBalance = currentBalance + (voucher.Credit - voucher.Debit);
        }
        debit += voucher.Debit;
        credit += voucher.Credit;
      }
      

      AccountStatementEntry voucherWithCurrentBalance = 
                              GetInitialOrCurrentAccountBalance(
                                AccountStatementCommand.Entry.CurrentBalanceForBalances, 
                                true, debit, credit);

      if (voucherWithCurrentBalance != null) {
        returnedVouchersWithCurrentBalance.Add(voucherWithCurrentBalance);
      }

      return returnedVouchersWithCurrentBalance.ToFixedList();
    }


    internal FixedList<AccountStatementEntry> GetVoucherEntries() {

      AccountStatementCommandData commandData = VouchersByAccountCommandDataMapped();

      return AccountStatementDataService.GetVouchersByAccountEntries(commandData);
    }


    private AccountStatementCommandData VouchersByAccountCommandDataMapped() {

      var commandExtensions = new AccountStatementQueryExtensions(AccountStatementCommand);
      AccountStatementCommandData commandData = commandExtensions.MapToVouchersByAccountCommandData();

      return commandData;
    }

    #endregion Public methods


    #region Private methods



    #endregion Private methods

  } // class VouchersByAccountHelper

} // namespace Empiria.FinancialAccounting.Reporting.Domain
