/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Use case interactor class               *
*  Type     : FinancialReportDesignUseCases              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to design financial reports.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports.UseCases {

  /// <summary>Use cases used to design financial reports.</summary>
  public class FinancialReportDesignUseCases : UseCase {

    #region Constructors and parsers

    protected FinancialReportDesignUseCases() {
      // no-op
    }

    static public FinancialReportDesignUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<FinancialReportDesignUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public FinancialReportDesignDto FinancialReportDesign(string financialReportTypeUID) {
      Assertion.Require(financialReportTypeUID, "financialReportTypeUID");

      var financialReportType = FinancialReportType.Parse(financialReportTypeUID);

      return FinancialReportDesignMapper.Map(financialReportType);
    }


    public FixedList<NamedEntityDto> FinancialReportTypesForDesign(string accountsChartUID) {
      Assertion.Require(accountsChartUID, "accountsChartUID");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      var list = FinancialReportType.GetListForDesign(accountsChart);

      return list.MapToNamedEntityList();
    }

    #endregion Use cases

  }  // class FinancialReportDesignUseCases

}  // namespace Empiria.FinancialAccounting.FinancialReports.UseCases
