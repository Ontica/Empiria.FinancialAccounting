/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Mapper class                            *
*  Type     : DerramaSwapsCoberturaMapper                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map 'Derrama de intereses swaps de cobertura'.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters {

  /// <summary>Methods used to map account comparer.</summary>
  static internal class DerramaSwapsCoberturaMapper {

    #region Public methods

    static internal ReportDataDto MapToReportDataDto(ReportBuilderQuery buildQuery,
                                                     List<DerramaSwapsCoberturaEntry> entries) {

      return new ReportDataDto {
        Query = buildQuery,
        Columns = GetColumns(),
        Entries = MapToReportDataEntries(entries)
      };

    }

    #endregion Public methods

    #region Private methods


    static private FixedList<DataTableColumn> GetColumns() {
      var columns = new List<DataTableColumn> {
        new DataTableColumn("subledgerAccount", "Auxiliar", "text-nowrap"),
        new DataTableColumn("subledgerAccountName", "Nombre del auxiliar", "text"),
        new DataTableColumn("incomeAccountTotal", "Saldo 5.01.04", "decimal"),
        new DataTableColumn("expensesAccountTotal", "Saldo 6.01.07", "decimal"),
        new DataTableColumn("total", "Consolidado", "decimal"),
        new DataTableColumn("classification", "Clasificación", "text")
      };

      return columns.ToFixedList();
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries(List<DerramaSwapsCoberturaEntry> entries) {

      var mappedItems = entries.Select((x) => MapToDerramaSwapsCoberturaDto(x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private DerramaSwapsCoberturaEntryDto MapToDerramaSwapsCoberturaDto(DerramaSwapsCoberturaEntry x) {
      return new DerramaSwapsCoberturaEntryDto {
        ItemType = x.ItemType,
        SubledgerAccount = x.SubledgerAccount,
        SubledgerAccountName = x.SubledgerAccountName,
        Classification = x.Classification,
        IncomeAccountTotal = x.IncomeAccountTotal,
        ExpensesAccountTotal = x.ExpensesAccountTotal
      };
    }

    #endregion Private methods

  } // class DerramaSwapsCoberturaMapper

} // namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters
