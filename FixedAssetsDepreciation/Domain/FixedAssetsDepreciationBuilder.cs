/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Fixed Assets Depreciation                         Component : Domain Layer                     *
*  Assembly : FinancialAccounting.FixedAssetsDepreciation.dll   Pattern   : Service provider                 *
*  Type     : FixedAssetsDepreciationBuilder                    License   : Please read LICENSE.txt file     *
*                                                                                                            *
*  Summary  : Generates base data for fixed assets depreciation.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.AccountsLists.SpecialCases;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.FixedAssetsDepreciation {

  /// <summary>Generates base data for fixed assets depreciation.</summary>
  public class FixedAssetsDepreciationBuilder {

    private readonly DateTime _date;
    private readonly string[] _ledgers;

    #region Constructors

    public FixedAssetsDepreciationBuilder(DateTime date, string[] ledgers) {
      _date = date;
      _ledgers = ledgers;
    }

    #endregion Constructors

    #region Public methods

    public FixedList<FixedAssetsDepreciationEntry> Build() {

      var entries = new List<FixedAssetsDepreciationEntry>(2048);

      BuildValorHistoricoEntries(entries);

      BuildDepreciacionAcumuladaEntries(entries);

      SetClassifications(entries);

      BuildRevaluacionEntries(entries);

      BuildDepreciacionAcumuladaRevaluacionEntries(entries);

      entries = Sort(entries);

      return entries.ToFixedList();
    }


    private void BuildValorHistoricoEntries(List<FixedAssetsDepreciationEntry> list) {
      FixedList<SaldosPorCuentaEntryDto> incomeBalances = GetBalances("1.13.01");

      foreach (var balance in incomeBalances) {
        FixedAssetsDepreciationEntry entry = ConvertBalanceToListEntry(balance);

        var item = list.Find(x => x.AuxiliarHistorico.Number == balance.SubledgerAccountNumber);

        if (item == null) {
          entry.ValorHistorico = balance.CurrentBalanceForBalances;
          list.Add(entry);
        } else {
          item.ValorHistorico = balance.CurrentBalanceForBalances;
        }
      }
    }


    private void BuildDepreciacionAcumuladaEntries(List<FixedAssetsDepreciationEntry> list) {
      FixedList<SaldosPorCuentaEntryDto> incomeBalances = GetBalances("3.06.01");

      foreach (var balance in incomeBalances) {
        FixedAssetsDepreciationEntry entry = ConvertBalanceToListEntry(balance);

        var item = list.Find(x => x.AuxiliarHistorico.Number == balance.SubledgerAccountNumber);

        if (item == null) {
          entry.DepreciacionAcumuladaRegistradaContablemente = balance.CurrentBalanceForBalances;
          list.Add(entry);
        } else {
          item.DepreciacionAcumuladaRegistradaContablemente = balance.CurrentBalanceForBalances;
        }
      }
    }


    private void BuildRevaluacionEntries(List<FixedAssetsDepreciationEntry> list) {
      FixedList<SaldosPorCuentaEntryDto> incomeBalances = GetBalances("1.13.02");

      foreach (var balance in incomeBalances) {

        var item = list.Find(x => x.AuxiliarRevaluacion.Number == balance.SubledgerAccountNumber);

        if (item == null) {
          // no-op
        } else {
          item.Revaluacion = balance.CurrentBalanceForBalances;
        }
      }
    }

    private void BuildDepreciacionAcumuladaRevaluacionEntries(List<FixedAssetsDepreciationEntry> list) {
      FixedList<SaldosPorCuentaEntryDto> incomeBalances = GetBalances("3.06.02");

      foreach (var balance in incomeBalances) {

        var item = list.Find(x => x.AuxiliarRevaluacion.Number == balance.SubledgerAccountNumber);

        if (item == null) {
          // no-op
        } else {
          item.DepreciacionAcumuladaDeLaRevaluacionRegistradaContablemente = balance.CurrentBalanceForBalances;
        }
      }
    }

    private FixedAssetsDepreciationEntry ConvertBalanceToListEntry(SaldosPorCuentaEntryDto balance) {
      return new FixedAssetsDepreciationEntry(Ledger.Parse(balance.LedgerUID),
                                              SubledgerAccount.Parse(balance.SubledgerAccountId),
                                              _date);
    }


    private void SetClassifications(List<FixedAssetsDepreciationEntry> list) {
      var listItems = DepreciacionActivoFijoList.Parse().GetItems();

      foreach (var item in list) {
        var activoFijoEntry = listItems.Find(x => x.Ledger.Equals(item.Ledger) &&
                                                  x.AuxiliarHistorico.Number == item.AuxiliarHistorico.Number);

        if (activoFijoEntry == null) {
          // no-op
        } else {
          item.SetValues(activoFijoEntry);
        }
      }
    }


    private List<FixedAssetsDepreciationEntry> Sort(List<FixedAssetsDepreciationEntry> entries) {
      return entries.OrderBy(x => x.Ledger.Number)
                    .ThenBy(x => x.AuxiliarHistorico.Number)
                    .ToList();
    }

    #endregion Public methods

    #region Private methods


    private FixedList<SaldosPorCuentaEntryDto> GetBalances(string accountNumber) {

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceQuery trialBalanceQuery = this.MapToTrialBalanceQuery(accountNumber);

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(trialBalanceQuery);

        var entries = trialBalance.Entries.FindAll(x => x.ItemType == TrialBalanceItemType.Entry);

        return entries.Select(x => (SaldosPorCuentaEntryDto) x)
                      .ToFixedList();
      }
    }

    private TrialBalanceQuery MapToTrialBalanceQuery(string account) {

      return new TrialBalanceQuery {
        TrialBalanceType = TrialBalanceType.SaldosPorCuenta,
        AccountsChartUID = AccountsChart.IFRS.UID,
        BalancesType = BalancesType.WithCurrentBalance,
        ShowCascadeBalances = true,
        WithSubledgerAccount = true,
        WithAverageBalance = false,
        FromAccount = account,
        ToAccount = account,
        Ledgers = _ledgers,
        InitialPeriod = new BalancesPeriod {
          FromDate = new DateTime(_date.Year, _date.Month, 1),
          ToDate = _date
        },
        IsOperationalReport = true,
      };

    }

    #endregion Private methods

  } // class FixedAssetsDepreciationBuilder

} // namespace Empiria.FinancialAccounting.FixedAssetsDepreciation
