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
                                                AccountStatementEntry initialBalance,
                                                FixedList<AccountStatementEntry> vouchers) {
      if (initialBalance == null) {
        return vouchers;
      }

      var initialBalanceAndVouchers = new List<AccountStatementEntry>();
      initialBalanceAndVouchers.Add(initialBalance);
      initialBalanceAndVouchers.AddRange(vouchers);

      return initialBalanceAndVouchers.ToFixedList();
    }


    internal FixedList<AccountStatementEntry> GetEntriesByBalanceType(
             FixedList<AccountStatementEntry> voucherEntries) {

      if (_buildQuery.Entry.ItemType == BalanceEngine.TrialBalanceItemType.Entry &&
          _buildQuery.BalancesQuery.WithSubledgerAccount &&
          _buildQuery.Entry.SubledgerAccountNumber.Length <= 1) {

        return voucherEntries.Where(a => a.SubledgerAccountNumber.Length <= 1).ToFixedList();

      }

      return voucherEntries;
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


    private List<AccountStatementEntry> GetOrderByAccountingDate(List<AccountStatementEntry> vouchers) {

      List<AccountStatementEntry> list = new List<AccountStatementEntry>();

      if (_buildQuery.OrderBy.OrderType == AccountStatementOrderType.Descending) {

        list = vouchers.OrderByDescending(a => a.AccountingDate)
                                 .ThenByDescending(a => a.Ledger.Number)
                                 .ThenByDescending(a => a.AccountNumber)
                                 .ThenByDescending(a => a.SubledgerAccountNumber)
                                 .ThenByDescending(a => a.VoucherNumber)
                                 .ToList();

      } else {
        list = vouchers.OrderBy(a => a.AccountingDate)
                                 .ThenBy(a => a.Ledger.Number)
                                 .ThenBy(a => a.AccountNumber)
                                 .ThenBy(a => a.SubledgerAccountNumber)
                                 .ThenBy(a => a.VoucherNumber)
                                 .ToList();
      }
      return list;
    }


    private List<AccountStatementEntry> GetOrderByAmount(List<AccountStatementEntry> vouchers) {

      List<AccountStatementEntry> list = new List<AccountStatementEntry>();

      if (_buildQuery.OrderBy.OrderType == AccountStatementOrderType.Descending) {
        list = vouchers.OrderByDescending(a => a.Ledger.Number)
                                 .ThenByDescending(a => a.Debit)
                                 .ThenByDescending(a => a.Credit)
                                 .ThenByDescending(a => a.AccountNumber)
                                 .ThenByDescending(a => a.SubledgerAccountNumber)
                                 .ThenByDescending(a => a.VoucherNumber)
                                 .ToList();
      } else {
        list = vouchers.OrderBy(a => a.Ledger.Number)
                                 .ThenBy(a => a.Debit)
                                 .ThenBy(a => a.Credit)
                                 .ThenBy(a => a.AccountNumber)
                                 .ThenBy(a => a.SubledgerAccountNumber)
                                 .ThenBy(a => a.VoucherNumber)
                                 .ToList();
      }
      return list;
    }


    private List<AccountStatementEntry> GetOrderByRecordingDate(List<AccountStatementEntry> vouchers) {

      List<AccountStatementEntry> list = new List<AccountStatementEntry>();

      if (_buildQuery.OrderBy.OrderType == AccountStatementOrderType.Descending) {
        list = vouchers.OrderByDescending(a => a.Ledger.Number)
                                 .ThenByDescending(a => a.RecordingDate)
                                 .ThenByDescending(a => a.AccountNumber)
                                 .ThenByDescending(a => a.SubledgerAccountNumber)
                                 .ThenByDescending(a => a.VoucherNumber)
                                 .ToList();
      } else {
        list = vouchers.OrderBy(a => a.Ledger.Number)
                                 .ThenBy(a => a.RecordingDate)
                                 .ThenBy(a => a.AccountNumber)
                                 .ThenBy(a => a.SubledgerAccountNumber)
                                 .ThenBy(a => a.VoucherNumber)
                                 .ToList();
      }
      return list;
    }


    private List<AccountStatementEntry> GetOrderBySubledgerAccount(List<AccountStatementEntry> vouchers) {

      List<AccountStatementEntry> list = new List<AccountStatementEntry>();

      if (_buildQuery.OrderBy.OrderType == AccountStatementOrderType.Descending) {
        list = vouchers.OrderByDescending(a => a.Ledger.Number)
                                 .ThenByDescending(a => a.SubledgerAccountNumber)
                                 .ThenByDescending(a => a.AccountNumber)
                                 .ThenByDescending(a => a.VoucherNumber)
                                 .ThenByDescending(a => a.AccountingDate)
                                 .ToList();
      } else {
        list = vouchers.OrderBy(a => a.Ledger.Number)
                                 .ThenBy(a => a.SubledgerAccountNumber)
                                 .ThenBy(a => a.AccountNumber)
                                 .ThenBy(a => a.VoucherNumber)
                                 .ThenBy(a => a.AccountingDate)
                                 .ToList();
      }
      return list;
    }


    private List<AccountStatementEntry> GetOrderByVoucherNumber(List<AccountStatementEntry> vouchers) {

      List<AccountStatementEntry> list = new List<AccountStatementEntry>();

      if (_buildQuery.OrderBy.OrderType == AccountStatementOrderType.Descending) {
        list = vouchers.OrderByDescending(a => a.Ledger.Number)
                                 .ThenByDescending(a => a.VoucherNumber)
                                 .ThenByDescending(a => a.AccountNumber)
                                 .ThenByDescending(a => a.SubledgerAccountNumber)
                                 .ThenByDescending(a => a.AccountingDate)
                                 .ToList();
      } else {
        list = vouchers.OrderBy(a => a.Ledger.Number)
                                 .ThenBy(a => a.VoucherNumber)
                                 .ThenBy(a => a.AccountNumber)
                                 .ThenBy(a => a.SubledgerAccountNumber)
                                 .ThenBy(a => a.AccountingDate)
                                 .ToList();
      }
      return list;
    }


    private List<AccountStatementEntry> GetOrderingByAccountStatementFilter(
      FixedList<AccountStatementEntry> voucherEntries) {

      List<AccountStatementEntry> orderingVouchers =
        voucherEntries.Where(x => x.ItemType == TrialBalanceItemType.Entry).ToList();

      switch (_buildQuery.OrderBy.SortType) {

        case AccountStatementOrder.AccountingDate:

          orderingVouchers = GetOrderByAccountingDate(orderingVouchers);
          return orderingVouchers.ToList();

        case AccountStatementOrder.Amount:

          orderingVouchers = GetOrderByAmount(orderingVouchers);
          return orderingVouchers.ToList();

        case AccountStatementOrder.RecordingDate:

          orderingVouchers = GetOrderByRecordingDate(orderingVouchers);
          return orderingVouchers.ToList();

        case AccountStatementOrder.SubledgerAccount:

          orderingVouchers = GetOrderBySubledgerAccount(orderingVouchers);
          return orderingVouchers.ToList();

        case AccountStatementOrder.VoucherNumber:

          orderingVouchers = GetOrderByVoucherNumber(orderingVouchers);
          return orderingVouchers.ToList();

        default:
          return orderingVouchers.ToList();
      }
    }


    internal FixedList<AccountStatementEntry> GetOrderingByFilter(
     FixedList<AccountStatementEntry> voucherEntries) {
      var returnedVouchers = GetOrderingByAccountStatementFilter(voucherEntries);

      returnedVouchers.AddRange(voucherEntries.Where(x => x.ItemType == TrialBalanceItemType.Total));

      return returnedVouchers.ToFixedList();
    }


    internal FixedList<AccountStatementEntry> GetOrderingVouchers(
                                              FixedList<AccountStatementEntry> voucherEntries) {

      if (voucherEntries.Count == 0) {
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
