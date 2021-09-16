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


    internal FixedList<DataTableColumn> DataColumns() {
      switch (this.Command.FinancialReportType) {
        case FinancialReportType.R01:
          return R01DataColumns();

        default:
          throw Assertion.AssertNoReachThisCode(
                $"Unhandled trial balance type {this.Command.FinancialReportType}.");
      }
    }


    private FixedList<DataTableColumn> R01DataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("conceptCode", "Clave CNBV", "text-link"));
      columns.Add(new DataTableColumn("concept", "Concepto", "text"));
      columns.Add(new DataTableColumn("domesticCurrencyTotal", "1 Moneda Nacional", "decimal"));
      columns.Add(new DataTableColumn("foreignCurrencyTotal", "2 Moneda Extranjera", "decimal"));
      columns.Add(new DataTableColumn("total", "3 Total", "decimal"));

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
