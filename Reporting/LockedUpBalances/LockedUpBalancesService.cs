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
using Empiria.Collections;
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

        FixedList<LockedUpBalancesEntryDto> mappedEntries = MergeBalancesIntoLockedUpBalanceEntries(
                                                            entries, accountsChart.Accounts);

        FixedList<LockedUpBalancesEntryDto> headers = GetHeaderByLedger(mappedEntries);

        FixedList<LockedUpBalancesEntryDto> headersAndEntries = MergeHeadersAndEntries(headers, mappedEntries);

        return Map(headersAndEntries);

      }
    }


    #endregion Public methods

    #region Private methods

    static private void AccountClauses(LockedUpBalancesEntryDto dto,
                                       BalanzaTradicionalEntryDto entry,
                                       AccountDescriptorDto account) {
      if (entry.SubledgerAccountNumber.Length > 1) {

        dto.AccountNumber = entry.AccountNumberForBalances;
        dto.SubledgerAccount = entry.SubledgerAccountNumber;

      } else {
        dto.AccountNumber = entry.AccountNumber;
        dto.SubledgerAccount = "";

      }

      if (account.Role != AccountRole.Control) {
        dto.ItemType = TrialBalanceItemType.Summary;

      } else {
        dto.ItemType = entry.ItemType;
      }
    }


    private List<BalanzaTradicionalEntryDto> BalancesByAccount(
                ReportBuilderQuery buildQuery, FixedList<AccountDescriptorDto> accounts) {

      var returnedEntries = new List<BalanzaTradicionalEntryDto>();
      foreach (var account in accounts) {

        List<BalanzaTradicionalEntryDto> entries = GetBalancesByAccount(buildQuery, account);

        returnedEntries.AddRange(entries);
      }

      return returnedEntries.OrderBy(a => a.AccountNumberForBalances)
                     .ThenBy(a => a.CurrencyCode)
                     .ThenBy(a => a.SectorCode)
                     .ThenBy(a => a.SubledgerAccountNumber).ToList();
    }


    static private LockedUpBalancesEntryDto CreateGroupEntry(LockedUpBalancesEntryDto entry) {
      var group = new LockedUpBalancesEntryDto();
      group.LedgerNumber = entry.LedgerNumber;
      group.AccountName = $"({entry.LedgerNumber}) {entry.LedgerName}".ToUpper();
      group.ItemType = TrialBalanceItemType.Group;
      group.canGenerateVoucher = true;
      group.RoleChangeDate = entry.RoleChangeDate;
      group.LastChangeDate = ExecutionServer.DateMaxValue;
      return group;
    }


    private List<BalanzaTradicionalEntryDto> GetAccountEntries(
            FixedList<ITrialBalanceEntryDto> entries, AccountDescriptorDto account) {

      if (entries.Count == 0) {
        return new List<BalanzaTradicionalEntryDto>();
      }

      var balanceEntries = entries.Select(x => (BalanzaTradicionalEntryDto) x);

      if (account.Role != AccountRole.Control) {

        return balanceEntries.Where(a => a.AccountNumberForBalances == account.Number &&
                                         a.ItemType == TrialBalanceItemType.Entry).ToList();

      }

      return balanceEntries.Where(a => a.AccountNumberForBalances == account.Number).ToList();
    }


    private List<BalanzaTradicionalEntryDto> GetBalancesByAccount(
                ReportBuilderQuery buildQuery, AccountDescriptorDto account) {

      using (var balancesUsecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceQuery _query = this.MapToTrialBalanceQuery(buildQuery, account);

        TrialBalanceDto trialBalance = balancesUsecases.BuildTrialBalance(_query);

        return GetAccountEntries(trialBalance.Entries, account);

      }

    }


    static private FixedList<LockedUpBalancesEntryDto> GetHeaderByLedger(
                  FixedList<LockedUpBalancesEntryDto> mappedEntries) {

      var headers = new EmpiriaHashTable<LockedUpBalancesEntryDto>();

      foreach (var entry in mappedEntries) {

        string hash = $"{entry.LedgerNumber}||{entry.RoleChangeDate}";

        LockedUpBalancesEntryDto groupEntry;

        headers.TryGetValue(hash, out groupEntry);

        if (groupEntry == null) {
          groupEntry = CreateGroupEntry(entry);
          headers.Insert(hash, groupEntry);
        }
        
      }

      return headers.Values.OrderBy(a => a.LedgerNumber)
                           .ThenBy(a => a.RoleChangeDate).ToFixedList();
    }


    static private LockedUpBalancesDto Map(FixedList<LockedUpBalancesEntryDto> mappedEntries) {

      return new LockedUpBalancesDto {
        Columns = MapColumns(),
        Entries = mappedEntries
      };

    }


    static private FixedList<DataTableColumn> MapColumns() {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("sectorCode", "Sector", "text"));
      columns.Add(new DataTableColumn("subledgerAccount", "Auxiliar", "text"));
      columns.Add(new DataTableColumn("currentBalance", "Saldo encerrado", "decimal"));
      columns.Add(new DataTableColumn("roleChangeDate", "Fecha cambio Rol", "date"));
      columns.Add(new DataTableColumn("actionRole", "Rol", "text-button"));


      return columns.ToFixedList();
    }


    static private FixedList<LockedUpBalancesEntryDto> MergeBalancesIntoLockedUpBalanceEntries(
                   List<BalanzaTradicionalEntryDto> entries, FixedList<AccountDescriptorDto> accounts) {

      var mapped = entries.Select(x => MapToLockedUpEntry(x, accounts));

      return new FixedList<LockedUpBalancesEntryDto>(mapped);
    }


    static private LockedUpBalancesEntryDto MapToLockedUpEntry(
                   BalanzaTradicionalEntryDto entry, FixedList<AccountDescriptorDto> accounts) {

      var account = accounts.Find(a => a.Number == entry.AccountNumberForBalances);

      var dto = new LockedUpBalancesEntryDto();
      AccountClauses(dto, entry, account);
      dto.StandardAccountId = entry.StandardAccountId;
      dto.CurrencyCode = entry.CurrencyCode;
      dto.LedgerUID = entry.LedgerUID;
      dto.LedgerNumber = entry.LedgerNumber;
      dto.LedgerName = entry.LedgerName;
      dto.RoleChangeDate = account.EndDate;
      dto.ActionRole = $"{account.Role}-{entry.AccountRole}";
      dto.AccountName = entry.AccountName;
      dto.SectorCode = entry.SectorCode;
      dto.CurrentBalance = (decimal) entry.CurrentBalance;
      dto.LastChangeDate = entry.LastChangeDate;
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


    static private FixedList<LockedUpBalancesEntryDto> MergeHeadersAndEntries(
                            FixedList<LockedUpBalancesEntryDto> headers,
                            FixedList<LockedUpBalancesEntryDto> mappedEntries) {

      var mergedEntries = new List<LockedUpBalancesEntryDto>();

      foreach (var header in headers) {

        var entriesByHeader = mappedEntries.Where(a => a.RoleChangeDate == header.RoleChangeDate &&
                                                  a.LedgerNumber == header.LedgerNumber).ToList();

        mergedEntries.Add(header);
        mergedEntries.AddRange(entriesByHeader);
      }

      return mergedEntries.ToFixedList();
    }


    #endregion Private methods

  } // class LockedUpBalancesService

} // namespace Empiria.FinancialAccounting.Reporting
