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

      columns.Add(new DataTableColumn("CodAgrup", "CodAgrup", "text"));
      columns.Add(new DataTableColumn("NumCta", "NumCta", "text"));
      columns.Add(new DataTableColumn("Desc", "Desc", "text"));
      columns.Add(new DataTableColumn("SubCtaDe", "SubCtaDe", "text"));
      columns.Add(new DataTableColumn("Nivel", "Nivel", "decimal", 0));
      columns.Add(new DataTableColumn("Natur", "Natur", "text"));

      return columns.ToFixedList();
    }


    static private ReportDataDto MapToReportDataDto(BuildReportCommand command,
                                                    FixedList<AccountDescriptorDto> accounts) {
      return new ReportDataDto {
        Command = command,
        Columns = GetReportColumns(),
        Entries = MapToReportDataEntries(accounts)
      };
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries(FixedList<AccountDescriptorDto> list) {
      var mappedItems = list.Select((x) => MapAccountsToOperationalReport(x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private IReportEntryDto MapAccountsToOperationalReport(AccountDescriptorDto account) {
      return new CatalogoCuentasSatEntry {
        CodigoAgrupacion = "000",
        NumeroCuenta = account.Number,
        Descripcion = account.Name,
        SubcuentaDe = account.Level > 1 ? account.Parent : account.Number,
        Nivel = account.Level,
        Naturaleza = account.DebtorCreditor == DebtorCreditorType.Deudora ? "D" : "A"
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


  }  // class CatalogoCuentasSatEntry


}  // namespace Empiria.FinancialAccounting.Reporting.Builders
