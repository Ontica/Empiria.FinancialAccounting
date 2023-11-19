/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                                   Component : Interface adapters            *
*  Assembly : FinancialAccounting.Reporting.dll                    Pattern   : Mapper class                  *
*  Type     : IntegracionSaldosCapitalInteresesConsolidadoMapper   License   : Please read LICENSE.txt file  *
*                                                                                                            *
*  Summary  : Methods used to map 'Integracion de saldos de capital e intereses' report entries.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters {

  /// <summary>Methods used to map 'Integracion de saldos de capital e intereses' report entries.</summary>
  static internal class IntegracionSaldosCapitalInteresesConsolidadoMapper {

    #region Public methods

    static internal ReportDataDto MapToReportDataDto(ReportBuilderQuery buildQuery,
                                                     List<IIntegracionSaldosCapitalInteresesEntry> entries) {

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
        new DataTableColumn("title", "Banco", "text-nowrap"),
        new DataTableColumn("prestamoID", "Préstamo", "text-nowrap"),
        new DataTableColumn("currencyName", "Moneda", "text"),
        new DataTableColumn("capitalMonedaOrigenTotal", "Capital MO", "decimal"),
        new DataTableColumn("interesesMonedaOrigenTotal", "Intereses MO", "decimal"),
        new DataTableColumn("totalMonedaOrigen", "Total MO", "decimal"),
        new DataTableColumn("tipoCambio", "T.C", "decimal", 6),
        new DataTableColumn("capitalMonedaNacional", "Capital MN", "decimal"),
        new DataTableColumn("interesesMonedaNacional", "Intereses MN", "decimal"),
        new DataTableColumn("totalMonedaNacional", "Total MN", "decimal"),
      };

      return columns.ToFixedList();
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries(List<IIntegracionSaldosCapitalInteresesEntry> entries) {

      var mappedItems = entries.Select((x) => MapToReportEntry(x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private IReportEntryDto MapToReportEntry(IIntegracionSaldosCapitalInteresesEntry entry) {
      if (entry is IntegracionSaldosCapitalInteresesEntry e) {
        return MapToReportEntry(e);
      }
      if (entry is IntegracionSaldosCapitalInteresesSubTotal subtotal) {
        return MapToReportEntry(subtotal);
      }
      if (entry is IntegracionSaldosCapitalInteresesTitle title) {
        return MapToReportEntry(title);
      }
      throw new NotImplementedException();
    }

    static private IntegracionSaldosCapitalInteresesConsolidadoEntryDto MapToReportEntry(IntegracionSaldosCapitalInteresesEntry entry) {
      return new IntegracionSaldosCapitalInteresesConsolidadoEntryDto {
        ItemType = entry.PrestamoBase.Order > 90 ? "Total" : "Entry",
        Title = entry.PrestamoBase.Bank,
        Banco = entry.PrestamoBase.Bank,
        PrestamoID = entry.PrestamoBase.Number,
        PrestamoName = entry.PrestamoBase.Name,
        CurrencyName = Currency.Parse(entry.CurrencyCode).ShortName,
        CapitalCortoPlazoMonedaOrigen = entry.CapitalCortoPlazoMonedaOrigen,
        CapitalLargoPlazoMonedaOrigen = entry.CapitalLargoPlazoMonedaOrigen,
        CapitalMonedaOrigenTotal = entry.CapitalMonedaOrigen,
        InteresesMonedaOrigenTotal = entry.InteresesMonedaOrigen,
        TotalMonedaOrigen = entry.TotalMonedaOrigen,
        TipoCambio = entry.TipoCambio,
        CapitalMonedaNacional = entry.CapitalMonedaNacional,
        InteresesMonedaNacional = entry.InteresesMonedaNacional,
        TotalMonedaNacional = entry.TotalMonedaNacional
      };
    }


    static private IntegracionSaldosCapitalInteresesConsolidadoTotalDto MapToReportEntry(IntegracionSaldosCapitalInteresesSubTotal entry) {
      return new IntegracionSaldosCapitalInteresesConsolidadoTotalDto {
        ItemType = "Total",
        Title = entry.Title,
        CapitalMonedaNacional = entry.CapitalMonedaNacional,
        InteresesMonedaNacional = entry.InteresesMonedaNacional,
        TotalMonedaNacional = entry.TotalMonedaNacional
      };
    }

    static private IntegracionSaldosCapitalInteresesConsolidadoTitleDto MapToReportEntry(IntegracionSaldosCapitalInteresesTitle entry) {
      return new IntegracionSaldosCapitalInteresesConsolidadoTitleDto {
        ItemType = "Total",
        Title = entry.Title
      };
    }

    #endregion Private methods

  } // class IntegracionSaldosCapitalInteresesConsolidadoMapper

} // namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters
