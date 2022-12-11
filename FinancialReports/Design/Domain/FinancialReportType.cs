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
using Empiria.FinancialAccounting.FinancialConcepts;

using Empiria.FinancialAccounting.ExternalData;

namespace Empiria.FinancialAccounting.FinancialReports {

  public enum FinancialReportDesignType {

    AccountsIntegration,

    FixedCells,

    FixedRows

  }


  public enum FinancialReportDataSource {

    AnaliticoCuentas,

    BalanzaEnColumnasPorMoneda,

    BalanzaTradicional,

  }


  /// <summary>Describes a financial report.</summary>
  public class FinancialReportType : GeneralObject {

    #region Constructors and parsers

    protected FinancialReportType() {
      // Required by Empiria Framework.
    }


    static public FinancialReportType Parse(int id) {
      return BaseObject.ParseId<FinancialReportType>(id, true);
    }


    static public FinancialReportType Parse(string uid) {
      return BaseObject.ParseKey<FinancialReportType>(uid, true);
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


    public bool ConsecutiveRows {
      get {
        return base.ExtendedDataField.Get<bool>("consecutiveRows", true);
      }
    }

    public bool IsDesignable {
      get {
        return this.DesignType != FinancialReportDesignType.AccountsIntegration;
      }
    }


    public RoundTo RoundTo {
      get {
        return base.ExtendedDataField.Get("roundTo", RoundTo.DoNotRound);
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


    public string Title {
      get {
        return base.ExtendedDataField.Get("title", string.Empty);
      }
    }


    public FixedList<FinancialConceptGroup> FinancialConceptGroups {
      get {
        return base.ExtendedDataField.GetFixedList<FinancialConceptGroup>("groupingRules", false);
      }
    }


    public FixedList<ExternalVariablesSet> ExternalVariablesSets {
      get {
        return base.ExtendedDataField.GetFixedList<ExternalVariablesSet>("externalVariablesSets", false);
      }
    }


    public FixedList<NamedEntity> DataFields {
      get {
        return DataColumns.Select(x => new NamedEntity(x.Field, x.Title))
                          .ToFixedList();
      }
    }

    public FixedList<DataTableColumn> OutputDataColumns {
      get {
        return DataColumns.FindAll(x => x.Show);
      }
    }

    public FixedList<string> GridColumns {
      get {
        if (DesignType == FinancialReportDesignType.FixedRows ||
            DesignType == FinancialReportDesignType.AccountsIntegration) {
          return DataColumns.Select(x => x.Column)
                            .ToFixedList();
        }
        return base.ExtendedDataField.GetFixedList<string>("grid/columns");
      }
    }


    public int GridStartRow {
      get {
        return base.ExtendedDataField.Get<int>("grid/startRow", 1);
      }
    }


    public int GridEndRow {
      get {
        return base.ExtendedDataField.Get<int>("grid/endRow", -1);
      }
    }


    #endregion Properties

    #region Methods

    public FixedList<FinancialReportCell> GetCells() {
      return GetItems().FindAll(x => x is FinancialReportCell)
                       .Select(y => (FinancialReportCell) y).ToFixedList();
    }


    public FixedList<FinancialReportRow> GetRows() {
      return GetItems().FindAll(x => x is FinancialReportRow)
                        .Select(y => (FinancialReportRow) y).ToFixedList();
    }


    FixedList<FinancialReportItemDefinition> _items;

    internal FixedList<FinancialReportItemDefinition> GetItems() {
      if (_items != null) {
        return _items;
      }

      if (BaseReport.IsEmptyInstance) {
        _items = FinancialReportsData.GetItems(this);
      } else {
        _items = BaseReport.GetItems();
      }

      return _items;
    }


    public ExportTo GetExportToConfig(string exportToUID) {
      return ExportTo.Find(x => x.UID == exportToUID);
    }


    internal FinancialReportItemDefinition GetItem(string rowOrCellUID) {
      return GetItems().Find(x => x.UID == rowOrCellUID);
    }


    internal FinancialReportRow GetRow(string rowUID) {
      return GetRows().Find(x => x.UID == rowUID);
    }


    internal FinancialReportRow InsertRow(ReportRowFields rowFields, Positioning positioning) {
      throw new NotImplementedException();
    }


    #endregion Methods

  }  // class FinancialReportType

}  // namespace Empiria.FinancialAccounting.FinancialReports
