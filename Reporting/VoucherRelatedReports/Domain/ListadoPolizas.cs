/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : ListadoPolizas                                License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Listado de Polizas para reporte operativo.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.Domain;

namespace Empiria.FinancialAccounting.Reporting.Builders {

  /// <summary>Listado de Polizas para reporte operativo.</summary>
  internal class ListadoPolizas : IReportBuilder {


    #region Public methods

    public ReportDataDto Build(ReportBuilderQuery query) {
      Assertion.Require(query, nameof(query));

      ListadoPolizasQuery listadoPolizasQuery = MapToListadoPolizasQuery(query);

      PolizasDto polizas = BuildPolizasReport(listadoPolizasQuery);

      return MapToReportDataDto(query, polizas);
    }

    private PolizasDto BuildPolizasReport(ListadoPolizasQuery query) {

      var helper = new ListadoPolizasHelper(query);

      FixedList<PolizaEntry> entries = helper.GetPolizaEntries();

      FixedList<PolizaEntry> polizasConTotales = helper.SetTotalsRows(entries);

      var polizasToReturn = new FixedList<IPolizaEntry>(polizasConTotales.Select(x => (IPolizaEntry) x));

      return PolizasMapper.Map(polizasToReturn, query);
    }

    #endregion

    #region Private methods


    private ListadoPolizasQuery MapToListadoPolizasQuery(ReportBuilderQuery reportBuilderQuery) {
      return new ListadoPolizasQuery {
        AccountsChartUID = reportBuilderQuery.AccountsChartUID,
        Ledgers = reportBuilderQuery.Ledgers,
        FromDate = reportBuilderQuery.FromDate,
        ToDate = reportBuilderQuery.ToDate
      };
    }


    static private FixedList<DataTableColumn> GetReportColumns() {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("ledgerName", "Mayor", "text-nowrap"));
      columns.Add(new DataTableColumn("voucherNumber", "No. Póliza", "text-nowrap"));
      columns.Add(new DataTableColumn("accountingDate", "Afectación", "text"));
      columns.Add(new DataTableColumn("recordingDate", "Elaboración", "text"));
      columns.Add(new DataTableColumn("elaboratedBy", "Elaborado por", "text"));
      columns.Add(new DataTableColumn("concept", "Concepto", "text"));
      columns.Add(new DataTableColumn("debit", "Cargos", "decimal"));
      columns.Add(new DataTableColumn("credit", "Abonos", "decimal"));

      return columns.ToFixedList();
    }


    static private PolizaReturnedEntry MapToPolizaEntry(PolizasEntryDto voucher) {
      var polizaEntry = new PolizaReturnedEntry();

      polizaEntry.LedgerName = voucher.ItemType == ItemType.Group ||
                                  voucher.ItemType == ItemType.Total ? "" :
                                  voucher.LedgerName;

      polizaEntry.VoucherNumber = voucher.ItemType == ItemType.Group ||
                                  voucher.ItemType == ItemType.Total ? "" :
                                  voucher.VoucherNumber;

      polizaEntry.AccountingDate = voucher.ItemType == ItemType.Group ||
                                   voucher.ItemType == ItemType.Total ? "" :
                                   voucher.AccountingDate.ToString("dd/MM/yyyy");

      polizaEntry.RecordingDate = voucher.ItemType == ItemType.Group ||
                                  voucher.ItemType == ItemType.Total ? "" :
                                  voucher.RecordingDate.ToString("dd/MM/yyyy");

      polizaEntry.ElaboratedBy = voucher.ItemType == ItemType.Group ||
                                 voucher.ItemType == ItemType.Total ? "" :
                                 voucher.ElaboratedBy;

      if (voucher.ItemType == ItemType.Group) {
        polizaEntry.Concept = $"POLIZAS POR CONTABILIDAD: {voucher.VouchersByLedger}";

      } else if (voucher.ItemType == ItemType.Total) {
        polizaEntry.Concept = $"TOTAL DE POLIZAS: {voucher.VouchersByLedger}";

      } else {
        polizaEntry.Concept = voucher.Concept;

      }

      polizaEntry.Debit = voucher.Debit;
      polizaEntry.Credit = voucher.Credit;
      polizaEntry.ItemType = voucher.ItemType;

      return polizaEntry;
    }


    static private ReportDataDto MapToReportDataDto(ReportBuilderQuery query,
                                                    PolizasDto polizas) {
      return new ReportDataDto {
        Query = query,
        Columns = GetReportColumns(),
        Entries = MapToReportDataEntries(polizas.Entries)
      };
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries(FixedList<IPolizasDto> list) {

      var mappedItems = list.Select((x) => MapToPolizaEntry((PolizasEntryDto) x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    #endregion Private methods



  } // ListadoPolizas


  public class PolizaReturnedEntry : IReportEntryDto {

    public string LedgerNumber {
      get; internal set;
    }


    public string LedgerName {
      get; internal set;
    }


    public string VoucherNumber {
      get;
      internal set;
    }


    public string AccountingDate {
      get; internal set;
    }


    public string RecordingDate {
      get; internal set;
    }


    public string ElaboratedBy {
      get; internal set;
    }


    public string Concept {
      get; internal set;
    }


    public decimal Debit {
      get; internal set;
    }


    public decimal Credit {
      get; internal set;
    }

    public int VouchersByLedger {
      get; internal set;
    }


    public ItemType ItemType {
      get; internal set;
    } = ItemType.Entry;


  }  // class PolizaReturnedEntry

} // namespace Empiria.FinancialAccounting.Reporting.Builders
