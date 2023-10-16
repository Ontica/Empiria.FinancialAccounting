/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportRow                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a financial report fixed row.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialReports.Data;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Describes a financial report fixed row.</summary>
  public class FinancialReportRow : FinancialReportItemDefinition, IPositionable {

    #region Constructors and parsers

    protected FinancialReportRow() {
      // Required by Empiria Framework.
    }


    internal FinancialReportRow(FinancialReportType reportType, ReportRowFields fields) : base(reportType) {
      Assertion.Require(fields, nameof(fields));

      Update(fields);
    }


    static new public FinancialReportRow Parse(int id) {
      return BaseObject.ParseId<FinancialReportRow>(id);
    }


    static new public FinancialReportRow Parse(string uid) {
      return BaseObject.ParseKey<FinancialReportRow>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    int IPositionable.Position {
      get {
        return base.RowIndex;
      }
    }

    public string[] BlockedCells {
      get {
        if (!base.ExtendedData.Contains("blockedCells")) {
          return new string[0];
        }
        return base.ExtendedData.Get<string>("blockedCells").Split(',');
      }
    }

    #endregion Properties

    #region Methods

    protected override void OnSave() {
      FinancialReportsData.Write(this);
    }


    internal void Update(ReportRowFields fields) {
      base.Update(fields);
    }

    #endregion Methods

  }  // class FinancialReportRow

}  // namespace Empiria.FinancialAccounting.FinancialReports
