/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Mapper class                            *
*  Type     : ActivoFijoDepreciacionMapper              License   : Please read LICENSE.txt file             *
*                                                                                                            *
*  Summary  : Methods used to map 'Activo fijo depreciacion' report.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.FixedAssetsDepreciation;

namespace Empiria.FinancialAccounting.Reporting.ActivoFijoDepreciacion.Adapters {

  /// <summary>Methods used to map 'Activo fijo depreciacion' report.</summary>
  static internal class ActivoFijoDepreciacionMapper {

    #region Public methods

    static internal ReportDataDto MapToReportDataDto(ReportBuilderQuery buildQuery,
                                                     FixedList<FixedAssetsDepreciationEntry> entries) {

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
        new DataTableColumn("numContabilidad", "Del", "text-nowrap"),
        new DataTableColumn("nombreContabilidad", "Delegación", "text-nowrap"),
        new DataTableColumn("auxiliarHistorico", "Auxiliar histórico", "text-nowrap"),
        new DataTableColumn("numeroInventario", "No Inventario", "text-nowrap"),
        new DataTableColumn("nombreAuxiliar", "Nombre auxiliar / Descripción", "text"),
        new DataTableColumn("fechaAdquisicion", "Adquisición", "date"),
        new DataTableColumn("fechaInicioDepreciacion", "Inicio dep.", "date"),
        new DataTableColumn("fechaTerminoDepreciacion", "Término dep.", "date"),
        new DataTableColumn("mesesDepreciacion", "Meses", "decimal", 0),
        new DataTableColumn("valorHistorico", "1.13.01", "decimal"),
        new DataTableColumn("depreciacionMensual", "Dep. mensual", "decimal"),
        new DataTableColumn("mesesTranscurridos", "Meses transc.", "decimal", 0),
        new DataTableColumn("depreciacionAcumulada", "Dep. acumulada", "decimal"),
        new DataTableColumn("depreciacionAcumuladaRegistradaContablemente", "3.06.01", "decimal"),
        new DataTableColumn("depreciacionPendienteRegistrar", "Dep. pend. registrar", "decimal"),
        new DataTableColumn("auxiliarRevaluacion", "Auxiliar revaluación", "text"),
        new DataTableColumn("montoRevaluacion", "Revaluación", "decimal"),
        new DataTableColumn("depreciacionDeLaRevaluacionMensual", "Dep. reval. mensual", "decimal"),
        new DataTableColumn("depreciacionAcumuladaDeLaRevaluacion", "Dep. acumulada reval.", "decimal"),
        new DataTableColumn("depreciacionPendienteRegistrarDeLaRevaluacion", "Dep. pend. registrar reval.", "decimal"),
        new DataTableColumn("valorHistoricoEnLibros", "Valor en libros hist.", "decimal"),
      };

      return columns.ToFixedList();
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries(FixedList<FixedAssetsDepreciationEntry> entries) {

      var mappedItems = entries.Select((x) => ActivoFijoDepreciacionEntryDto(x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private ActivoFijoDepreciacionEntryDto ActivoFijoDepreciacionEntryDto(FixedAssetsDepreciationEntry x) {
      return new ActivoFijoDepreciacionEntryDto {
        ItemType = "Entry",
        NumContabilidad = x.Ledger.Number,
        NombreContabilidad = x.Ledger.Name,
        AuxiliarHistorico = x.AuxiliarHistorico.Number,
        NumeroInventario = x.NumeroInventario,
        NombreAuxiliar = x.AuxiliarHistorico.Name,
        FechaAdquisicion = x.FechaAdquisicion,
        FechaInicioDepreciacion = x.FechaInicioDepreciacion,
        FechaTerminoDepreciacion = x.FechaTerminoDepreciacion,
        MesesDepreciacion = x.MesesDepreciacion,
        ValorHistorico = x.ValorHistorico,
        DepreciacionMensual = x.DepreciacionMensual,
        MesesTranscurridos = x.MesesTranscurridos,
        DepreciacionAcumulada = x.DepreciacionAcumulada,
        DepreciacionAcumuladaRegistradaContablemente = x.DepreciacionAcumuladaRegistradaContablemente,
        DepreciacionPendienteRegistrar = x.DepreciacionPendienteRegistrar,
        AuxiliarRevaluacion = x.AuxiliarRevaluacion.IsEmptyInstance ? string.Empty : x.AuxiliarRevaluacion.Number,
        MontoRevaluacion = x.MontoRevaluacion,
        DepreciacionDeLaRevaluacionMensual = x.DepreciacionDeLaRevaluacionMensual,
        DepreciacionAcumuladaDeLaRevaluacion = x.DepreciacionAcumuladaDeLaRevaluacion,
        DepreciacionAcumuladaDeLaRevaluacionRegistradaContablemente = x.DepreciacionAcumuladaDeLaRevaluacionRegistradaContablemente,
        DepreciacionPendienteRegistrarDeLaRevaluacion = x.DepreciacionPendienteRegistrarDeLaRevaluacion,
        ValorHistoricoEnLibros = x.ValorHistoricoEnLibros
      };
    }

    #endregion Private methods

  } // class ActivoFijoDepreciacionMapper

} // namespace Empiria.FinancialAccounting.Reporting.ActivoFijoDepreciacion.Adapters
