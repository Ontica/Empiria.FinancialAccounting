/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : BalanzaTradicionalReclasificadaExcelExporter License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with BalanzaTradicionalReclasificadaExcelExporter information.  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.DynamicData;
using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Reclassification.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Reclassification {

  /// <summary>Creates a Microsoft Excel file with BalanzaTradicionalReclasificadaExcelExporter information.</summary>
  internal class BalanzaTradicionalReclasificadaExcelExporter {

    private TrialBalanceQuery _query = new TrialBalanceQuery();

    private readonly FileTemplateConfig _templateConfig;

    private ExcelFile _excelFile;

    public BalanzaTradicionalReclasificadaExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, nameof(templateConfig));

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(DynamicDto<BalanzaTradicionalRealDto> balanzaTradicional) {
      Assertion.Require(balanzaTradicional, nameof(balanzaTradicional));

      _query = (TrialBalanceQuery) balanzaTradicional.Query;

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader();

      SetTable(balanzaTradicional.Entries);

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


    private void SetTable(FixedList<BalanzaTradicionalRealDto> entries) {

      int i = _templateConfig.FirstRowIndex;

      foreach (var entry in entries) {

        _excelFile.SetCell($"A{i}", entry.AccountNo);
        _excelFile.SetCell($"B{i}", entry.AccountName);
        _excelFile.SetCell($"C{i}", entry.OperationType);
        _excelFile.SetCell($"D{i}", entry.CurrencyCode);
        _excelFile.SetCell($"E{i}", entry.RealInitialBalance);
        _excelFile.SetCell($"F{i}", entry.RealDebits);
        _excelFile.SetCell($"G{i}", entry.RealCredits);
        _excelFile.SetCell($"G{i}", entry.RealFinalBalance);

        i++;
      }
    }

    #endregion Private methods

  }  // class BalanzaAnaliticaOperacionesExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Reclassification
