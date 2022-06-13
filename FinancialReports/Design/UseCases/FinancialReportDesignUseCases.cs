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


    public FinancialReportDesignDto FinancialReportDesign(string reportTypeUID) {
      Assertion.Require(reportTypeUID, nameof(reportTypeUID));

      var reportType = FinancialReportType.Parse(reportTypeUID);

      return FinancialReportDesignMapper.Map(reportType);
    }


    public FixedList<NamedEntityDto> FinancialReportTypesForDesign(string chartUID) {
      Assertion.Require(chartUID, nameof(chartUID));

      var accountsChart = AccountsChart.Parse(chartUID);

      var list = FinancialReportType.GetListForDesign(accountsChart);

      return list.MapToNamedEntityList();
    }


    public FinancialReportCellDto InsertCell(EditFinancialReportCommand command) {
      throw new NotImplementedException();
    }


    public FinancialReportRowDto InsertRow(EditFinancialReportCommand command) {
      Assertion.Require(command, nameof(command));

      command.Arrange();

      command.EnsureIsValid();

      FinancialReportType reportType = command.Entities.FinancialReportType;

      FinancialReportRow row = reportType.InsertRow(command.MapToRowEditionField(),
                                                    command.Payload.Positioning);

      return FinancialReportDesignMapper.Map(row);
    }


    public void RemoveCell(string reportTypeUID, string cellUID) {
      Assertion.Require(reportTypeUID, nameof(reportTypeUID));
      Assertion.Require(cellUID, nameof(cellUID));

      throw new NotImplementedException();
    }


    public void RemoveRow(string reportTypeUID, string rowUID) {
      Assertion.Require(reportTypeUID, nameof(reportTypeUID));
      Assertion.Require(rowUID, nameof(rowUID));

      throw new NotImplementedException();
    }


    public FinancialReportCellDto UpdateCell(EditFinancialReportCommand command) {
      throw new NotImplementedException();
    }


    public FinancialReportRowDto UpdateRow(EditFinancialReportCommand command) {
      Assertion.Require(command, nameof(command));

      command.Arrange();

      command.EnsureIsValid();

      throw new NotImplementedException();
    }


    #endregion Use cases

  }  // class FinancialReportDesignUseCases

}  // namespace Empiria.FinancialAccounting.FinancialReports.UseCases
