/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Use case interactor class               *
*  Type     : FinancialReportsUseCases                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to generate financial reports.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports.UseCases {

  /// <summary>Use cases used to generate financial reports.</summary>
  public class FinancialReportsUseCases : UseCase {

    #region Constructors and parsers

    protected FinancialReportsUseCases() {
      // no-op
    }

    static public FinancialReportsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<FinancialReportsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<FinancialReportTypeDto> FinancialReportTypes(string accountsChartUID) {
      Assertion.Require(accountsChartUID, nameof(accountsChartUID));

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      var list = FinancialReportType.GetList(accountsChart);

      return FinancialReportMapper.Map(list);
    }


    public FinancialReportDto GenerateFinancialReport(FinancialReportQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      var financialReportGenerator = new FinancialReportBuilder(buildQuery);

      FinancialReport financialReport = financialReportGenerator.Build();

      return FinancialReportMapper.Map(financialReport);
    }


    public FinancialReportDto GetFinancialReportBreakdown(string reportRowUID, FinancialReportQuery buildQuery) {
      Assertion.Require(reportRowUID, nameof(reportRowUID));
      Assertion.Require(buildQuery, nameof(buildQuery));

      var financialReportGenerator = new FinancialReportBuilder(buildQuery);

      FinancialReport breakdownReport = financialReportGenerator.GetBreakdown(reportRowUID);

      return FinancialReportMapper.MapBreakdown(breakdownReport);
    }

    #endregion Use cases

  }  // class FinancialReportsUseCases

}  // namespace Empiria.FinancialAccounting.FinancialReports.UseCases
