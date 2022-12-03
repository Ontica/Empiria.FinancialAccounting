/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FixedRowFinancialReportEntry               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a financial report defined by fixed rows.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Represents an entry for financial report defined by fixed rows.</summary>
  public class FixedRowFinancialReportEntry : FinancialReportEntry {

    private readonly FinancialReportItemDefinition _reportItemDefinition;

    internal FixedRowFinancialReportEntry(FinancialReportItemDefinition reportItemDefinition) {
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

  } // class FixedRowFinancialReportEntry

} // namespace Empiria.FinancialAccounting.FinancialReports
