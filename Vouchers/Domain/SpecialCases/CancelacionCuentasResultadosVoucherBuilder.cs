/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Concrete Builder                        *
*  Type     : CancelacionCuentasResultadosVoucherBuilder License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds a voucher that cancels the balances of profit and loss accounts                         *
*             at a given date (cuentas de resultados).                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.SpecialCases {

  /// <summary>Builds a voucher that cancels the balances of profit and loss
  /// accounts at a given date (cuentas de resultados).</summary>
  internal class CancelacionCuentasResultadosVoucherBuilder : VoucherBuilder {

    internal CancelacionCuentasResultadosVoucherBuilder(VoucherSpecialCaseFields fields) : base(fields) {
      // no-op
    }

    #region Abstract Implements

    protected override FixedList<VoucherEntryFields> BuildVoucherEntries() {
      FixedList<AccountsListItem> cancelationRulesList = base.SpecialCaseType.AccountsList.GetItems();

      FixedList<TrialBalanceEntryDto> balances = GetBalances();

      var voucherEntries = new List<VoucherEntryFields>();

      foreach (var cancelationRule in cancelationRulesList) {
        FixedList<TrialBalanceEntryDto> accountsToCancelBalances = balances.FindAll(x => x.AccountNumberForBalances.StartsWith(cancelationRule.AccountNumber));

        var ruleVoucherEntries = new List<VoucherEntryFields>();

        foreach (var accountBalance in accountsToCancelBalances) {
          VoucherEntryFields voucherEntry = BuildVoucherEntry(accountBalance);

          if (voucherEntry != null) {     /// ToDo - WARNING: Remove this IF after fix locked balances. No nulls must be returned
            ruleVoucherEntries.Add(voucherEntry);
          }
        }

        FixedList<VoucherEntryFields> targetAccountVoucherEntries = BuildTargetAccountVoucherEntry(ruleVoucherEntries,
                                                                                                   cancelationRule.TargetAccountNumber);

        ruleVoucherEntries.AddRange(targetAccountVoucherEntries);

        voucherEntries.AddRange(ruleVoucherEntries);
      }

      Assertion.Require(voucherEntries.Count >= 2,
                        "No hay saldos de cuentas de resultados por cancelar a la fecha proporcionada.");

      return voucherEntries.ToFixedList();
    }

    #endregion Abstract Implements

    #region Helpers

    private FixedList<TrialBalanceEntryDto> GetBalances() {
      var query = new TrialBalanceQuery {
        TrialBalanceType = TrialBalanceType.GeneracionDeSaldos,
        AccountsChartUID = base.AccountsChart.UID,
        Ledgers = new[] { base.Ledger.UID },
        BalancesType = BalancesType.WithCurrentBalance,
        ShowCascadeBalances = true,
        WithSubledgerAccount = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = base.Fields.CalculationDate,
          ToDate = base.Fields.CalculationDate
        }
      };

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        var entries = usecases.BuildTrialBalance(query)
                              .Entries;

        return entries.Select(x => (TrialBalanceEntryDto) x)
                      .ToFixedList();
      }
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


    private VoucherEntryFields BuildVoucherEntry(TrialBalanceEntryDto accountBalance) {

      if (accountBalance.AccountNumberForBalances == "6.05.01.02.03.03" &&     // ToDo - WARNING: Remove this code after fix locked balances
          accountBalance.SubledgerAccountId <= 0) {
        return null;
      }

      if (accountBalance.DebtorCreditor == "Deudora" &&
          accountBalance.CurrentBalance > 0) {
        return BuildVoucherEntryFields(VoucherEntryType.Credit, accountBalance);

      } else if (accountBalance.DebtorCreditor == "Deudora" &&
                 accountBalance.CurrentBalance < 0) {

        return BuildVoucherEntryFields(VoucherEntryType.Debit, accountBalance);

      } else if (accountBalance.DebtorCreditor == "Acreedora" &&
                 accountBalance.CurrentBalance > 0) {
        return BuildVoucherEntryFields(VoucherEntryType.Debit, accountBalance);

      } else if (accountBalance.DebtorCreditor == "Acreedora" &&
                 accountBalance.CurrentBalance < 0) {
        return BuildVoucherEntryFields(VoucherEntryType.Credit, accountBalance);
      }

      throw Assertion.EnsureNoReachThisCode();
    }


    private VoucherEntryFields BuildVoucherEntryFields(VoucherEntryType entryType,
                                                       TrialBalanceEntryDto accountBalance) {
      var subledgerAccount = SubledgerAccount.Parse(accountBalance.SubledgerAccountId);

      return BuildVoucherEntryFields(entryType,
                                     accountBalance.AccountNumberForBalances,
                                     accountBalance.SectorCode,
                                     subledgerAccount,
                                     Math.Abs(accountBalance.CurrentBalance.Value)
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

  }  // class CancelacionCuentasResultadosVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
