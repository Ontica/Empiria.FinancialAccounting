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

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {


  /// <summary>Interface used to homologate all financial report result types.</summary>
  internal interface IFinancialReportResult {

    FinancialConcept FinancialConcept { get; }

  }  // IFinancialReportResult


  /// <summary>Represents a calculated entry for a financial report.</summary>
  public class FinancialReportEntryResult : FinancialReportEntry, IFinancialReportResult {

    internal FinancialReportEntryResult(FinancialReportItemDefinition reportItemDefinition) {
      Definition = reportItemDefinition;
    }

    #region Properties

    public string UID {
      get {
        return Definition.UID;
      }
    }


    public string Label {
      get {
        return Definition.Label;
      }
    }


    public FinancialConcept FinancialConcept {
      get {
        return Definition.FinancialConcept;
      }
    }


    public FinancialReportItemDefinition Definition {
      get;
    }

    #endregion Properties

  } // class FinancialReportEntryResult

} // namespace Empiria.FinancialAccounting.FinancialReports
