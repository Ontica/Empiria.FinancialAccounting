/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportBreakdownEntry              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that holds information about a financial report breakdown entry.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.FinancialAccounting.Rules;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Data structure that holds information about a financial report breakdown entry.</summary>
  internal class FinancialReportBreakdownEntry : FinancialReportEntry {

    #region Constructors and parsers

    internal FinancialReportBreakdownEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers

    public GroupingRuleItem GroupingRuleItem {
      get;
      internal set;
    }

    public decimal DomesticCurrencyTotal {
      get;
      internal set;
    }

    public decimal ForeignCurrencyTotal {
      get;
      internal set;
    }

    public decimal Total {
      get;
      internal set;
    }

  } // class FinancialReportBreakdownEntry

}  // namespace Empiria.FinancialAccounting.FinancialReports
