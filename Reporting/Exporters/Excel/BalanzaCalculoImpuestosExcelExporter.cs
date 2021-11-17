/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : BalanzaCalculoImpuestosExcelExporter         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Genera los datos de la balanza para calcular impuestos en un archivo Excel.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Reporting.Builders;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Genera los datos de la balanza para calcular impuestos en un archivo Excel.</summary>
  internal class BalanzaCalculoImpuestosExcelExporter : IExcelExporter {
    private readonly ReportDataDto _reportData;
    private readonly ExcelTemplateConfig _template;

    public BalanzaCalculoImpuestosExcelExporter(ReportDataDto reportData, ExcelTemplateConfig template) {
      Assertion.AssertObject(reportData, "reportData");
      Assertion.AssertObject(template, "template");

      _reportData = reportData;
      _template = template;
    }


    public FileReportDto CreateExcelFile() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      FillOutRows(excelFile, _reportData.Entries.Select(x => (BalanzaCalculoImpuestosEntry) x));

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileReportDto();
    }


    #region Private methods

    private void FillOutRows(ExcelFile excelFile, IEnumerable<BalanzaCalculoImpuestosEntry> entries) {
      int i = 2;

      foreach (var entry in entries) {

        string cuentaSinSector = entry.Cuenta.Contains("-") ? entry.Cuenta.Replace("-", "") : entry.Cuenta;
        for (int _i = cuentaSinSector.Length; _i <= 18; _i++) {
          cuentaSinSector += "0";
        }
        string cuentaSector = $"{cuentaSinSector.Substring(0, 16)}{entry.Sector}";


        excelFile.SetCell($"A{i}", entry.Moneda);
        excelFile.SetCell($"B{i}", entry.Cuenta);
        excelFile.SetCell($"C{i}", entry.Sector);
        excelFile.SetCell($"D{i}", entry.Descripcion);
        excelFile.SetCell($"E{i}", entry.SaldoInicial);
        excelFile.SetCell($"F{i}", entry.Debe);
        excelFile.SetCell($"G{i}", entry.Haber);
        excelFile.SetCell($"H{i}", entry.SaldoFinal);

        excelFile.SetCell($"I{i}", entry.Movimiento);
        excelFile.SetCell($"J{i}", entry.Contabilidad);
        excelFile.SetCell($"K{i}", entry.FechaConsulta);

        excelFile.SetCell($"L{i}", entry.Cuenta.Substring(0, 1));
        excelFile.SetCell($"M{i}", entry.Cuenta.Substring(0, 2));
        excelFile.SetCell($"N{i}", entry.Cuenta.Substring(2, 2));
        excelFile.SetCell($"O{i}", entry.Cuenta.Substring(0, 4));
        excelFile.SetCell($"P{i}", cuentaSector.Substring(4, 2));
        excelFile.SetCell($"Q{i}", cuentaSector.Substring(6, 2));
        excelFile.SetCell($"R{i}", cuentaSector.Substring(8, 2));
        excelFile.SetCell($"S{i}", cuentaSector.Substring(10, 2));
        excelFile.SetCell($"T{i}", cuentaSector.Substring(12, 2));
        excelFile.SetCell($"U{i}", cuentaSector.Substring(14, 2));
        excelFile.SetCell($"V{i}", cuentaSector.Substring(16, 2));
        excelFile.SetCell($"W{i}", cuentaSector);
        excelFile.SetCell($"X{i}", cuentaSinSector);

        excelFile.SetCell($"Y{i}", entry.Moneda);
        excelFile.SetCell($"Z{i}", entry.Cuenta);
        excelFile.SetCell($"AA{i}", entry.Sector);
        excelFile.SetCell($"AB{i}", entry.Descripcion);
        excelFile.SetCell($"AC{i}", entry.SaldoInicial);
        excelFile.SetCell($"AD{i}", entry.Debe);
        excelFile.SetCell($"AE{i}", entry.Haber);
        excelFile.SetCell($"AF{i}", entry.SaldoFinal);
        excelFile.SetCell($"AG{i}", entry.Movimiento);

        excelFile.SetCell($"AH{i}", entry.VBxcoEquivalencia);
        excelFile.SetCell($"AI{i}", entry.VBxcoSaldoInicial);
        excelFile.SetCell($"AJ{i}", entry.VBxcoDebe);
        excelFile.SetCell($"AK{i}", entry.VBxcoHaber);
        excelFile.SetCell($"AL{i}", entry.VBxcoSaldoFinal);

        i++;
      }
    }

    #endregion Private methods

  } // class BalanzaSatExcelExporter

} // Empiria.FinancialAccounting.Reporting.Exporters.Excel
