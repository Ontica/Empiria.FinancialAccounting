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

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.Data;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Provides helper methods to build acount statements.</summary>
  internal class AccountStatementHelper {

    private readonly AccountStatementQuery _buildQuery;

    internal AccountStatementHelper(AccountStatementQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      _buildQuery = buildQuery;
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

      decimal initialBalance = _buildQuery.Entry.CurrentBalanceForBalances;

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
                                                AccountStatementEntry initialAccountBalance) {

      List<AccountStatementEntry> returnedVouchersWithCurrentBalance =
                                    new List<AccountStatementEntry>(orderingVouchers).ToList();

      decimal initialBalance = initialAccountBalance.CurrentBalance;
      decimal currentBalance = initialBalance;
      decimal debit = 0;
      decimal credit = 0;

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


      AccountStatementEntry voucherWithCurrentBalance = GetInitialOrCurrentAccountBalance(
                               _buildQuery.Entry.CurrentBalanceForBalances, true, debit, credit);

      if (voucherWithCurrentBalance != null) {
        returnedVouchersWithCurrentBalance.Add(voucherWithCurrentBalance);
      }

      return returnedVouchersWithCurrentBalance.ToFixedList();
    }


    internal FixedList<AccountStatementEntry> GetVoucherEntries() {
      var builder = new AccountStatementSqlClausesBuilder(_buildQuery);

      var sqlClauses = builder.BuildSqlClauses();

      return AccountStatementDataService.GetVouchersWithAccounts(sqlClauses);
    }

    #endregion Public methods

  } // class AccountStatementHelper

} // namespace Empiria.FinancialAccounting.Reporting.Domain
