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

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Genera los datos de la balanza para calcular impuestos en un archivo Excel.</summary>
  internal class BalanzaCalculoImpuestosExcelExporter : IExcelExporter {
    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public BalanzaCalculoImpuestosExcelExporter(ReportDataDto reportData, FileTemplateConfig template) {
      Assertion.Require(reportData, "reportData");
      Assertion.Require(template, "template");

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
      bool accountsChartflag = false;

      foreach (var entry in entries) {

        if (entry.Contabilidad == "Contabilidad bancaria") {
          FillAccountChartRows(entry, excelFile, i);
          accountsChartflag = true;

        } else if (entry.Contabilidad == "Contabilidad bancaria 2022") {
          FillAccountChart2022Rows(entry, excelFile, i);
        }
        i++;
      }
      if (accountsChartflag) {
        excelFile.RemoveColumn("Z");
        excelFile.RemoveColumn("Y");
        excelFile.RemoveColumn("X");
        excelFile.RemoveColumn("W");
      }
    }

    private void FillAccountChartRows(BalanzaCalculoImpuestosEntry entry, ExcelFile excelFile, int i) {

      string cuentaSinSector = entry.Cuenta.Replace("-", string.Empty);
      cuentaSinSector = cuentaSinSector.PadRight(18, '0');

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

      excelFile.SetCell($"L{i}", cuentaSinSector.Substring(0, 1));
      excelFile.SetCell($"M{i}", cuentaSinSector.Substring(0, 2));
      excelFile.SetCell($"N{i}", cuentaSinSector.Substring(2, 2));
      excelFile.SetCell($"O{i}", cuentaSinSector.Substring(0, 4));
      excelFile.SetCell($"P{i}", cuentaSector.Substring(4, 2));
      excelFile.SetCell($"Q{i}", cuentaSector.Substring(6, 2));
      excelFile.SetCell($"R{i}", cuentaSector.Substring(8, 2));
      excelFile.SetCell($"S{i}", cuentaSector.Substring(10, 2));
      excelFile.SetCell($"T{i}", cuentaSector.Substring(12, 2));
      excelFile.SetCell($"U{i}", cuentaSector.Substring(14, 2));
      excelFile.SetCell($"V{i}", cuentaSector.Substring(16, 2));
      excelFile.SetCell($"AA{i}", cuentaSector);
      excelFile.SetCell($"AB{i}", cuentaSinSector);

      excelFile.SetCell($"AC{i}", entry.Moneda);
      excelFile.SetCell($"AD{i}", entry.Cuenta);
      excelFile.SetCell($"AE{i}", entry.Sector);
      excelFile.SetCell($"AF{i}", entry.Descripcion);
      excelFile.SetCell($"AG{i}", entry.SaldoInicial);
      excelFile.SetCell($"AH{i}", entry.Debe);
      excelFile.SetCell($"AI{i}", entry.Haber);
      excelFile.SetCell($"AJ{i}", entry.SaldoFinal);
      excelFile.SetCell($"AK{i}", entry.Movimiento);

      excelFile.SetCell($"AL{i}", entry.VBxcoEquivalencia);
      excelFile.SetCell($"AM{i}", entry.VBxcoSaldoInicial);
      excelFile.SetCell($"AN{i}", entry.VBxcoDebe);
      excelFile.SetCell($"AO{i}", entry.VBxcoHaber);
      excelFile.SetCell($"AP{i}", entry.VBxcoSaldoFinal);
    }


    private void FillAccountChart2022Rows(BalanzaCalculoImpuestosEntry entry, ExcelFile excelFile, int i) {
      string cuentaSinSector = (entry.Cuenta.Replace(".", string.Empty)).PadRight(23, '0');

      string cuentaSector = $"{cuentaSinSector.Substring(0, 21)}{entry.Sector}";

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

      // TODO
      excelFile.SetCell($"L{i}", cuentaSinSector.Substring(0, 1));
      excelFile.SetCell($"M{i}", cuentaSinSector.Substring(0, 1));
      excelFile.SetCell($"N{i}", cuentaSinSector.Substring(0, 1));
      excelFile.SetCell($"O{i}", cuentaSinSector.Substring(0, 1));
      excelFile.SetCell($"P{i}", cuentaSector.Substring(1, 2));
      excelFile.SetCell($"Q{i}", cuentaSector.Substring(3, 2));
      excelFile.SetCell($"R{i}", cuentaSector.Substring(5, 2));
      excelFile.SetCell($"S{i}", cuentaSector.Substring(7, 2));
      excelFile.SetCell($"T{i}", cuentaSector.Substring(9, 2));
      excelFile.SetCell($"U{i}", cuentaSector.Substring(11, 2));
      excelFile.SetCell($"V{i}", cuentaSector.Substring(13, 2));
      excelFile.SetCell($"W{i}", cuentaSector.Substring(15, 2));
      excelFile.SetCell($"X{i}", cuentaSector.Substring(17, 2));
      excelFile.SetCell($"Y{i}", cuentaSector.Substring(19, 2));
      excelFile.SetCell($"Z{i}", cuentaSector.Substring(21, 2));
      excelFile.SetCell($"AA{i}", cuentaSector);
      excelFile.SetCell($"AB{i}", cuentaSinSector);

      excelFile.SetCell($"AC{i}", entry.Moneda);
      excelFile.SetCell($"AD{i}", entry.Cuenta);
      excelFile.SetCell($"AE{i}", entry.Sector);
      excelFile.SetCell($"AF{i}", entry.Descripcion);
      excelFile.SetCell($"AG{i}", entry.SaldoInicial);
      excelFile.SetCell($"AH{i}", entry.Debe);
      excelFile.SetCell($"AI{i}", entry.Haber);
      excelFile.SetCell($"AJ{i}", entry.SaldoFinal);
      excelFile.SetCell($"AK{i}", entry.Movimiento);

      excelFile.SetCell($"AL{i}", entry.VBxcoEquivalencia);
      excelFile.SetCell($"AM{i}", entry.VBxcoSaldoInicial);
      excelFile.SetCell($"AN{i}", entry.VBxcoDebe);
      excelFile.SetCell($"AO{i}", entry.VBxcoHaber);
      excelFile.SetCell($"AP{i}", entry.VBxcoSaldoFinal);

      i++;
    }

    #endregion Private methods

  } // class BalanzaSatExcelExporter

} // Empiria.FinancialAccounting.Reporting
