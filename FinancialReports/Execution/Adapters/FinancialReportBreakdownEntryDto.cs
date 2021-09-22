/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Data Transfer Object                    *
*  Type     : FinancialReportEntryDto                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return the entries of a financial report breakdown.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.FinancialAccounting.Rules;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Output DTO used to return the entries of a financial report breakdown.</summary>
  public class FinancialReportBreakdownEntryDto {

    public string UID {
      get; internal set;
    }

    public GroupingRuleItemType Type {
      get;
      internal set;
    }

    public string GroupingRuleUID {
      get; internal set;
    }

    public string ItemName {
      get;
      internal set;
    }

    public string ItemCode {
      get;
      internal set;
    }

    public string SubledgerAccount {
      get;
      internal set;
    }

    public string SectorCode {
      get;
      internal set;
    }

    public string Operator {
      get;
      internal set;
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

  } // class FinancialReportBreakdownEntryDto

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
