/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : Polizas                                       License   : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Polizas actualizadas o pendientes de actualizar para reporte operativo.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.Domain;
using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.Reporting.Builders {

  /// <summary>Polizas actualizadas o pendientes de actualizar para reporte operativo.</summary>
  internal class Polizas : IReportBuilder {


    #region Public methods

    public ReportDataDto Build(BuildReportCommand command) {
      Assertion.AssertObject(command, "command");

      PolizasCommand polizasCommand = GetPolizasCommand(command);

      PolizasDto polizas = BuildPolizasReport(polizasCommand);

      return MapToReportDataDto(command, polizas);
    }

    private PolizasDto BuildPolizasReport(PolizasCommand polizasCommand) {

      var helper = new PolizasHelper(polizasCommand);
      FixedList<PolizaEntry> entries = helper.GetPolizaEntries();

      var returnPolizas = new FixedList<IPolizaEntry>(entries.Select(x => (IPolizaEntry) x));

      PolizasBuilder polizas = new PolizasBuilder(polizasCommand, returnPolizas);

      return PolizasMapper.Map(polizas);
    }

    #endregion

    #region Private methods


    static private FixedList<DataTableColumn> GetReportColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("ledgerNumber", "Numero mayor", "text"));
      columns.Add(new DataTableColumn("ledgerName", "Nombre mayor", "text"));
      columns.Add(new DataTableColumn("voucherNumber", "Numero poliza", "text"));
      columns.Add(new DataTableColumn("accountingDate", "Afectacion", "date"));
      columns.Add(new DataTableColumn("recordingDate", "Elaboracion", "date"));
      columns.Add(new DataTableColumn("elaboratedBy", "Elaborado por", "text"));
      columns.Add(new DataTableColumn("concept", "Concepto", "text"));
      columns.Add(new DataTableColumn("debit", "Cargos", "decimal"));
      columns.Add(new DataTableColumn("credit", "Abonos", "decimal"));

      return columns.ToFixedList();
    }



    private PolizasCommand GetPolizasCommand(BuildReportCommand command) {
      return new PolizasCommand {
        AccountsChartUID = AccountsChart.Parse(command.AccountsChartUID).UID,
        Ledgers = command.Ledgers,
        FromDate = command.FromDate,
        ToDate = command.ToDate
      };
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

    static private PolizaReturnedEntry MapToPolizaEntry(PolizasEntryDto entry) {
      return new PolizaReturnedEntry {
        LedgerNumber = entry.LedgerNumber,
        LedgerName = entry.LedgerName,
        VoucherNumber = entry.VoucherNumber,
        AccountingDate = entry.AccountingDate,
        RecordingDate = entry.RecordingDate,
        ElaboratedBy = entry.ElaboratedBy,
        Concept = entry.Concept,
        Debit = entry.Debit,
        Credit = entry.Credit
      };
    }
    #endregion Private methods



  } // PolizasActualizadas

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


    public DateTime AccountingDate {
      get; internal set;
    }


    public DateTime RecordingDate {
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

  }  // class Polizas

} // namespace Empiria.FinancialAccounting.Reporting.Builders
