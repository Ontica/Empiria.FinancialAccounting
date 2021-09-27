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

using Empiria.FinancialAccounting.Rules;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Represents an entry for financial report defined by fixed rows.</summary>
  public class FixedRowFinancialReportEntry {

    #region Constructors and parsers

    internal FixedRowFinancialReportEntry() {
      // Required by Empiria Framework.
    }


    #endregion Constructors and parsers

    public FinancialReportRow Row {
      get;
      internal set;
    }


    public FinancialReportItemType ItemType {
      get;
      internal set;
    } = FinancialReportItemType.Entry;


    public GroupingRule GroupingRule {
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

  } // class FixedRowFinancialReportEntry

} // namespace Empiria.FinancialAccounting.FinancialReports
