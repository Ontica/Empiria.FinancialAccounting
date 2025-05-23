/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Service provider                        *
*  Type     : TestsHelpers                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for balances tests.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.Reporting;
using Empiria.FinancialAccounting.Reporting.FiscalReports.Builders;

namespace Empiria.FinancialAccounting.Tests.Reporting {

  /// <summary>Provides services for balances tests.</summary>
  static public class TestsHelpers {

    static internal string BalanceDiffMsg(string column, string accountText,
                                          decimal expected, decimal sutValue) {

      return $"Diferencia en {column}, cuenta {accountText}. " +
             $"Valor esperado = {expected.ToString("C2")}, " +
             $"Reporte = {sutValue.ToString("C2")}. " +
             $"Diferencia = {(expected - sutValue).ToString("C2")}";
    }


    static internal FixedList<BalanzaSatEntry> GetBalanzaSat(DateTime fromDate, DateTime toDate) {

      ReportBuilderQuery query = new ReportBuilderQuery {
        AccountsChartUID = AccountsChart.IFRS.UID,
        ReportType = ReportTypes.BalanzaSAT,
        FromDate = fromDate,
        ToDate = toDate
      };

      return ExecuteTrialBalance<BalanzaSatEntry>(query);
    }


    static internal CoreBalanceEntries GetCoreBalanceEntries(DateTime fromDate, DateTime toDate,
                                                             ExchangeRateType exchangeRateType) {
      var query = new TrialBalanceQuery() {
        AccountsChartUID = AccountsChart.IFRS.UID,
        TrialBalanceType = TrialBalanceType.Balanza,
        BalancesType = BalancesType.AllAccounts,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate,
        }
      };

      return new CoreBalanceEntries(exchangeRateType, query);
    }


    static internal FixedList<T> ExecuteTrialBalance<T>(ReportBuilderQuery query) {

      using (var service = ReportingService.ServiceInteractor()) {

        var entries = service.GenerateReport(query).Entries;

        return entries.Select(x => (T) x).ToFixedList();
      }
    }

  }
}
