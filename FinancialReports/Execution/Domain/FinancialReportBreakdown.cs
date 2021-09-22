/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : FinancialReportBreakdown                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that holds a financial report item breakdown.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Data structure that holds a financial report item breakdown.</summary>
  internal class FinancialReportBreakdown {


    public FinancialReportBreakdown(FinancialReportCommand command,
                                    FixedList<FinancialReportBreakdownEntry> entries) {
      this.Command = command;
      this.Entries = entries;
    }

    #region Properties

    internal FixedList<DataTableColumn> DataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("itemCode", "Clave / Cuenta", "text"));
      columns.Add(new DataTableColumn("itemName", "Concepto / Nombre de la cuenta", "text"));
      columns.Add(new DataTableColumn("subledgerAccount", "Auxiliar", "text"));
      columns.Add(new DataTableColumn("sectorCode", "Sector", "text"));
      columns.Add(new DataTableColumn("operator", "Operador", "text"));
      columns.Add(new DataTableColumn("domesticCurrencyTotal", "1 Moneda Nacional", "decimal"));
      columns.Add(new DataTableColumn("foreignCurrencyTotal", "2 Moneda Extranjera", "decimal"));
      columns.Add(new DataTableColumn("total", "3 Total", "decimal"));

      return columns.ToFixedList();
    }

    public FinancialReportCommand Command {
      get;
    }


    public FixedList<FinancialReportBreakdownEntry> Entries {
      get;
    }

    #endregion Properties

  }  // class FinancialReportBreakdown

}  // namespace Empiria.FinancialAccounting.FinancialReports
