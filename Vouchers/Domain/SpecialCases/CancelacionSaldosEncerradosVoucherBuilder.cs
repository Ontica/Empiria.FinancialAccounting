/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Concrete Builder                        *
*  Type     : CancelacionSaldosEncerradosVoucherBuilder  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds a voucher that cancels locked balances for changes in the charts of accounts.           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.SpecialCases {

  /// <summary>Builds a voucher that cancels locked balances for changes in the charts of accounts.</summary>
  internal class CancelacionSaldosEncerradosVoucherBuilder : VoucherBuilder {

    internal CancelacionSaldosEncerradosVoucherBuilder(VoucherSpecialCaseFields fields) : base(fields) {
      // no-op
    }

    #region Abstract Implements

    protected override FixedList<VoucherEntryFields> BuildVoucherEntries() {
      FixedList<SaldosEncerradosEntryDto> lockedBalances = GetLockedBalances();

      var voucherEntries = new List<VoucherEntryFields>();

      foreach (var lockedBalance in lockedBalances) {
        VoucherEntryFields voucherEntry = BuildVoucherEntry(lockedBalance);

        voucherEntries.Add(voucherEntry);
      }

      FixedList<VoucherEntryFields> targetAccountVoucherEntries =
                                              BuildTargetAccountVoucherEntry(voucherEntries, "9.03.14");

      voucherEntries.AddRange(targetAccountVoucherEntries);

      return voucherEntries.ToFixedList();
    }

    #endregion Abstract Implements

    #region Helpers

    private FixedList<SaldosEncerradosEntryDto> GetLockedBalances() {
      var query = new SaldosEncerradosQuery {
        AccountsChartUID = base.AccountsChart.UID,
        FromDate = base.Fields.CalculationDate,
        ToDate = base.Fields.CalculationDate,
        LedgerUID = base.Fields.LedgerUID
      };

      var builder = new SaldosEncerradosService(query);

      FixedList <SaldosEncerradosEntryDto> items = builder.BuildEntries();

      Assertion.Require(items.Count > 0,
                        "No hay saldos encerrados por cancelar en la fecha proporcionada.");

      return items;
    }


    private FixedList<VoucherEntryFields> BuildTargetAccountVoucherEntry(List<VoucherEntryFields> accumulatedEntries,
                                                                         string targetAccountNumber) {
      decimal totalDebits = 0m;
      decimal totalCredits = 0m;

      foreach (var entry in accumulatedEntries) {
        if (entry.VoucherEntryType == VoucherEntryType.Debit) {
          totalDebits += entry.Amount;
        } else {
          totalCredits += entry.Amount;
        }
      }

      var entries = new List<VoucherEntryFields>();

      if (totalDebits != 0) {
        entries.Add(BuildVoucherEntryFields(VoucherEntryType.Credit, targetAccountNumber,
                                            "00", SubledgerAccount.Empty, totalDebits));
      }
      if (totalCredits != 0) {
        entries.Add(BuildVoucherEntryFields(VoucherEntryType.Debit, targetAccountNumber,
                                            "00", SubledgerAccount.Empty, totalCredits));
      }

      return entries.ToFixedList();
    }


    private VoucherEntryFields BuildVoucherEntry(SaldosEncerradosEntryDto lockedBalance) {

      if (lockedBalance.DebtorCreditor == "Deudora" &&
          lockedBalance.LockedBalance > 0) {
        return BuildVoucherEntryFields(VoucherEntryType.Credit, lockedBalance);

      } else if (lockedBalance.DebtorCreditor == "Deudora" &&
                 lockedBalance.LockedBalance < 0) {

        return BuildVoucherEntryFields(VoucherEntryType.Debit, lockedBalance);

      } else if (lockedBalance.DebtorCreditor == "Acreedora" &&
                 lockedBalance.LockedBalance > 0) {
        return BuildVoucherEntryFields(VoucherEntryType.Debit, lockedBalance);

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
                                     Math.Abs(lockedBalance.LockedBalance)
                                    );
    }


    private VoucherEntryFields BuildVoucherEntryFields(VoucherEntryType entryType,
                                                   string accountNumber,
                                                   string sectorCode,
                                                   SubledgerAccount subledgerAccount,
                                                   decimal balance) {

      StandardAccount stdAccount = base.AccountsChart.GetStandardAccount(accountNumber);

      LedgerAccount ledgerAccount = base.Ledger.AssignAccount(stdAccount);

      return new VoucherEntryFields {
        Amount = balance,
        BaseCurrencyAmount = balance,
        CurrencyUID = base.Ledger.BaseCurrency.UID,
        SectorId = Sector.Parse(sectorCode).Id,
        SubledgerAccountId = subledgerAccount.Id,
        SubledgerAccountNumber = subledgerAccount.IsEmptyInstance ?
                                                    string.Empty : subledgerAccount.Number,
        StandardAccountId = stdAccount.Id,
        LedgerAccountId = ledgerAccount.Id,
        VoucherEntryType = entryType
      };
    }

    #endregion Helpers

  }  // class CancelacionSaldosEncerradosVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
