/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : LockedUpBalancesService                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Main service to get balances information to create locked up balances report.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.Reporting.LockedUpBalances.Adapters;
using Empiria.FinancialAccounting.UseCases;
using Empiria.Services;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Main service to get balances information to create locked up balances report.</summary>
  public class LockedUpBalancesService : Service {


    #region Constructors and parsers

    private LockedUpBalancesService() {
      // no-op
    }

    static public LockedUpBalancesService ServiceInteractor() {
      return Service.CreateInstance<LockedUpBalancesService>();
    }

    #endregion Constructors and parsers


    #region Public methods


    public LockedUpBalancesDto GenerateReport(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      using (var accountsUsecases = AccountsChartUseCases.UseCaseInteractor()) {

        AccountsChartDto accountsChart = accountsUsecases.TryGetAccountsWithChange(
                buildQuery.AccountsChartUID, buildQuery.FromDate, buildQuery.ToDate);

        List<BalanzaTradicionalEntryDto> entries = BalancesByAccount(buildQuery, accountsChart.Accounts);

        return Map(entries, accountsChart.Accounts);
      }
    }


    private List<BalanzaTradicionalEntryDto> BalancesByAccount(
                ReportBuilderQuery buildQuery, FixedList<AccountDescriptorDto> accounts) {

      var returnedEntries = new List<BalanzaTradicionalEntryDto>();
      foreach (var account in accounts) {

        List<BalanzaTradicionalEntryDto> entries = GetBalancesByAccount(buildQuery, account);
        returnedEntries.AddRange(entries);

      }

      return returnedEntries;
    }


    #endregion Public methods

    #region Private methods


    private List<BalanzaTradicionalEntryDto> GetBalancesByAccount(
                ReportBuilderQuery buildQuery, AccountDescriptorDto account) {

      using (var balancesUsecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceQuery _query = this.MapToTrialBalanceQuery(buildQuery, account);

        TrialBalanceDto trialBalance = balancesUsecases.BuildTrialBalance(_query);

        return GetParentEntries(trialBalance.Entries, account.Number);

      }

    }


    private List<BalanzaTradicionalEntryDto> GetParentEntries(
            FixedList<ITrialBalanceEntryDto> entries, string number) {

      if (entries.Count == 0) {
        return new List<BalanzaTradicionalEntryDto>();
      }

      var balanceEntries = entries.Select(x => (BalanzaTradicionalEntryDto) x);

      return balanceEntries.Where(a => a.AccountNumberForBalances == number).ToList();
    }


    private LockedUpBalancesDto Map(List<BalanzaTradicionalEntryDto> entries,
                                    FixedList<AccountDescriptorDto> accounts) {

      return new LockedUpBalancesDto {
        Columns = MapColumns(),
        Entries = MapToDto(entries, accounts)
      };

    }


    private FixedList<DataTableColumn> MapColumns() {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("currencyCode", "Moneda", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text-nowrap"));
      columns.Add(new DataTableColumn("SectorCode", "Sector", "text"));
      columns.Add(new DataTableColumn("subledgerAccount", "Auxiliar", "text"));
      columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
      columns.Add(new DataTableColumn("SectorCode", "Sector", "text"));
      columns.Add(new DataTableColumn("role", "Rol", "text"));
      columns.Add(new DataTableColumn("newRole", "Nuevo Rol", "text"));
      columns.Add(new DataTableColumn("roleChangeDate", "Fecha cambio de Rol", "date"));
      columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));


      return columns.ToFixedList();
    }


    static private FixedList<LockedUpBalancesEntryDto> MapToDto(
                   List<BalanzaTradicionalEntryDto> entries, FixedList<AccountDescriptorDto> accounts) {

      var mapped = entries.Select(x => MapToLockedUpEntry(x, accounts));

      return new FixedList<LockedUpBalancesEntryDto>(mapped);
    }


    static private LockedUpBalancesEntryDto MapToLockedUpEntry(
                   BalanzaTradicionalEntryDto entry, FixedList<AccountDescriptorDto> accounts) {

      var account = accounts.Find(a => a.Number == entry.AccountNumberForBalances);

      var dto = new LockedUpBalancesEntryDto();

      dto.StandardAccountId = entry.StandardAccountId;
      dto.CurrencyCode = entry.CurrencyCode;
      dto.AccountNumber = entry.AccountNumber;
      dto.AccountName = entry.AccountName;
      dto.SectorCode = entry.SectorCode;
      dto.SubledgerAccount = entry.SubledgerAccountNumber.Length > 1 ?
                             entry.SubledgerAccountNumber : "";
      dto.CurrentBalance = (decimal) entry.CurrentBalance;
      dto.LastChangeDate = entry.LastChangeDate;
      dto.RoleChangeDate = account.EndDate;
      dto.Role = account.Role.ToString();
      dto.NewRole = entry.AccountRole.ToString();

      return dto;
    }


    private TrialBalanceQuery MapToTrialBalanceQuery(ReportBuilderQuery buildQuery,
                                                     AccountDescriptorDto account) {

      return new TrialBalanceQuery {
        AccountsChartUID = buildQuery.AccountsChartUID,
        TrialBalanceType = TrialBalanceType.Balanza,
        BalancesType = BalancesType.AllAccounts,
        InitialPeriod = {
         FromDate = buildQuery.FromDate,
         ToDate = buildQuery.ToDate
        },

        FromAccount = account.Number,
        ToAccount = account.Number,
        IsOperationalReport = true,
        UseDefaultValuation = false,
        ShowCascadeBalances = true,
        WithSubledgerAccount = account.Role == AccountRole.Control ? true : false
      };

    }

    #endregion Private methods

  } // class LockedUpBalancesService

} // namespace Empiria.FinancialAccounting.Reporting
