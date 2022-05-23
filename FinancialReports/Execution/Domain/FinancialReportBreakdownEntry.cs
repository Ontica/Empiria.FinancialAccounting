/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportBreakdownEntry              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that holds information about a financial report breakdown entry.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Data structure that holds information about a financial report breakdown entry.</summary>
  internal class FinancialReportBreakdownEntry : FinancialReportEntry {

    #region Constructors and parsers

    internal FinancialReportBreakdownEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers

    public FinancialConceptEntry IntegrationEntry {
      get;
      internal set;
    }

    public override IEnumerable<string> GetDynamicMemberNames() {
      var members = new List<string>();

      members.Add("IntegrationEntry");

      members.AddRange(base.GetDynamicMemberNames());

      return members;
    }

  } // class FinancialReportBreakdownEntry

}  // namespace Empiria.FinancialAccounting.FinancialReports
