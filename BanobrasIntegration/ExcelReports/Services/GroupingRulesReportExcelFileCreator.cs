/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Excel Reports                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service                               *
*  Type     : GroupingRulesReportExcelFileCreator          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Main service to export grouping rules information to Microsoft Excel.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Rules.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports {

  /// <summary>Main service to export grouping rules information to Microsoft Excel.</summary>
  internal class GroupingRulesReportExcelFileCreator {

    private readonly ExcelTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public GroupingRulesReportExcelFileCreator(ExcelTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(FixedList<GroupingRuleDto> rules) {
      Assertion.AssertObject(rules, "rules");

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetTable(rules);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }

    private void SetTable(FixedList<GroupingRuleDto> rules) {
      int i = 5;

      foreach (var rule in rules) {
        _excelFile.SetCell($"A{i}", rule.Code);
        _excelFile.SetCell($"B{i}", rule.Concept);
        i++;
      }
    }

  }  // class GroupingRulesReportExcelFileCreator

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports
