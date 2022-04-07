/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : CatalogoCuentasSat                            License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Catálolgo de cuentas para la contabilidad electrónica del SAT.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.Reporting.Builders {

  /// <summary>Catálolgo de cuentas para la contabilidad electrónica del SAT.</summary>
  internal class CatalogoCuentasSat : IReportBuilder {

    #region Public methods

    public ReportDataDto Build(BuildReportCommand command) {
      Assertion.AssertObject(command, "command");

      AccountsSearchCommand searchCommand = GetAccountsSearchCommand(command);

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountsChartDto accountsChart = usecases.SearchAccounts(command.AccountsChartUID, searchCommand);

        return MapToReportDataDto(command, accountsChart.Accounts);
      }
    }


    #endregion Public methods


    #region Private methods

    static private AccountsSearchCommand GetAccountsSearchCommand(BuildReportCommand command) {
      return new AccountsSearchCommand {
        Date = command.ToDate
      };
    }


    static private FixedList<DataTableColumn> GetReportColumns() {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("codigoAgrupacion", "Código agrupación", "text-nowrap"));
      columns.Add(new DataTableColumn("numeroCuenta", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("descripcion", "Descripción", "text"));
      columns.Add(new DataTableColumn("subcuentaDe", "Subcuenta de", "text"));
      columns.Add(new DataTableColumn("nivel", "Nivel", "decimal", 0));
      columns.Add(new DataTableColumn("naturaleza", "Naturaleza", "text"));
      columns.Add(new DataTableColumn("fechaModificacion", "Última modificación", "date"));
      columns.Add(new DataTableColumn("baja", "Baja", "text"));

      return columns.ToFixedList();
    }


    static private ReportDataDto MapToReportDataDto(BuildReportCommand command,
                                                    FixedList<AccountDescriptorDto> accounts) {
      return new ReportDataDto {
        Command = command,
        Columns = GetReportColumns(),
        Entries = MapToReportDataEntries(accounts, command.ToDate)
      };
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries(FixedList<AccountDescriptorDto> list,
                                                                     DateTime date) {
      var mappedItems = list.FindAll(x => x.StartDate <= date && date <= x.EndDate)
                            .Select((x) => MapAccountsToOperationalReport(date, x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private IReportEntryDto MapAccountsToOperationalReport(DateTime date,
                                                                  AccountDescriptorDto account) {
      DateTime lastUpdate = date < account.EndDate ? account.StartDate : account.EndDate;

      return new CatalogoCuentasSatEntry {
        CodigoAgrupacion = "000",
        NumeroCuenta = account.Number,
        Descripcion = account.Name,
        SubcuentaDe = account.Level > 1 ? account.Parent : account.Number,
        Nivel = account.Level,
        Naturaleza = account.DebtorCreditor == DebtorCreditorType.Deudora ? "D" : "A",
        FechaModificacion = lastUpdate,
        Baja = account.SummaryWithNotChildren ? "Sí" : ""
      };
    }

    #endregion Private methods

  }  // class CatalogoCuentasSat


  public class CatalogoCuentasSatEntry : IReportEntryDto {


    public string CodigoAgrupacion {
      get; internal set;
    }


    public string NumeroCuenta {
      get; internal set;
    }


    public string Descripcion {
      get; internal set;
    }


    public string SubcuentaDe {
      get; internal set;
    }


    public int Nivel {
      get; internal set;
    }


    public string Naturaleza {
      get; internal set;
    }


    public DateTime FechaModificacion {
      get; internal set;
    }


    public string Baja {
      get; internal set;
    }

  }  // class CatalogoCuentasSatEntry


}  // namespace Empiria.FinancialAccounting.Reporting.Builders
