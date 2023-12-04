/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : ActivoFijoDepreciacionExcelExporter          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Service used to export 'Activo Fijo Depreciacion' to Microsoft Excel.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Office;
using Empiria.Storage;

using System.Collections.Generic;

using Empiria.FinancialAccounting.Reporting.ActivoFijoDepreciacion.Adapters;
using System.Globalization;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Service used to export 'Activo Fijo Depreciacion' to Microsoft Excel.</summary>
  internal class ActivoFijoDepreciacionExcelExporter : IExcelExporter {

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public ActivoFijoDepreciacionExcelExporter(ReportDataDto reportData, FileTemplateConfig template) {
      Assertion.Require(reportData, nameof(reportData));
      Assertion.Require(template, nameof(template));

      _reportData = reportData;
      _template = template;
    }


    public FileReportDto CreateExcelFile() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile, _reportData.Entries.Select(x => (ActivoFijoDepreciacionEntryDto) x));

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileReportDto();
    }

    #region Private methods

    private void FillOutRows(ExcelFile excelFile, IEnumerable<ActivoFijoDepreciacionEntryDto> entries) {
      int i = _template.FirstRowIndex;

      int consecutivo = 1;
      foreach (var entry in entries) {

        excelFile.SetCell($"A{i}", $"{consecutivo}");
        excelFile.SetCell($"B{i}", entry.NumContabilidad);
        excelFile.SetCell($"C{i}", entry.NombreContabilidad);
        excelFile.SetCell($"D{i}", entry.AuxiliarHistorico);
        excelFile.SetCell($"E{i}", entry.NumeroInventario);
        excelFile.SetCell($"F{i}", entry.TipoActivoFijoName);
        excelFile.SetCell($"G{i}", entry.NombreAuxiliar);
        if (entry.FechaAdquisicion != ExecutionServer.DateMinValue) {
          excelFile.SetCell($"H{i}", entry.FechaAdquisicion);
        }
        if (entry.FechaInicioDepreciacion != ExecutionServer.DateMinValue) {
          excelFile.SetCell($"I{i}", entry.FechaInicioDepreciacion);
        }
        if (entry.FechaTerminoDepreciacion != ExecutionServer.DateMinValue) {
          excelFile.SetCell($"J{i}", entry.FechaTerminoDepreciacion);
        }
        if (entry.MesesDepreciacion != 0) {
          excelFile.SetCell($"K{i}", entry.MesesDepreciacion);
        }
        excelFile.SetCell($"L{i}", entry.ValorHistorico);
        excelFile.SetCell($"M{i}", entry.DepreciacionMensual);
        excelFile.SetCell($"N{i}", entry.MesesTranscurridos);
        excelFile.SetCell($"O{i}", entry.DepreciacionAcumulada);
        excelFile.SetCell($"P{i}", entry.DepreciacionAcumuladaRegistradaContablemente);
        excelFile.SetCell($"Q{i}", entry.DepreciacionPendienteRegistrar);
        excelFile.SetCell($"R{i}", entry.AuxiliarRevaluacion);
        excelFile.SetCell($"S{i}", entry.MontoRevaluacion);
        excelFile.SetCell($"T{i}", entry.DepreciacionDeLaRevaluacionMensual);
        excelFile.SetCell($"U{i}", entry.DepreciacionAcumuladaDeLaRevaluacion);
        excelFile.SetCell($"V{i}", entry.DepreciacionAcumuladaDeLaRevaluacionRegistradaContablemente);
        excelFile.SetCell($"W{i}", entry.DepreciacionPendienteRegistrarDeLaRevaluacion);
        excelFile.SetCell($"X{i}", entry.ValorHistoricoEnLibros);

        i++;
        consecutivo ++;
      }
    }


    private void SetHeader(ExcelFile excelFile) {
      CultureInfo esUS = new CultureInfo("es-US");

      var date = $"Al {_reportData.Query.ToDate.ToString("dd \\de MMMM \\de yyyy", esUS)}".ToUpper();

      excelFile.SetCell(_template.ReportDateCell, date);
    }

    #endregion Private methods

  }  // class ActivoFijoDepreciacionExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting
