/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : GroupingRulesReportExcelExporter             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Main service to export grouping rules information to Microsoft Excel.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Rules.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Main service to export grouping rules information to Microsoft Excel.</summary>
  internal class GroupingRulesReportExcelExporter {

    private readonly FileTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public GroupingRulesReportExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(FixedList<GroupingRulesTreeItemDto> rulesTreeItems) {
      Assertion.AssertObject(rulesTreeItems, "rulesTreeItems");

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader(_excelFile);

      SetTable(rulesTreeItems);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }

    private void SetTable(FixedList<GroupingRulesTreeItemDto> rulesTreeItems) {
      int i = 5;

      foreach (var rule in rulesTreeItems) {
        _excelFile.SetCell($"A{i}", rule.ItemCode);
        _excelFile.SetCell($"B{i}", rule.Type == Rules.GroupingRuleItemType.Agrupation ? "Concepto": "Cuenta");
        _excelFile.SetCell($"C{i}", rule.ItemName);
        if (rule.Level > 1) {
          _excelFile.IndentCell($"C{i}", rule.Level - 1);
        }
        _excelFile.SetCell($"D{i}", rule.SubledgerAccount);
        _excelFile.SetCell($"E{i}", rule.SubledgerAccountName);
        _excelFile.SetCell($"F{i}", rule.SectorCode);
        _excelFile.SetCell($"G{i}", rule.CurrencyCode);
        _excelFile.SetCell($"H{i}", rule.Operator);
        _excelFile.SetCell($"I{i}", rule.Qualification);
        _excelFile.SetCell($"J{i}", rule.Level);
        _excelFile.SetCell($"K{i}", rule.ParentCode);

        i++;
      }
    }

    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _templateConfig.Title);
    }


  }  // class GroupingRulesReportExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
