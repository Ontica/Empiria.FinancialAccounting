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

    private readonly ExcelTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public GroupingRulesReportExcelExporter(ExcelTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(FixedList<GroupingRulesTreeItemDto> rulesTreeItems) {
      Assertion.AssertObject(rulesTreeItems, "rulesTreeItems");

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

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
        if (rule.SubledgerAccount.Length != 0) {
          _excelFile.SetCell($"E{i}", "Nombre del auxiliar");
        }
        _excelFile.SetCell($"F{i}", rule.SectorCode);
        _excelFile.SetCell($"G{i}", rule.Operator);
        _excelFile.SetCell($"H{i}", rule.Qualification);
        _excelFile.SetCell($"I{i}", rule.Level);
        _excelFile.SetCell($"J{i}", rule.ParentCode);

        i++;
      }
    }

  }  // class GroupingRulesReportExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
