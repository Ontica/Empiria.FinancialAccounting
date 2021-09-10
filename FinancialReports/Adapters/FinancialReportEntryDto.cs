/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Data Transfer Object                    *
*  Type     : FinancialReportEntryDto                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return the entries of a financial report.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Output DTO used to return the entries of a financial report.</summary>
  public class FinancialReportEntryDto {

    public FinancialReportItemType ItemType {
      get; internal set;
    }

    public string GroupingRuleUID {
      get; internal set;
    }

    public string ConceptCode {
      get; internal set;
    }

    public string Concept {
      get; internal set;
    }

    public decimal DomesticCurrencyTotal {
      get; internal set;
    }

    public decimal ForeignCurrencyTotal {
      get; internal set;
    }

    public decimal Total {
      get; internal set;
    }

  } // class FinancialReportEntryDto

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
