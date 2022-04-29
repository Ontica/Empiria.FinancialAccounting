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

using Empiria.Json;

using Empiria.FinancialAccounting.FinancialReports.Data;

namespace Empiria.FinancialAccounting.FinancialReports {

  public enum FinancialReportDesignType {

    AccountsIntegration,

    FixedCells,

    FixedRows

  }


  public enum FinancialReportDataSource {

    AnaliticoCuentas,

    BalanzaEnColumnasPorMoneda

  }


  /// <summary>Describes a financial report.</summary>
  public class FinancialReportType : GeneralObject {

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

    static readonly FinancialReportType Empty = BaseObject.ParseEmpty<FinancialReportType>();

    #endregion Constructors and parsers

    #region Properties

    public AccountsChart AccountsChart {
      get {
        return base.ExtendedDataField.Get<AccountsChart>("accountsChartId");
      }
    }


    public FixedList<DataTableColumn> DataColumns {
      get {
        return base.ExtendedDataField.GetFixedList<DataTableColumn>("dataColumns", false);
      }
    }


    public FixedList<DataTableColumn> BreakdownColumns {
      get {
        return base.ExtendedDataField.GetFixedList<DataTableColumn>("breakdownColumns", false);
      }
    }


    public FinancialReportDataSource DataSource {
      get {
        return base.ExtendedDataField.Get<FinancialReportDataSource>("dataSource");
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


    public bool RoundDecimals {
      get {
        return base.ExtendedDataField.Get("roundDecimals", false);
      }
    }


    public FixedList<ExportTo> ExportTo {
      get {
        return base.ExtendedDataField.GetFixedList<ExportTo>("exportTo", false);
      }
    }

    public FinancialReportType BaseReport {
      get {
        return base.ExtendedDataField.Get("baseReportId", FinancialReportType.Empty);
      }
    }

    #endregion Properties

    #region Methods

    public FixedList<FinancialReportRow> GetRows() {
      if (BaseReport.IsEmptyInstance) {
        return FinancialReportsRowData.GetRows(this);
      } else {
        return BaseReport.GetRows();
      }
    }


    public ExportTo GetExportToConfig(string exportToUID) {
      return ExportTo.Find(x => x.UID == exportToUID);
    }


    internal FinancialReportRow GetRow(string rowUID) {
      return GetRows().Find(x => x.UID == rowUID);
    }


    #endregion Methods

  }  // class FinancialReportType

}  // namespace Empiria.FinancialAccounting.FinancialReports
