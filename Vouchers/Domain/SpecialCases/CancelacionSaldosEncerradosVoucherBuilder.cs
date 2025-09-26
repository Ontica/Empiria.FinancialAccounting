/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                           Component : Domain Layer                         *
*  Assembly : FinancialAccounting.Vouchers.dll              Pattern   : Concrete Builder                     *
*  Type     : CancelacionSaldosEncerradosVoucherBuilder     License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Builds a voucher that cancels locked balances because charts of accounts changes.              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.SpecialCases {

  /// <summary>Builds a voucher that cancels locked balances because charts of accounts changes.</summary>
  internal class CancelacionSaldosEncerradosVoucherBuilder : VoucherBuilder {

    static private readonly string CONTROL_ACCOUNT_NO = "9.03.14";

    internal CancelacionSaldosEncerradosVoucherBuilder(VoucherSpecialCaseFields fields) : base(fields) {
      // no-op
    }

    #region Abstract Implements

    protected override FixedList<VoucherEntryFields> BuildVoucherEntries() {
      FixedList<SaldosEncerradosEntryDto> lockedBalances = GetLockedBalances();

      var voucherEntries = new List<VoucherEntryFields>();

      voucherEntries.AddRange(BuildDebitVoucherEntries(lockedBalances));

      voucherEntries.AddRange(BuildCreditVoucherEntries(lockedBalances));

      return voucherEntries.ToFixedList();
    }

    #endregion Abstract Implements

    #region Builders

    private FixedList<VoucherEntryFields> BuildCreditVoucherEntries(FixedList<SaldosEncerradosEntryDto> lockedBalances) {

      var creditLockedBalances = FilterCreditLockedBalances(lockedBalances);

      if (creditLockedBalances.Count == 0) {
        return new FixedList<VoucherEntryFields>();
      }

      var voucherEntries = new List<VoucherEntryFields>(creditLockedBalances.Count + 8);

      foreach (var currency in GetCurrenciesList(creditLockedBalances)) {

        var to_process = creditLockedBalances.FindAll(x => x.CurrencyCode == currency.Code);

        var byCurrencyVoucherEntries = BuildVoucherEntries(to_process, VoucherEntryType.Debit,
                                                           currency, CONTROL_ACCOUNT_NO);

        voucherEntries.AddRange(byCurrencyVoucherEntries);
      }

      return voucherEntries.ToFixedList();
    }

    private FixedList<VoucherEntryFields> BuildDebitVoucherEntries(FixedList<SaldosEncerradosEntryDto> lockedBalances) {

      var debitLockedBalances = FilterDebitLockedBalances(lockedBalances);

      if (debitLockedBalances.Count == 0) {
        return new FixedList<VoucherEntryFields>();
      }

      var voucherEntries = new List<VoucherEntryFields>(debitLockedBalances.Count + 8);

      foreach (var currency in GetCurrenciesList(debitLockedBalances)) {

        var to_process = debitLockedBalances.FindAll(x => x.CurrencyCode == currency.Code);

        var byCurrencyVoucherEntries = BuildVoucherEntries(to_process, VoucherEntryType.Credit,
                                                           currency, CONTROL_ACCOUNT_NO);

        voucherEntries.AddRange(byCurrencyVoucherEntries);
      }

      return voucherEntries.ToFixedList();
    }


    private FixedList<SaldosEncerradosEntryDto> GetLockedBalances() {
      var query = new SaldosEncerradosQuery {
        AccountsChartUID = base.AccountsChart.UID,
        FromDate = base.Fields.DatePeriod.FromDate,
        ToDate = base.Fields.DatePeriod.ToDate,
        LedgerUID = base.Fields.LedgerUID
      };

      var builder = new SaldosEncerradosService(query);

      FixedList<SaldosEncerradosEntryDto> items = builder.BuildEntries();

      Assertion.Require(items.Count > 0,
                        "No hay saldos encerrados por cancelar en la fecha proporcionada.");

      return items;
    }


    private VoucherEntryFields BuildTargetAccountVoucherEntry(List<VoucherEntryFields> accumulatedEntries,
                                                              VoucherEntryType targetEntryType,
                                                              string targetAccountNumber, Currency currency) {

      decimal total = accumulatedEntries.FindAll(x => x.Currency.Equals(currency))
                                        .Sum(x => x.Amount);

      return BuildVoucherEntryFields(targetEntryType, targetAccountNumber,
                                     "00", SubledgerAccount.Empty, total, currency);
    }


    private VoucherEntryFields BuildVoucherEntry(SaldosEncerradosEntryDto lockedBalance) {

      if (lockedBalance.DebtorCreditor == "Deudora" &&
          lockedBalance.LockedBalance < 0) {

        return BuildVoucherEntryFields(VoucherEntryType.Debit, lockedBalance);

      } else if (lockedBalance.DebtorCreditor == "Acreedora" &&
                 lockedBalance.LockedBalance > 0) {

        return BuildVoucherEntryFields(VoucherEntryType.Debit, lockedBalance);

      } else if (lockedBalance.DebtorCreditor == "Deudora" &&
                 lockedBalance.LockedBalance > 0) {

        return BuildVoucherEntryFields(VoucherEntryType.Credit, lockedBalance);

      } else if (lockedBalance.DebtorCreditor == "Acreedora" &&
                 lockedBalance.LockedBalance < 0) {

        return BuildVoucherEntryFields(VoucherEntryType.Credit, lockedBalance);
      }

      throw Assertion.EnsureNoReachThisCode();
    }


    private VoucherEntryFields BuildVoucherEntryFields(VoucherEntryType entryType,
                                                       SaldosEncerradosEntryDto lockedBalance) {
      SubledgerAccount subledgerAccount = SubledgerAccount.Empty;

      if (!string.IsNullOrWhiteSpace(lockedBalance.SubledgerAccount)) {
        subledgerAccount = base.Ledger.TryGetSubledgerAccount(lockedBalance.SubledgerAccount);
      }

      return BuildVoucherEntryFields(entryType,
                                     lockedBalance.AccountNumber,
                                     lockedBalance.SectorCode,
                                     subledgerAccount,
                                     Math.Abs(lockedBalance.LockedBalance),
                                     Currency.Parse(lockedBalance.CurrencyCode)
                                    );
    }


    private VoucherEntryFields BuildVoucherEntryFields(VoucherEntryType entryType,
                                                       string accountNumber,
                                                       string sectorCode,
                                                       SubledgerAccount subledgerAccount,
                                                       decimal balance,
                                                       Currency currency) {

      StandardAccount stdAccount = base.AccountsChart.GetStandardAccount(accountNumber);

      LedgerAccount ledgerAccount = base.Ledger.AssignAccount(stdAccount);

      return new VoucherEntryFields {
        Amount = balance,
        BaseCurrencyAmount = balance,
        CurrencyUID = currency.UID,
        SectorId = Sector.Parse(sectorCode).Id,
        SubledgerAccountId = subledgerAccount.Id,
        SubledgerAccountNumber = subledgerAccount.IsEmptyInstance ?
                                                    string.Empty : subledgerAccount.Number,
        StandardAccountId = stdAccount.Id,
        LedgerAccountId = ledgerAccount.Id,
        VoucherEntryType = entryType
      };
    }

    private IEnumerable<VoucherEntryFields> BuildVoucherEntries(FixedList<SaldosEncerradosEntryDto> to_process,
                                                                VoucherEntryType totalEntryType,
                                                                Currency totalCurrency,
                                                                string totalControlAccountNo) {

      var voucherEntries = new List<VoucherEntryFields>(to_process.Count + 1);

      foreach (var lockedBalance in to_process) {

        VoucherEntryFields voucherEntry = BuildVoucherEntry(lockedBalance);

        voucherEntries.Add(voucherEntry);
      }

      VoucherEntryFields totalEntry = BuildTargetAccountVoucherEntry(voucherEntries,
                                                                     totalEntryType,
                                                                     totalControlAccountNo,
                                                                     totalCurrency);

      voucherEntries.Add(totalEntry);

      return voucherEntries;
    }

    #endregion Builders

    #region Helpers

    static private FixedList<SaldosEncerradosEntryDto> FilterCreditLockedBalances(FixedList<SaldosEncerradosEntryDto> items) {
      var filtered = new List<SaldosEncerradosEntryDto>(items.Count);

      var chunk = items.FindAll(x => x.DebtorCreditor == "Deudora" && x.LockedBalance > 0);
      filtered.AddRange(chunk);

      chunk = items.FindAll(x => x.DebtorCreditor == "Acreedora" && x.LockedBalance < 0);
      filtered.AddRange(chunk);

      return filtered.ToFixedList();
    }


    static private FixedList<SaldosEncerradosEntryDto> FilterDebitLockedBalances(FixedList<SaldosEncerradosEntryDto> items) {
      var filtered = new List<SaldosEncerradosEntryDto>(items.Count);

      var chunk = items.FindAll(x => x.DebtorCreditor == "Deudora" && x.LockedBalance < 0);
      filtered.AddRange(chunk);

      chunk = items.FindAll(x => x.DebtorCreditor == "Acreedora" && x.LockedBalance > 0);
      filtered.AddRange(chunk);

      return filtered.ToFixedList();
    }


    static private IEnumerable<Currency> GetCurrenciesList(FixedList<SaldosEncerradosEntryDto> lockedBalances) {
      return lockedBalances.SelectDistinct(x => Currency.Parse(x.CurrencyCode));
    }

    #endregion Helpers

  }  // class CancelacionSaldosEncerradosVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
