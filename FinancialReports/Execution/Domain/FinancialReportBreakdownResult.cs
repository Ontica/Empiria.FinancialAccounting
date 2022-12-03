/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportBreakdownResult             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that holds information about a financial report breakdown entry.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Data structure that holds information about a financial report breakdown entry.</summary>
  internal class FinancialReportBreakdownResult : FinancialReportEntry, IFinancialReportResult {

    #region Constructors and parsers

    internal FinancialReportBreakdownResult(FinancialConceptEntry integrationEntry) {
      this.IntegrationEntry = integrationEntry;
    }

    #endregion Constructors and parsers

    public FinancialConceptEntry IntegrationEntry {
      get;
    }


    public FinancialConcept FinancialConcept {
      get {
        return IntegrationEntry.FinancialConcept;
      }
    }


    public override IEnumerable<string> GetDynamicMemberNames() {
      var members = new List<string>();

      members.Add("IntegrationEntry");

      members.AddRange(base.GetDynamicMemberNames());

      return members;
    }

  } // class FinancialReportBreakdownResult

}  // namespace Empiria.FinancialAccounting.FinancialReports
