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

    private readonly AccountStatementQuery query;

    internal AccountStatementHelper(AccountStatementQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      query = buildQuery;
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


    internal FixedList<AccountStatementEntry> CombineVouchersWithCurrentBalance(
                                              FixedList<AccountStatementEntry> voucherEntriesWithBalances,
                                              AccountStatementEntry entryWithCurrentBalance) {

      var returnedVouchers = new List<AccountStatementEntry>(voucherEntriesWithBalances).ToList();
      returnedVouchers.Add(entryWithCurrentBalance);

      return returnedVouchers.ToFixedList();
    }


    internal FixedList<AccountStatementEntry> GetEntriesByBalanceType(
             FixedList<AccountStatementEntry> voucherEntries) {

      if (query.Entry.ItemType == TrialBalanceItemType.Entry &&
          query.BalancesQuery.WithSubledgerAccount &&
          query.Entry.SubledgerAccountNumber.Length <= 1) {

        return voucherEntries.Where(a => a.SubledgerAccountNumber.Length <= 1).ToFixedList();

      }

      return voucherEntries;
    }



    internal AccountStatementEntry GetEntryWithCurrentBalance(
                                    FixedList<AccountStatementEntry> voucherEntries) {

      AccountStatementEntry currentBalance = AccountStatementEntry.SetTotalAccountBalance(
                                                    query.Entry.CurrentBalanceForBalances);
      currentBalance.Debit = voucherEntries.Sum(x => x.Debit);
      currentBalance.Credit = voucherEntries.Sum(x => x.Credit);
      currentBalance.ExchangeRate = voucherEntries.First().ExchangeRate;
      currentBalance.IsCurrentBalance = true;
      return currentBalance;
    }


    internal AccountStatementEntry GetInitialBalance(FixedList<AccountStatementEntry> orderingVouchers) {

      if (orderingVouchers.Count == 0) {
        return new AccountStatementEntry();
      }

      var vouchersList = new List<AccountStatementEntry>(orderingVouchers).ToList();
      decimal balance = query.Entry.CurrentBalanceForBalances;

      balance += vouchersList.Where(x => x.DebtorCreditor == "A").Sum(x => (x.Debit - x.Credit)) -
                 vouchersList.Where(x => x.DebtorCreditor == "D").Sum(x => (x.Debit - x.Credit));

      AccountStatementEntry initialBalance = AccountStatementEntry.SetTotalAccountBalance(balance);
      initialBalance.ExchangeRate = vouchersList.First().ExchangeRate;
      return initialBalance;
    }


    internal FixedList<AccountStatementEntry> GetOrderingByFilter(
     FixedList<AccountStatementEntry> voucherEntries) {

      List<AccountStatementEntry> returnedVouchers = GetOrderingByAccountStatementFilter(voucherEntries);

      returnedVouchers.AddRange(voucherEntries.Where(x => x.ItemType == TrialBalanceItemType.Total));

      return returnedVouchers.ToFixedList();
    }


    internal FixedList<AccountStatementEntry> GetInitialOrderingToCalculateBalances(
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

      var accountNumber = query.Entry.AccountNumberForBalances;

      var accountName = query.Entry.AccountName;

      var subledgerAccountNumber = query.Entry.SubledgerAccountNumber;

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


    internal FixedList<AccountStatementEntry> GetVoucherEntries() {
      var builder = new AccountStatementSqlClausesBuilder(query);

      //var sqlClauses = builder.BuildSqlClauses();
      string filtering = query.MapToFilterString();
      string ordering = query.MapToSortString();
      return AccountStatementDataService.GetVouchersWithAccounts(filtering, ordering);
    }


    internal FixedList<AccountStatementEntry> GetVoucherEntriesBalance(
                                              FixedList<AccountStatementEntry> orderingVouchers,
                                              AccountStatementEntry initialBalance) {

      if (orderingVouchers.Count == 0) {
        return new FixedList<AccountStatementEntry>();
      }
      var returnedVouchers = new List<AccountStatementEntry>(orderingVouchers).ToList();
      SumTotalByDebitCredit(returnedVouchers, initialBalance.CurrentBalance);

      return returnedVouchers.ToFixedList();
    }


    internal void RoundValorizedBalances(FixedList<AccountStatementEntry> vouchers) {

      if (query.BalancesQuery.UseDefaultValuation ||
          query.BalancesQuery.InitialPeriod.ExchangeRateTypeUID != string.Empty) {

        foreach (var entry in vouchers) {
          entry.RoundBalances();
        }
      }
    }


    internal void ValuateAccountStatementToExchangeRate(FixedList<AccountStatementEntry> accounts) {

      if (query.BalancesQuery.UseDefaultValuation ||
          query.BalancesQuery.InitialPeriod.ExchangeRateTypeUID != string.Empty) {

        FixedList<ExchangeRate> exchangeRates = GetExchangeRateListForDate();

        foreach (var account in accounts.Where(a => a.Currency.Distinct(Currency.MXN))) {

          var exchangeRate = exchangeRates.Find(
            a => a.ToCurrency.Equals(account.Currency) &&
            a.FromCurrency.Code == query.BalancesQuery.InitialPeriod.ValuateToCurrrencyUID);

          Assertion.Require(exchangeRate, $"No se ha registrado el tipo de cambio para la " +
                                          $"moneda {account.Currency.FullName} en la fecha proporcionada.");

          account.MultiplyBy(exchangeRate.Value);
        }
      }
    }

    #endregion Public methods

    #region Private methods


    private FixedList<ExchangeRate> GetExchangeRateListForDate() {

      if (query.BalancesQuery.UseDefaultValuation) {

        query.BalancesQuery.InitialPeriod.ExchangeRateTypeUID =
          ExchangeRateType.ValorizacionBanxico.UID;

        query.BalancesQuery.InitialPeriod.ValuateToCurrrencyUID = Currency.MXN.Code;

        query.BalancesQuery.InitialPeriod.ExchangeRateDate =
          query.BalancesQuery.InitialPeriod.ToDate;

      }
      return ExchangeRate.GetList(ExchangeRateType.Parse(
                                  query.BalancesQuery.InitialPeriod.ExchangeRateTypeUID),
                                  query.BalancesQuery.InitialPeriod.ExchangeRateDate);
    }


    private List<AccountStatementEntry> GetOrderingByAccountStatementFilter(
      FixedList<AccountStatementEntry> voucherEntries) {

      List<AccountStatementEntry> orderingVouchers =
        voucherEntries.Where(x => x.ItemType == TrialBalanceItemType.Entry).ToList();

      switch (query.OrderBy.SortType) {

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


    private List<AccountStatementEntry> GetOrderByAccountingDate(List<AccountStatementEntry> vouchers) {

      List<AccountStatementEntry> list = new List<AccountStatementEntry>();

      list = query.OrderBy.OrderType == AccountStatementOrderType.Descending
        ? vouchers.OrderByDescending(a => a.AccountingDate)
                  .ThenByDescending(a => a.Ledger.Number)
                  .ThenByDescending(a => a.AccountNumber)
                  .ThenByDescending(a => a.SubledgerAccountNumber)
                  .ThenByDescending(a => a.VoucherNumber)
                  .ToList()
        : vouchers.OrderBy(a => a.AccountingDate)
                  .ThenBy(a => a.Ledger.Number)
                  .ThenBy(a => a.AccountNumber)
                  .ThenBy(a => a.SubledgerAccountNumber)
                  .ThenBy(a => a.VoucherNumber)
                  .ToList();
      return list;
    }


    private List<AccountStatementEntry> GetOrderByAmount(List<AccountStatementEntry> vouchers) {

      List<AccountStatementEntry> list = new List<AccountStatementEntry>();

      list = query.OrderBy.OrderType == AccountStatementOrderType.Descending
        ? vouchers.OrderByDescending(a => a.Ledger.Number)
                                 .ThenByDescending(a => a.Debit)
                                 .ThenByDescending(a => a.Credit)
                                 .ThenByDescending(a => a.AccountNumber)
                                 .ThenByDescending(a => a.SubledgerAccountNumber)
                                 .ThenByDescending(a => a.VoucherNumber)
                                 .ToList()
        : vouchers.OrderBy(a => a.Ledger.Number)
                                 .ThenBy(a => a.Debit)
                                 .ThenBy(a => a.Credit)
                                 .ThenBy(a => a.AccountNumber)
                                 .ThenBy(a => a.SubledgerAccountNumber)
                                 .ThenBy(a => a.VoucherNumber)
                                 .ToList();
      return list;
    }


    private List<AccountStatementEntry> GetOrderByRecordingDate(List<AccountStatementEntry> vouchers) {

      List<AccountStatementEntry> list = new List<AccountStatementEntry>();

      list = query.OrderBy.OrderType == AccountStatementOrderType.Descending
        ? vouchers.OrderByDescending(a => a.Ledger.Number)
                                 .ThenByDescending(a => a.RecordingDate)
                                 .ThenByDescending(a => a.AccountNumber)
                                 .ThenByDescending(a => a.SubledgerAccountNumber)
                                 .ThenByDescending(a => a.VoucherNumber)
                                 .ToList()
        : vouchers.OrderBy(a => a.Ledger.Number)
                                 .ThenBy(a => a.RecordingDate)
                                 .ThenBy(a => a.AccountNumber)
                                 .ThenBy(a => a.SubledgerAccountNumber)
                                 .ThenBy(a => a.VoucherNumber)
                                 .ToList();
      return list;
    }


    private List<AccountStatementEntry> GetOrderBySubledgerAccount(List<AccountStatementEntry> vouchers) {

      List<AccountStatementEntry> list = new List<AccountStatementEntry>();

      list = query.OrderBy.OrderType == AccountStatementOrderType.Descending
        ? vouchers.OrderByDescending(a => a.Ledger.Number)
                                 .ThenByDescending(a => a.SubledgerAccountNumber)
                                 .ThenByDescending(a => a.AccountNumber)
                                 .ThenByDescending(a => a.VoucherNumber)
                                 .ThenByDescending(a => a.AccountingDate)
                                 .ToList()
        : vouchers.OrderBy(a => a.Ledger.Number)
                                 .ThenBy(a => a.SubledgerAccountNumber)
                                 .ThenBy(a => a.AccountNumber)
                                 .ThenBy(a => a.VoucherNumber)
                                 .ThenBy(a => a.AccountingDate)
                                 .ToList();
      return list;
    }


    private List<AccountStatementEntry> GetOrderByVoucherNumber(List<AccountStatementEntry> vouchers) {

      List<AccountStatementEntry> list = new List<AccountStatementEntry>();

      list = query.OrderBy.OrderType == AccountStatementOrderType.Descending
        ? vouchers.OrderByDescending(a => a.Ledger.Number)
                                 .ThenByDescending(a => a.VoucherNumber)
                                 .ThenByDescending(a => a.AccountNumber)
                                 .ThenByDescending(a => a.SubledgerAccountNumber)
                                 .ThenByDescending(a => a.AccountingDate)
                                 .ToList()
        : vouchers.OrderBy(a => a.Ledger.Number)
                                 .ThenBy(a => a.VoucherNumber)
                                 .ThenBy(a => a.AccountNumber)
                                 .ThenBy(a => a.SubledgerAccountNumber)
                                 .ThenBy(a => a.AccountingDate)
                                 .ToList();
      return list;
    }


    private void SumTotalByDebitCredit(List<AccountStatementEntry> returnedVouchers, decimal flagBalance) {

      foreach (var voucher in returnedVouchers) {
        if (voucher.DebtorCreditor == "D") {
          voucher.CurrentBalance = flagBalance + (voucher.Debit - voucher.Credit);
          flagBalance += (voucher.Debit - voucher.Credit);

        } else {
          voucher.CurrentBalance = flagBalance + (voucher.Credit - voucher.Debit);
          flagBalance += (voucher.Credit - voucher.Debit);

        }
      }

    }

    #endregion Private methods

  } // class AccountStatementHelper

} // namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Domain
