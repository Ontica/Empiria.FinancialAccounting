/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Mapper class                            *
*  Type     : IntegracionSaldosCapitalMapper             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map 'Integracion de saldos de capital' report entries.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters {

  /// <summary>Methods used to map 'Integracion de saldos de capital' report entries.</summary>
  static internal class IntegracionSaldosCapitalMapper {

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
        new DataTableColumn("prestamoName", "Préstamo", "text-nowrap"),
        new DataTableColumn("currencyCode", "Mon", "text"),
        new DataTableColumn("subledgerAccount", "Auxiliar", "text-nowrap"),
        new DataTableColumn("subledgerAccountName", "Nombre del auxiliar", "text"),
        new DataTableColumn("sectorCode", "Sector", "text"),
        new DataTableColumn("capitalCortoPlazoMonedaOrigen", "Saldo Corto Plazo", "decimal"),
        new DataTableColumn("capitalLargoPlazoMonedaOrigen", "Saldo Largo Plazo", "decimal"),
        new DataTableColumn("capitalMonedaOrigenTotal", "Capital MO", "decimal"),
        new DataTableColumn("vencimiento", "Vencimiento", "date"),
      };

      return columns.ToFixedList();
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries(List<IIntegracionSaldosCapitalInteresesEntry> entries) {

      var mappedItems = entries.Select((x) => MapToReportEntry(x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private IReportEntryDto MapToReportEntry(IIntegracionSaldosCapitalInteresesEntry entry) {
      if (entry is IntegracionSaldosCapitalInteresesEntry) {
        return MapToReportEntry((IntegracionSaldosCapitalInteresesEntry) entry);
      }
      if (entry is IntegracionSaldosCapitalInteresesTitle) {
        return MapToReportEntry((IntegracionSaldosCapitalInteresesTitle) entry);
      }
      throw new NotImplementedException();
    }

    static private IntegracionSaldosCapitalEntryDto MapToReportEntry(IntegracionSaldosCapitalInteresesEntry entry) {
      return new IntegracionSaldosCapitalEntryDto {
        ItemType = entry.ItemType,
        SubledgerAccount = entry.SubledgerAccount,
        SubledgerAccountName = entry.SubledgerAccountName,
        PrestamoName = entry.PrestamoBase.Name,
        CurrencyCode = entry.CurrencyCode,
        SectorCode = entry.SectorCode,
        CapitalCortoPlazoMonedaOrigen = entry.CapitalCortoPlazoMonedaOrigen,
        CapitalLargoPlazoMonedaOrigen = entry.CapitalLargoPlazoMonedaOrigen,
        CapitalMonedaOrigenTotal = entry.CapitalMonedaOrigen,
        Vencimiento = entry.Vencimiento
      };
    }


    static private IntegracionSaldosCapitalTitleDto MapToReportEntry(IntegracionSaldosCapitalInteresesTitle entry) {
      return new IntegracionSaldosCapitalTitleDto {
        ItemType = "Total",
        SubledgerAccount = entry.Title
      };
    }


    #endregion Private methods

  } // class IntegracionSaldosCapitalMapper

} // namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters
