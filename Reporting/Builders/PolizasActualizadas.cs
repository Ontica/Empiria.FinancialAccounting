/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : PolizasActualizadas                            License   : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Polizas actualizadas o pendientes de actualizar para reporte operativo.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.Reporting.Builders {

  /// <summary>Polizas actualizadas o pendientes de actualizar para reporte operativo.</summary>
  internal class PolizasActualizadas : IReportBuilder {

    #region Public methods

    public ReportDataDto Build(BuildReportCommand command) {
      Assertion.AssertObject(command, "command");

      PolizasCommand polizasCommand = GetPolizasCommand(command);


      
      return MapToReportDataDto(command);
    }

    #endregion

    #region Private methods


    static private FixedList<DataTableColumn> GetReportColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("numeroPoliza", "Numero", "text"));
      columns.Add(new DataTableColumn("afectacion", "Afectacion", "date"));
      columns.Add(new DataTableColumn("elaboracion", "Elaboracion", "date"));
      columns.Add(new DataTableColumn("elaboradoPor", "Elaborado por", "text"));
      columns.Add(new DataTableColumn("concepto", "Concepto", "text"));
      columns.Add(new DataTableColumn("debe", "Cargos", "decimal"));
      columns.Add(new DataTableColumn("haber", "Abonos", "decimal"));

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


    static private ReportDataDto MapToReportDataDto(BuildReportCommand command) {
      return new ReportDataDto {
        Command = command,
        Columns = GetReportColumns(),
        Entries = MapToReportDataEntries()
      };
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries() {

      var mappedItems = new FixedList<IReportEntryDto>();

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    #endregion Private methods

  } // PolizasActualizadas

  public class PolizaEntry : IReportEntryDto {

    public string Cuenta {
      get; internal set;
    }

    public decimal SaldoInicial {
      get; internal set;
    }


    public decimal Debe {
      get; internal set;
    }


    public decimal Haber {
      get; internal set;
    }


    public decimal SaldoFinal {
      get; internal set;
    }

  }  // class BalanzaSatEntry

} // namespace Empiria.FinancialAccounting.Reporting.Builders
