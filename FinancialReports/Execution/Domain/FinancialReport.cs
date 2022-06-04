/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReport                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the header and entries of a financial report.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Contains the header and entries of a financial report.</summary>
  public class FinancialReport {

    #region Constructors and parsers

    internal FinancialReport(FinancialReportQuery buildQuery,
                             FixedList<FinancialReportEntry> entries) {
      Assertion.Require(buildQuery, nameof(buildQuery));
      Assertion.Require(entries, nameof(entries));

      this.BuildQuery = buildQuery;
      this.FinancialReportType = buildQuery.GetFinancialReportType();
      this.Entries = entries;
    }

    #endregion Constructors and parsers

    #region Properties

    public FinancialReportType FinancialReportType {
      get;
    }

    public FinancialReportQuery BuildQuery {
      get;
    }

    public FixedList<FinancialReportEntry> Entries {
      get;
    }

    #endregion Properties

  } // class FinancialReport

} // namespace Empiria.FinancialAccounting.FinancialReports
