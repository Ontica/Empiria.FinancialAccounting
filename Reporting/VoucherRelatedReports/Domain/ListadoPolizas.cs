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

    public ReportDataDto Build(BuildReportCommand command) {
      Assertion.Require(command, "command");

      ListadoPolizasCommand voucherCommand = GetPolizasCommand(command);

      PolizasDto polizas = BuildPolizasReport(voucherCommand);

      return MapToReportDataDto(command, polizas);
    }

    private PolizasDto BuildPolizasReport(ListadoPolizasCommand voucherCommand) {

      var helper = new ListadoPolizasHelper(voucherCommand);
      FixedList<PolizaEntry> entries = helper.GetPolizaEntries();

      FixedList<PolizaEntry> listadoPolizas = helper.GetListadoPolizasConTotales(entries);

      var returnPolizas = new FixedList<IPolizaEntry>(listadoPolizas.Select(x => (IPolizaEntry) x));

      ListadoPolizasBuilder polizas = new ListadoPolizasBuilder(voucherCommand, returnPolizas);

      return PolizasMapper.Map(polizas);
    }

    #endregion

    #region Private methods


    private ListadoPolizasCommand GetPolizasCommand(BuildReportCommand command) {
      return new ListadoPolizasCommand {
        AccountsChartUID = AccountsChart.Parse(command.AccountsChartUID).UID,
        Ledgers = command.Ledgers,
        FromDate = command.FromDate,
        ToDate = command.ToDate
      };
    }


    static private FixedList<DataTableColumn> GetReportColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

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


    static private ReportDataDto MapToReportDataDto(BuildReportCommand command,
                                                    PolizasDto polizas) {
      return new ReportDataDto {
        Command = command,
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
