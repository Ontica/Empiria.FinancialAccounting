/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : FinancialReportGenerator                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to generate a trial balance.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Provides services to generate a trial balance.</summary>
  internal class FinancialReportGenerator {

    internal FinancialReportGenerator(FinancialReportCommand command) {
      Assertion.AssertObject(command, "command");

      this.Command = command;
    }


    public FinancialReportCommand Command {
      get;
    }


    internal FinancialReport BuildFinancialReport() {
      FinancialReportType reportType = this.Command.GetFinancialReportType();

      switch (reportType.DesignType) {
        case FinancialReportDesignType.FixedRows:
          var fixedRows = new FixedRowGroupingRulesReport(this.Command);

          return fixedRows.Generate();

        case FinancialReportDesignType.ConceptsIntegration:
          var conceptsIntegration = new FixedRowGroupingRulesReport(this.Command);

          return conceptsIntegration.Generate();

        default:
          throw Assertion.AssertNoReachThisCode(
                    $"Unhandled financial report design type {reportType.DesignType}.");
      }
    }


    internal FinancialReport GetBreakdown(string groupingRuleUID) {
      FinancialReportType reportType = this.Command.GetFinancialReportType();

      switch (reportType.DesignType) {
        case FinancialReportDesignType.FixedRows:
          var fixedRows = new FixedRowGroupingRulesReport(this.Command);

          return fixedRows.GetBreakdown(groupingRuleUID);

        default:
          throw Assertion.AssertNoReachThisCode(
                    $"Unhandled financial report design type {reportType.DesignType}.");
      }
    }

  } // class FinancialReportGenerator

} // namespace Empiria.FinancialAccounting.FinancialReports
