/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportType                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a financial report.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialReports.Data;

namespace Empiria.FinancialAccounting.FinancialReports {

  public enum FinancialReportDesignType {

    FixedRows,

    ConceptsIntegration
  }


  /// <summary>Describes a financial report.</summary>
  public class FinancialReportType: GeneralObject {

    #region Constructors and parsers

    protected FinancialReportType() {
      // Required by Empiria Framework.
    }


    static public FinancialReportType Parse(int id) {
      return BaseObject.ParseId<FinancialReportType>(id);
    }


    static public FinancialReportType Parse(string uid) {
      return BaseObject.ParseKey<FinancialReportType>(uid);
    }


    static public FixedList<FinancialReportType> GetList() {
      return BaseObject.GetList<FinancialReportType>(string.Empty, "ObjectName")
                       .ToFixedList();
    }


    static public FixedList<FinancialReportType> GetList(AccountsChart accountsChart) {
      var fullList = GetList();

      return fullList.FindAll(x => x.AccountsChart.Equals(accountsChart));
    }

    static internal FixedList<FinancialReportType> GetListForDesign(AccountsChart accountsChart) {
      var fullList = GetList();

      return fullList.FindAll(x => x.AccountsChart.Equals(accountsChart) && x.IsDesignable);
    }

    #endregion Constructors and parsers

    #region Properties

    public AccountsChart AccountsChart {
      get {
        return base.ExtendedDataField.Get<AccountsChart>("accountsChartId");
      }
    }

    public FinancialReportDesignType DesignType {
      get {
        return base.ExtendedDataField.Get<FinancialReportDesignType>("designType");
      }
    }


    public bool IsDesignable {
      get {
        return this.DesignType == FinancialReportDesignType.FixedRows;
      }
    }


    public int TemplateFileId {
      get {
        return base.ExtendedDataField.Get("templateFileId", -1);
      }
    }

    #endregion Properties

    #region Methods

    public FixedList<FinancialReportRow> GetRows() {
      return FinancialReportsRowData.GetRows(this);
    }

    #endregion Methods

  }  // class FinancialReportType

}  // namespace Empiria.FinancialAccounting.FinancialReports
