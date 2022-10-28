/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Helper methods                          *
*  Type     : AccountStatementHelper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides helper methods to build acount statements.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.Reporting.AccountStatements.Adapters;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Domain {

  /// <summary>Provides helper methods to build acount statements.</summary>
  internal class AccountStatementHelper {

    private readonly AccountStatementQuery _buildQuery;

    internal AccountStatementHelper(AccountStatementQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      _buildQuery = buildQuery;
    }


    #region Public methods


    internal FixedList<AccountStatementEntry> CombineInitialBalanceWithVouchers(
                                                FixedList<AccountStatementEntry> vouchers,
                                                AccountStatementEntry initialBalance) {
      if (initialBalance == null) {
        return vouchers;
      }

      var totalBalanceAndVouchers = new List<AccountStatementEntry>();
      totalBalanceAndVouchers.Add(initialBalance);
      totalBalanceAndVouchers.AddRange(vouchers);

      return totalBalanceAndVouchers.ToFixedList();
    }


    internal FixedList<AccountStatementEntry> GetOrderingVouchers(
                                              FixedList<AccountStatementEntry> voucherEntries) {
      
      if (voucherEntries.Count==0) {
        return new FixedList<AccountStatementEntry>();
      }

      List<AccountStatementEntry> returnedVouchers = voucherEntries
                                                      .OrderBy(a => a.AccountingDate)
                                                      .ThenBy(a => a.Ledger.Number)
                                                      .ThenBy(a => a.AccountNumber)
                                                      .ThenBy(a => a.SubledgerAccountNumber)
                                                      .ThenBy(a => a.VoucherNumber)
                                                      .ToList();
      return returnedVouchers.ToFixedList();
    }


    internal AccountStatementEntry GetInitialBalance(FixedList<AccountStatementEntry> orderingVouchers) {

      if (orderingVouchers.Count == 0) {
        return new AccountStatementEntry();
      }

      var vouchersList = new List<AccountStatementEntry>(orderingVouchers).ToList();

      decimal balance = _buildQuery.Entry.CurrentBalanceForBalances;

      foreach (var voucher in vouchersList) {
        if (voucher.DebtorCreditor == "A") {
          balance += (voucher.Debit - voucher.Credit);
        } else {
          balance += (voucher.Credit - voucher.Debit);
        }
      }

      AccountStatementEntry inicialBalance = AccountStatementEntry.SetTotalAccountBalance(balance);
      return inicialBalance;
    }


    internal string GetTitle() {

      var accountNumber = _buildQuery.Entry.AccountNumberForBalances;

      var accountName = _buildQuery.Entry.AccountName;

      var subledgerAccountNumber = _buildQuery.Entry.SubledgerAccountNumber;

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
                                                AccountStatementEntry initialBalance) {

      if (orderingVouchers.Count == 0) {
        return new FixedList<AccountStatementEntry>();
      }

      var returnedVouchers = new List<AccountStatementEntry>(orderingVouchers).ToList();

      decimal flagBalance = initialBalance.CurrentBalance;
      decimal totalDebit, totalCredit;

      SumTotalByDebitCredit(returnedVouchers, flagBalance, out totalDebit, out totalCredit);

      AccountStatementEntry currentBalance = AccountStatementEntry.SetTotalAccountBalance(
                                                    _buildQuery.Entry.CurrentBalanceForBalances);

      if (currentBalance != null) {
        currentBalance.Debit = totalDebit;
        currentBalance.Credit = totalCredit;
        currentBalance.IsCurrentBalance = true;
        returnedVouchers.Add(currentBalance);
      }

      return returnedVouchers.ToFixedList();
    }

    
    internal FixedList<AccountStatementEntry> GetVoucherEntries() {
      var builder = new AccountStatementSqlClausesBuilder(_buildQuery);

      var sqlClauses = builder.BuildSqlClauses();

      return AccountStatementDataService.GetVouchersWithAccounts(sqlClauses);
    }

    #endregion Public methods

    #region Private methods

    private void SumTotalByDebitCredit(List<AccountStatementEntry> returnedVouchers,
                        decimal flagBalance, out decimal totalDebit, out decimal totalCredit) {
      totalCredit = 0;
      totalDebit = 0;

      foreach (var voucher in returnedVouchers) {
        if (voucher.DebtorCreditor == "D") {
          voucher.CurrentBalance = flagBalance + (voucher.Debit - voucher.Credit);
          flagBalance += (voucher.Debit - voucher.Credit);

        } else {
          voucher.CurrentBalance = flagBalance + (voucher.Credit - voucher.Debit);
          flagBalance += (voucher.Credit - voucher.Debit);

        }

        totalDebit += voucher.Debit;
        totalCredit += voucher.Credit;
      }

    }

    #endregion Private methods

  } // class AccountStatementHelper

} // namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Domain
