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

    public FixedList<NamedEntityDto> FinancialReportTypes(string accountsChartUID) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      var list = FinancialReportType.GetList(accountsChart);

      return list.MapToNamedEntityList();
    }


    public FinancialReportDto GenerateFinancialReport(FinancialReportCommand command) {
      Assertion.AssertObject(command, "command");

      var financialReportGenerator = new FinancialReportGenerator(command);

      FinancialReport financialReport = financialReportGenerator.BuildFinancialReport();

      return FinancialReportMapper.Map(financialReport);
    }

    #endregion Use cases

  }  // class FinancialReportsUseCases

}  // namespace Empiria.FinancialAccounting.FinancialReports.UseCases
