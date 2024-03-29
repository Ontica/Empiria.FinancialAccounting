﻿/* Empiria Financial *****************************************************************************************
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

      list = base.RestrictUserDataAccessTo(list);

      return list.MapToNamedEntityList();
    }


    public FinancialReportCellDto InsertCell(EditFinancialReportCommand command) {
      InitializeCommand(command);

      FinancialReportType reportType = command.Entities.FinancialReportType;

      ReportCellFields cellFields = command.MapToReportCellFields();

      FinancialReportCell cell = reportType.InsertCell(cellFields);

      cell.Save();

      return FinancialReportDesignMapper.Map(cell);
    }


    public FinancialReportRowDto InsertRow(EditFinancialReportCommand command) {
      InitializeCommand(command);

      FinancialReportType reportType = command.Entities.FinancialReportType;

      ReportRowFields rowFields = command.MapToReportRowFields();

      FinancialReportRow row = reportType.InsertRow(rowFields);

      row.Save();

      return FinancialReportDesignMapper.Map(row);
    }


    public void RemoveCell(string reportTypeUID, string cellUID) {
      Assertion.Require(reportTypeUID, nameof(reportTypeUID));
      Assertion.Require(cellUID, nameof(cellUID));

      var reportType = FinancialReportType.Parse(reportTypeUID);

      FinancialReportCell cell = reportType.GetCell(cellUID);

      reportType.RemoveCell(cell);

      cell.Save();
    }


    public void RemoveRow(string reportTypeUID, string rowUID) {
      Assertion.Require(reportTypeUID, nameof(reportTypeUID));
      Assertion.Require(rowUID, nameof(rowUID));

      var reportType = FinancialReportType.Parse(reportTypeUID);

      FinancialReportRow row = reportType.GetRow(rowUID);

      reportType.RemoveRow(row);

      row.Save();
    }


    public FinancialReportCellDto UpdateCell(EditFinancialReportCommand command) {
      InitializeCommand(command);

      FinancialReportType reportType = command.Entities.FinancialReportType;

      var cell = (FinancialReportCell) command.Entities.FinancialReportItem;

      ReportCellFields cellFields = command.MapToReportCellFields();

      reportType.UpdateCell(cell, cellFields);

      cell.Save();

      return FinancialReportDesignMapper.Map(cell);
    }


    public FinancialReportRowDto UpdateRow(EditFinancialReportCommand command) {
      InitializeCommand(command);

      FinancialReportType reportType = command.Entities.FinancialReportType;

      var row = (FinancialReportRow) command.Entities.FinancialReportItem;

      ReportRowFields rowFields = command.MapToReportRowFields();

      reportType.UpdateRow(row, rowFields);

      row.Save();

      return FinancialReportDesignMapper.Map(row);
    }

    #endregion Use cases

    #region Helpers

    private void InitializeCommand(EditFinancialReportCommand command) {
      Assertion.Require(command, nameof(command));

      command.Arrange();

      command.EnsureIsValid();
    }

    #endregion Helpers

  }  // class FinancialReportDesignUseCases

}  // namespace Empiria.FinancialAccounting.FinancialReports.UseCases
