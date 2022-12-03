/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportEntryResult                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents a calculated entry for a financial report.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Represents a calculated entry for a financial report.</summary>
  public class FinancialReportEntryResult : FinancialReportEntry {

    private readonly FinancialReportItemDefinition _reportItemDefinition;

    internal FinancialReportEntryResult(FinancialReportItemDefinition reportItemDefinition) {
      _reportItemDefinition = reportItemDefinition;
    }

    #region Properties

    public string UID {
      get {
        return _reportItemDefinition.UID;
      }
    }


    public string Label {
      get {
        return _reportItemDefinition.Label;
      }
    }


    public FinancialConcept FinancialConcept {
      get {
        return _reportItemDefinition.FinancialConcept;
      }
    }


    public FinancialReportItemDefinition Definition {
      get {
        return _reportItemDefinition;
      }
    }


    #endregion Properties

    public override IEnumerable<string> GetDynamicMemberNames() {
      var members = new List<string>();

      members.Add("UID");
      members.Add("FinancialConcept");

      members.AddRange(base.GetDynamicMemberNames());

      return members;
    }

  } // class FinancialReportEntryResult

} // namespace Empiria.FinancialAccounting.FinancialReports
