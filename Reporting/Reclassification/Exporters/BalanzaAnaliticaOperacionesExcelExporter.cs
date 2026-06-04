/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : BalanzaAnaliticaOperacionesExcelExporter     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with BalanzaAnaliticaOperaciones information.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.DynamicData;
using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Reclassification.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Reclassification {

  /// <summary>Creates a Microsoft Excel file with BalanzaAnaliticaOperaciones information.</summary>
  internal class BalanzaAnaliticaOperacionesExcelExporter {

    private TrialBalanceQuery _query = new TrialBalanceQuery();

    private readonly FileTemplateConfig _templateConfig;

    private ExcelFile _excelFile;

    public BalanzaAnaliticaOperacionesExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, nameof(templateConfig));

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(DynamicDto<BalanzaAnaliticaOperacionesDto> balanzaAnaliticaOperaciones) {
      Assertion.Require(balanzaAnaliticaOperaciones, nameof(balanzaAnaliticaOperaciones));

      _query = (TrialBalanceQuery) balanzaAnaliticaOperaciones.Query;

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader();

      SetTable(balanzaAnaliticaOperaciones.Entries);

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


    private void SetTable(FixedList<BalanzaAnaliticaOperacionesDto> entries) {

      int i = _templateConfig.FirstRowIndex;

      foreach (var entry in entries) {

        _excelFile.SetCell($"A{i}", entry.OperationType);
        _excelFile.SetCell($"B{i}", entry.AccountNo);
        _excelFile.SetCell($"C{i}", entry.AccountName);
        _excelFile.SetCell($"D{i}", entry.CurrencyCode);
        _excelFile.SetCell($"E{i}", entry.Debits);
        _excelFile.SetCell($"F{i}", entry.Credits);

        i++;
      }
    }

    #endregion Private methods

  }  // class BalanzaAnaliticaOperacionesExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Reclassification
