/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Mapper class                            *
*  Type     : DerramaSwapsCoberturaConsolidadoMapper     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map 'Derrama de intereses swaps de cobertura' report.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters {

  /// <summary>Methods used to map 'Derrama de intereses swaps de cobertura' report.</summary>
  static internal class DerramaSwapsCoberturaConsolidadoMapper {

    #region Public methods

    static internal ReportDataDto MapToReportDataDto(ReportBuilderQuery buildQuery,
                                                     FixedList<DerramaSwapsCoberturaConsolidadoEntryDto> entries) {

      return new ReportDataDto {
        Query = buildQuery,
        Columns = GetColumns(),
        Entries = new FixedList<IReportEntryDto>(entries)
      };

    }

    #endregion Public methods

    #region Private methods


    static private FixedList<DataTableColumn> GetColumns() {
      var columns = new List<DataTableColumn> {
        new DataTableColumn("classification", "Concepto", "text-nowrap"),
        new DataTableColumn("incomeTotal", "Ingreso 5.01.04", "decimal"),
        new DataTableColumn("expensesTotal", "Gasto 6.01.07", "decimal"),
        new DataTableColumn("total", "Efecto neto", "decimal"),

      };

      return columns.ToFixedList();
    }

    #endregion Private methods

  } // class DerramaSwapsCoberturaConsolidadoMapper

} // namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters
