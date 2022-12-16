/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportCell                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a financial report fixed cell.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialReports.Data;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Describes a financial report fixed row.</summary>
  public class FinancialReportCell : FinancialReportItemDefinition {

    #region Constructors and parsers

    protected FinancialReportCell() {
      // Required by Empiria Framework.
    }


    static public new FinancialReportCell Parse(int id) {
      return BaseObject.ParseId<FinancialReportCell>(id);
    }


    static public new FinancialReportCell Parse(string uid) {
      return BaseObject.ParseKey<FinancialReportCell>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("DATA_FIELD")]
    public string DataField {
      get; private set;
    }


    [DataField("COLUMNA")]
    public string ColumnIndex {
      get; private set;
    }

    #endregion Properties

    #region Methods

    protected override void OnSave() {
      FinancialReportsData.Write(this);
    }

    #endregion Methods

  }  // class FinancialReportCell

}  // namespace Empiria.FinancialAccounting.FinancialReports
