/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : BalanzaValorizadaExcelExporter               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with BalanzaValorizada information.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.DynamicData;
using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Balances {

  /// <summary>Creates a Microsoft Excel file with BalanzaValorizada information.</summary>
  internal class BalanzaValorizadaExcelExporter {

    private TrialBalanceQuery _query = new TrialBalanceQuery();

    private readonly FileTemplateConfig _templateConfig;

    private ExcelFile _excelFile;

    public BalanzaValorizadaExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, nameof(templateConfig));

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(DynamicDto<BalanzaValorizadaEntry> balanzaValorizada) {
      Assertion.Require(balanzaValorizada, nameof(balanzaValorizada));

      _query = (TrialBalanceQuery) balanzaValorizada.Query;

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader();

      SetTable(balanzaValorizada.Entries);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }

    #region Private methods

    private void SetHeader() {
      _excelFile.SetCell($"A2", _templateConfig.Title);

      var subTitle = $"Del {_query.InitialPeriod.FromDate.ToString("dd/MMM/yyyy")} " +
                     $"al {_query.InitialPeriod.ToDate.ToString("dd/MMM/yyyy")}";

      _excelFile.SetCell($"A3", subTitle);
    }


    private void SetTable(FixedList<BalanzaValorizadaEntry> entries) {

      int i = _templateConfig.FirstRowIndex;

      foreach (var entry in entries) {

        _excelFile.SetCell($"A{i}", entry.NumeroCuenta);
        _excelFile.SetCell($"B{i}", entry.NombreCuenta);
        _excelFile.SetCell($"C{i}", entry.CodigoMoneda);
        _excelFile.SetCell($"D{i}", entry.FechaAfectacion.ToString("dd/MMM/yyyy"));
        _excelFile.SetCell($"E{i}", entry.SaldoInicial);
        _excelFile.SetCell($"F{i}", entry.Cargos);
        _excelFile.SetCell($"G{i}", entry.Abonos);
        _excelFile.SetCell($"H{i}", entry.SaldoFinal);
        _excelFile.SetCell($"I{i}", entry.TipoCambio);
        _excelFile.SetCell($"J{i}", entry.SaldoInicialMXN);
        _excelFile.SetCell($"K{i}", entry.CargosMXN);
        _excelFile.SetCell($"L{i}", entry.AbonosMXN);
        _excelFile.SetCell($"M{i}", entry.SaldoFinalMXN);
        _excelFile.SetCell($"N{i}", entry.UtilidadCambiaria);

        i++;
      }

    }

    #endregion Private methods

  }  // class BalanzaValorizadaExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Balances
