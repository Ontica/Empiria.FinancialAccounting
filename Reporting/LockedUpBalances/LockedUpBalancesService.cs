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

        var balancesUsecases = TrialBalanceUseCases.UseCaseInteractor();

        TrialBalanceQuery _query = this.MapToTrialBalanceQuery(buildQuery);

        TrialBalanceDto trialBalance = balancesUsecases.BuildTrialBalance(_query);

        var entries = GetParentEntries(trialBalance.Entries);

        return Map(entries);
      }
    }


    #endregion Public methods

    #region Private methods


    private List<BalanzaTradicionalEntryDto> GetParentEntries(FixedList<ITrialBalanceEntryDto> entries) {

      var balanceEntries = entries.Select(x => (BalanzaTradicionalEntryDto) x);

      return balanceEntries.Where(a => a.IsParentPostingEntry).ToList();
    }


    private LockedUpBalancesDto Map(List<BalanzaTradicionalEntryDto> entries) {

      return new LockedUpBalancesDto {
        Columns = MapColumns(),
        Entries = MapToDto(entries)
      };

    }


    private FixedList<DataTableColumn> MapColumns() {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("currencyCode", "Moneda", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text-nowrap"));
      columns.Add(new DataTableColumn("SectorCode", "Sector", "text"));
      columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
      columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));


      return columns.ToFixedList();
    }


    static private FixedList<LockedUpBalancesEntryDto> MapToDto(List<BalanzaTradicionalEntryDto> entries) {

      var mapped = entries.Select(x => MapToLockedUpEntry(x));

      return new FixedList<LockedUpBalancesEntryDto>(mapped);
    }


    static private LockedUpBalancesEntryDto MapToLockedUpEntry(BalanzaTradicionalEntryDto entry) {
      var dto = new LockedUpBalancesEntryDto();

      dto.StandardAccountId = entry.StandardAccountId;
      dto.CurrencyCode = entry.CurrencyCode;
      dto.AccountNumber = entry.AccountNumber;
      dto.AccountName = entry.AccountName;
      dto.SectorCode = entry.SectorCode;
      dto.CurrentBalance = (decimal) entry.CurrentBalance;
      dto.LastChangeDate = entry.LastChangeDate;

      return dto;
    }


    private TrialBalanceQuery MapToTrialBalanceQuery(ReportBuilderQuery buildQuery) {

      return new TrialBalanceQuery {
        AccountsChartUID = buildQuery.AccountsChartUID,
        BalancesType = BalancesType.AllAccounts,
        InitialPeriod = {
         FromDate = buildQuery.FromDate,
         ToDate = buildQuery.ToDate
        },
        ShowCascadeBalances = false,
        TrialBalanceType = TrialBalanceType.Balanza,
        UseDefaultValuation = false,
        IsOperationalReport = true,
        FromAccount = "4",
        ToAccount = "4"
      };

    }

    #endregion Private methods

  } // class LockedUpBalancesService

} // namespace Empiria.FinancialAccounting.Reporting
