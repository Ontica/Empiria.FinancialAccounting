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
      PrepareCommand(command);

      FinancialReportType reportType = command.Entities.FinancialReportType;

      ReportCellFields cellFields = command.MapToReportCellFields();

      FinancialReportCell cell = reportType.InsertCell(cellFields);

      cell.Save();

      return FinancialReportDesignMapper.Map(cell);
    }


    public FinancialReportRowDto InsertRow(EditFinancialReportCommand command) {
      PrepareCommand(command);

      FinancialReportType reportType = command.Entities.FinancialReportType;

      ReportRowFields rowFields = command.MapToReportRowFields();

      FinancialReportRow row = reportType.InsertRow(rowFields,
                                                    command.Payload.Positioning);

      row.Save();

      return FinancialReportDesignMapper.Map(row);
    }


    public void RemoveCell(string reportTypeUID, string cellUID) {
      Assertion.Require(reportTypeUID, nameof(reportTypeUID));
      Assertion.Require(cellUID, nameof(cellUID));

      var reportType = FinancialReportType.Parse(reportTypeUID);

      var cell = FinancialReportCell.Parse(cellUID);

      reportType.RemoveCell(cell);

      cell.Save();
    }


    public void RemoveRow(string reportTypeUID, string rowUID) {
      Assertion.Require(reportTypeUID, nameof(reportTypeUID));
      Assertion.Require(rowUID, nameof(rowUID));

      var reportType = FinancialReportType.Parse(reportTypeUID);

      var row = FinancialReportRow.Parse(rowUID);

      reportType.RemoveRow(row);

      row.Save();
    }


    public FinancialReportCellDto UpdateCell(EditFinancialReportCommand command) {
      PrepareCommand(command);

      FinancialReportType reportType = command.Entities.FinancialReportType;

      ReportCellFields cellFields = command.MapToReportCellFields();

      FinancialReportCell cell = reportType.UpdateCell(cellFields);

      cell.Save();

      return FinancialReportDesignMapper.Map(cell);
    }


    public FinancialReportRowDto UpdateRow(EditFinancialReportCommand command) {
      PrepareCommand(command);

      FinancialReportType reportType = command.Entities.FinancialReportType;

      ReportRowFields rowFields = command.MapToReportRowFields();

      FinancialReportRow row = reportType.UpdateRow(rowFields,
                                                    command.Payload.Positioning);

      row.Save();

      return FinancialReportDesignMapper.Map(row);
    }

    #endregion Use cases

    #region Helpers

    private void PrepareCommand(EditFinancialReportCommand command) {
      Assertion.Require(command, nameof(command));

      command.Arrange();

      command.EnsureIsValid();
    }

    #endregion Helpers

  }  // class FinancialReportDesignUseCases

}  // namespace Empiria.FinancialAccounting.FinancialReports.UseCases
