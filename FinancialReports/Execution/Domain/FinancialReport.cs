/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReport                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the header and entries of a financial report.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Contains the header and entries of a financial report.</summary>
  public class FinancialReport {

    #region Constructors and parsers

    internal FinancialReport(FinancialReportCommand command,
                             FixedList<FinancialReportEntry> entries) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(entries, "entries");

      this.Command = command;
      this.Entries = entries;
    }


    internal FixedList<DataTableColumn> BreakdownDataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("itemCode", "Clave / Cuenta", "text"));
      columns.Add(new DataTableColumn("itemName", "Concepto / Nombre", "text"));
      columns.Add(new DataTableColumn("subledgerAccount", "Auxiliar", "text"));
      columns.Add(new DataTableColumn("sectorCode", "Sector", "text"));
      columns.Add(new DataTableColumn("operator", "Operador", "text"));
      columns.Add(new DataTableColumn("domesticCurrencyTotal", "M. Nal.", "decimal"));
      columns.Add(new DataTableColumn("foreignCurrencyTotal", "M. Ext.", "decimal"));
      columns.Add(new DataTableColumn("total", "Total", "decimal"));

      return columns.ToFixedList();
    }


    internal FixedList<DataTableColumn> DataColumns() {
      FinancialReportType reportType = this.Command.GetFinancialReportType();

      switch (reportType.DesignType) {
        case FinancialReportDesignType.FixedRows:
          return FixedRowsReportDataColumns();

        case FinancialReportDesignType.ConceptsIntegration:
          return FixedRowsReportConceptsIntegrationDataColumns();

        default:
          throw Assertion.AssertNoReachThisCode(
                $"Unhandled trial balance type {this.Command.FinancialReportType}.");
      }
    }


    private FixedList<DataTableColumn> FixedRowsReportDataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("conceptCode", "Clave CNBV", "text-link"));
      columns.Add(new DataTableColumn("concept", "Concepto", "text"));
      columns.Add(new DataTableColumn("domesticCurrencyTotal", "Moneda Nacional", "decimal"));
      columns.Add(new DataTableColumn("foreignCurrencyTotal", "Moneda Extranjera", "decimal"));
      columns.Add(new DataTableColumn("total", "Total", "decimal"));

      return columns.ToFixedList();
    }

    private FixedList<DataTableColumn> FixedRowsReportConceptsIntegrationDataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("conceptCode", "Clave CNBV", "text-link"));
      columns.Add(new DataTableColumn("concept", "Concepto", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text"));
      columns.Add(new DataTableColumn("subLedgerAccountNumber", "Auxiliar", "text"));
      columns.Add(new DataTableColumn("sectorCode", "Sector", "text"));
      columns.Add(new DataTableColumn("operator", "Operador", "text"));
      columns.Add(new DataTableColumn("domesticCurrencyTotal", "Moneda Nacional", "decimal"));
      columns.Add(new DataTableColumn("foreignCurrencyTotal", "Moneda Extranjera", "decimal"));
      columns.Add(new DataTableColumn("total", "Total", "decimal"));

      return columns.ToFixedList();
    }

    #endregion Constructors and parsers

    #region Properties

    public FinancialReportCommand Command {
      get;
    }


    public FixedList<FinancialReportEntry> Entries {
      get;
    }

    #endregion Properties

  } // class FinancialReport

} // namespace Empiria.FinancialAccounting.FinancialReports
