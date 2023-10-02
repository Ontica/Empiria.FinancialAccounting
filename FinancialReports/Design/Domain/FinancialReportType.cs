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
using System.Collections.Generic;

using Empiria.Storage;

using Empiria.FinancialAccounting.ExternalData;
using Empiria.FinancialAccounting.FinancialConcepts;

using Empiria.FinancialAccounting.FinancialReports.Data;

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

    ValorizacionEstimacionPreventiva,

  }


  /// <summary>Describes a financial report.</summary>
  public class FinancialReportType : GeneralObject {

    #region Fields

    private Lazy<List<FinancialReportItemDefinition>> _items;

    #endregion Fields

    #region Constructors and parsers

    protected FinancialReportType() {
      // Required by Empiria Framework.
    }


    static public FinancialReportType Parse(int id) {
      return BaseObject.ParseId<FinancialReportType>(id, false);
    }


    static public FinancialReportType Parse(string uid) {
      return BaseObject.ParseKey<FinancialReportType>(uid, false);
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


    static public FinancialReportType Empty => BaseObject.ParseEmpty<FinancialReportType>();


    protected override void OnLoad() {
      if (this.IsEmptyInstance) {
        return;
      }

      if (BaseReport.IsEmptyInstance) {
        _items = new Lazy<List<FinancialReportItemDefinition>>(() => FinancialReportsData.GetItems(this));
      } else {
        _items = BaseReport._items;
      }
    }


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
          return DataColumns.FindAll(x => x.Show && x.Column.Length != 0)
                            .Select(x => x.Column)
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
        return base.ExtendedDataField.Get<int>("grid/endRow", 100);
      }
    }


    public bool PrecalculateConcepts {
      get {
        return !PrecalculateConceptsFromReportType.IsEmptyInstance;
      }
    }


    public FinancialReportType PrecalculateConceptsFromReportType {
      get {
        return base.ExtendedDataField.Get<FinancialReportType>("precalculatedConcepts/reportType",
                                                               FinancialReportType.Empty);
      }
    }


    public FixedList<FinancialConceptGroup> PrecalculatedConceptsGroups {
      get {
        return base.ExtendedDataField.GetList<FinancialConceptGroup>("precalculatedConcepts/groups", false)
                                     .ToFixedList();
      }
    }

    public int RowsOffset {
      get {
        return base.ExtendedDataField.Get<int>("rowsOffset", 0);
      }
    }


    #endregion Properties

    #region Methods

    internal FinancialReportCell GetCell(string cellUID) {
      return GetCells().Find(x => x.UID == cellUID);
    }


    public FixedList<FinancialReportCell> GetCells() {
      return GetItems().FindAll(x => x is FinancialReportCell)
                       .Select(y => (FinancialReportCell) y).ToFixedList();
    }


    internal FinancialReportItemDefinition GetItem(string rowOrCellUID) {
      return GetItems().Find(x => x.UID == rowOrCellUID);
    }


    internal FixedList<FinancialReportItemDefinition> GetItems() {
      if (BaseReport.IsEmptyInstance) {
        return _items.Value.ToFixedList();

      } else {
        return BaseReport.GetItems();
      }
    }


    public FixedList<FinancialReportRow> GetRows() {
      return GetItems().FindAll(x => x is FinancialReportRow)
                       .Select(y => (FinancialReportRow) y).ToFixedList();
    }


    internal FinancialReportRow GetRow(string rowUID) {
      return GetRows().Find(x => x.UID == rowUID);
    }


    public ExportTo GetExportToConfig(string exportToUID) {
      return ExportTo.Find(x => x.UID == exportToUID);
    }


    internal FinancialReportCell InsertCell(ReportCellFields cellFields) {
      Assertion.Require(cellFields, nameof(cellFields));

      var cell = new FinancialReportCell(this, cellFields);

      UpdateListWith(cell);

      return cell;
    }


    internal FinancialReportRow InsertRow(ReportRowFields rowFields) {
      Assertion.Require(rowFields, nameof(rowFields));

      var row = new FinancialReportRow(this, rowFields);

      UpdateListWith(row);

      return row;
    }


    internal void RemoveCell(FinancialReportCell cell) {
      Assertion.Require(cell, nameof(cell));

      Assertion.Require(cell.FinancialReportType.Equals(this),
                        $"La celda {cell.UID} no pertenece al reporte {this.Name}.");

      cell.Delete();

      UpdateListWith(cell);
    }


    internal void RemoveRow(FinancialReportRow row) {
      Assertion.Require(row, nameof(row));

      Assertion.Require(row.FinancialReportType.Equals(this),
                        $"La fila {row.UID} no pertenece al reporte {this.Name}.");

      row.Delete();

      UpdateListWith(row);
    }


    internal void UpdateCell(FinancialReportCell cell,
                             ReportCellFields cellFields) {
      Assertion.Require(cell, nameof(cell));
      Assertion.Require(cellFields, nameof(cellFields));

      Assertion.Require(cell.FinancialReportType.Equals(this),
                        $"La celda {cell.UID} no pertenece al reporte {this.Name}.");

      cell.Update(cellFields);

      UpdateListWith(cell);
    }


    internal void UpdateRow(FinancialReportRow row,
                            ReportRowFields rowFields) {
      Assertion.Require(row, nameof(row));
      Assertion.Require(rowFields, nameof(rowFields));

      Assertion.Require(row.FinancialReportType.Equals(this),
                        $"La fila {row.UID} no pertenece al reporte {this.Name}.");

      row.Update(rowFields);

      UpdateListWith(row);
    }


    #endregion Methods

    #region Helpers

    private void UpdateListWith(FinancialReportCell cell) {
      int listIndex = _items.Value.IndexOf(cell);

      if (listIndex != -1) {
        _items.Value.RemoveAt(listIndex);
      }

      if (cell.Status == StateEnums.EntityStatus.Deleted) {
        return;
      }

      var offsetCell = _items.Value.FindLast(x => x.RowIndex <= cell.RowIndex);

      int insertionIndex = offsetCell != null ? _items.Value.IndexOf(offsetCell) : 0;

      _items.Value.Insert(insertionIndex, cell);
    }


    private void UpdateListWith(FinancialReportRow row) {
      int listIndex = _items.Value.IndexOf(row);

      if (listIndex != -1) {
        _items.Value.RemoveAt(listIndex);
      }

      for (int i = 0; i < _items.Value.Count; i++) {
        FinancialReportRow item = (FinancialReportRow) _items.Value[i];

        item.SetRowIndex(i + 1);
      }

      if (row.Status == StateEnums.EntityStatus.Deleted) {
        return;
      }

      _items.Value.Insert(row.RowIndex - 1, row);

      for (int i = row.RowIndex; i < _items.Value.Count; i++) {
        FinancialReportRow item = (FinancialReportRow) _items.Value[i];

        item.SetRowIndex(i + 1);
      }
    }

    #endregion Helpers

  }  // class FinancialReportType

}  // namespace Empiria.FinancialAccounting.FinancialReports
