/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : FinancialConceptsEntriesTreeExcelExporter    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Exports financial concepts integration entries as tree to Microsoft Excel.                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialConcepts;
using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Exports financial concepts integration entries as tree to Microsoft Excel.</summary>
  internal class FinancialConceptsEntriesTreeExcelExporter {

    private readonly FileTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public FinancialConceptsEntriesTreeExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(FixedList<FinancialConceptEntryAsTreeNodeDto> treeNodes) {
      Assertion.Require(treeNodes, nameof(treeNodes));

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader(_excelFile);

      SetTable(treeNodes);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    private void SetTable(FixedList<FinancialConceptEntryAsTreeNodeDto> treeNodes) {
      int i = 5;

      foreach (var node in treeNodes) {
        _excelFile.SetCell($"A{i}", node.ItemCode);
        _excelFile.SetCell($"B{i}", node.Type == FinancialConceptEntryType.FinancialConceptReference ? "Concepto": "Cuenta");
        _excelFile.SetCell($"C{i}", node.ItemName);

        if (node.Level > 1) {
          _excelFile.IndentCell($"C{i}", node.Level - 1);
        }

        _excelFile.SetCell($"D{i}", node.SubledgerAccount);
        _excelFile.SetCell($"E{i}", node.SubledgerAccountName);
        _excelFile.SetCell($"F{i}", node.SectorCode);
        _excelFile.SetCell($"G{i}", node.CurrencyCode);
        _excelFile.SetCell($"H{i}", node.Operator);
        _excelFile.SetCell($"I{i}", node.DataColumn);
        _excelFile.SetCell($"J{i}", node.Level);
        _excelFile.SetCell($"K{i}", node.ParentCode);

        i++;
      }
    }


    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _templateConfig.Title);
    }


  }  // class FinancialConceptsEntriesTreeExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
