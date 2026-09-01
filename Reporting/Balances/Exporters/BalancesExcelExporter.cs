/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : BalancesExcelExporter                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with trial balance information.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Balances {

  /// <summary>Creates a Microsoft Excel file with trial balance information.</summary>
  internal class BalancesExcelExporter {

    private TrialBalanceQuery _query = new TrialBalanceQuery();

    private readonly FileTemplateConfig _templateConfig;

    private ExcelFile _excelFile;

    public BalancesExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(TrialBalanceDto trialBalance) {
      Assertion.Require(trialBalance, "trialBalance");

      _query = trialBalance.Query;

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader();

      SetTable(trialBalance);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    #region Private methods

    private void SetHeader() {
      _excelFile.SetCell($"A2", _templateConfig.Title);

      var subTitle = $"Del {_query.InitialPeriod.FromDate.ToString("dd/MMM/yyyy")} " +
                     $"al {_query.InitialPeriod.ToDate.ToString("dd/MMM/yyyy")}";

      if ((_query.ValuateBalances || _query.UseDefaultValuation)
          && _query.TrialBalanceType != TrialBalanceType.BalanzaDiferenciaDiariaPorMoneda) {
        
        subTitle += $". Tipo valorización: " +
                    $"{ExchangeRateType.Parse(_query.InitialPeriod.ExchangeRateTypeUID).Name}, " +
                    $"al {_query.InitialPeriod.ExchangeRateDate.ToString("dd/MMM/yyyy")}. ";
      }

      _excelFile.SetCell($"A3", subTitle);
    }


    private void SetTable(TrialBalanceDto trialBalance) {

      var balaceSetTable = new BalancesFillOutExcelExporter(_query);
      var saldosSetTable = new SaldosFillOutExcelExporter(_query);

      switch (trialBalance.Query.TrialBalanceType) {
        
        case TrialBalanceType.AnaliticoDeCuentas:
          balaceSetTable.FillOutAnaliticoDeCuentas(_excelFile,
                          trialBalance.Entries.Select(x => (AnaliticoDeCuentasEntryDto) x));
          return;

        case TrialBalanceType.Balanza:
          balaceSetTable.FillOutBalanza(_excelFile,
                          trialBalance.Entries.Select(x => (BalanzaTradicionalEntryDto) x));
          return;

        case TrialBalanceType.BalanzaConContabilidadesEnCascada:
          balaceSetTable.FillOutBalanzaConContabilidadesEnCascada(_excelFile,
                          trialBalance.Entries.Select(x => (BalanzaContabilidadesCascadaEntryDto) x));
          return;

        case TrialBalanceType.BalanzaDiferenciaDiariaPorMoneda:
          balaceSetTable.FillOutBalanzaDiferenciaDiariaPorMonedaV2(_excelFile,
                          trialBalance.Entries.Select(x => (BalanzaDiferenciaDiariaMonedaEntryDto) x));
          return;

        case TrialBalanceType.BalanzaDolarizada:
          balaceSetTable.FillOutBalanzaDolarizada(_excelFile,
                          trialBalance.Entries.Select(x => (BalanzaDolarizadaEntryDto) x));
          return;

        case TrialBalanceType.BalanzaEnColumnasPorMoneda:
          balaceSetTable.FillOutBalanzaColumnasPorMoneda(_excelFile,
                          trialBalance.Entries.Select(x => (BalanzaColumnasMonedaEntryDto) x));
          return;

        case TrialBalanceType.BalanzaValorizadaComparativa:
          SetBalanzaComparativaHeaders();
          balaceSetTable.FillOutBalanzaComparativa(_excelFile,
                          trialBalance.Entries.Select(x => (BalanzaComparativaEntryDto) x));
          return;

        case TrialBalanceType.ResumenAjusteAnual:
          
          balaceSetTable.FillOutResumenAjusteAnual(_excelFile,
                          trialBalance.Entries.Select(x => (ResumenAjusteAnualEntryDto) x));
          return;

        case TrialBalanceType.SaldosPorAuxiliar:
          saldosSetTable.FillOutSaldosPorAuxiliar(_excelFile,
                          trialBalance.Entries.Select(x => (SaldosPorAuxiliarEntryDto) x));
          return;

        case TrialBalanceType.SaldosPorCuenta:
          saldosSetTable.FillOutSaldosPorCuenta(_excelFile,
                          trialBalance.Entries.Select(x => (SaldosPorCuentaEntryDto) x),
                          trialBalance.Query.WithSubledgerAccount);
          return;

        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }


    private void SetBalanzaComparativaHeaders() {
      _excelFile.SetCell($"I4", $"{_query.InitialPeriod.ToDate.ToString("MMM_yyyy")}");
      _excelFile.SetCell($"K4", $"{_query.InitialPeriod.ToDate.ToString("MMM")}_VAL_A");
      _excelFile.SetCell($"N4", $"{_query.FinalPeriod.ToDate.ToString("MMM_yyyy")}");
      _excelFile.SetCell($"P4", $"{_query.FinalPeriod.ToDate.ToString("MMM")}_VAL_B");
    }


    #endregion Private methods


  }  // class BalancesExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Balances
