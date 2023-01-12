/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Providers                               *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : AccountBalancesProvider                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides accounts balances for their use in financial reports.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

using Empiria.FinancialAccounting.FinancialConcepts;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports.Providers {

  /// <summary>Provides accounts balances for their use in financial reports.</summary>
  internal class AccountBalancesProvider {

    private readonly FinancialReportQuery _buildQuery;
    private readonly FinancialReportType _financialReportType;

    private readonly ExchangeRatesProvider _exchangeRatesProvider;

    private readonly EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> _balances;

    #region Constructors and parsers

    internal AccountBalancesProvider(FinancialReportQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      _buildQuery = buildQuery;
      _financialReportType = _buildQuery.GetFinancialReportType();
      _balances = GetBalancesAsHashTable();
      _exchangeRatesProvider = new ExchangeRatesProvider(buildQuery.ToDate);
    }

    #endregion Constructors and parsers

    #region Public methods

    public bool ContainsAccount(string accountNumber) {
      return _balances.ContainsKey(accountNumber);
    }


    public FixedList<ITrialBalanceEntryDto> GetAccountBalances(FinancialConceptEntry integrationEntry) {
      FixedList<ITrialBalanceEntryDto> balances = _balances[integrationEntry.AccountNumber];

      FixedList<ITrialBalanceEntryDto> filtered;

      if (integrationEntry.HasSector && integrationEntry.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == integrationEntry.SectorCode &&
                                         x.SubledgerAccountNumber == integrationEntry.SubledgerAccountNumber);

      } else if (integrationEntry.HasSector && !integrationEntry.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == integrationEntry.SectorCode &&
                                         x.SubledgerAccountNumber.Length <= 1);

      } else if (!integrationEntry.HasSector && integrationEntry.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == "00" &&
                                         x.SubledgerAccountNumber == integrationEntry.SubledgerAccountNumber);
        if (filtered.Count == 0) {
          filtered = balances.FindAll(x => x.SectorCode != "00" &&
                                           x.SubledgerAccountNumber == integrationEntry.SubledgerAccountNumber);
        }
      } else {
        filtered = balances.FindAll(x => x.SectorCode == "00" &&
                                         x.SubledgerAccountNumber.Length <= 1);
        if (filtered.Count == 0) {
          filtered = balances.FindAll(x => x.SectorCode != "00" &&
                                           x.SubledgerAccountNumber.Length <= 1);
        }
      }

      return ConvertToDynamicTrialBalanceEntryDto(filtered);
    }


    private FixedList<ITrialBalanceEntryDto> ConvertToDynamicTrialBalanceEntryDto(FixedList<ITrialBalanceEntryDto> sourceEntries) {

      var converter = new DynamicTrialBalanceEntryConverter(_financialReportType,
                                                            _exchangeRatesProvider);

      FixedList<DynamicTrialBalanceEntry> convertedEntries = converter.Convert(sourceEntries);

      return convertedEntries.Select(entry => (ITrialBalanceEntryDto) entry)
                             .ToFixedList();
    }

    #endregion Public methods

    #region Helper methods

    private TrialBalanceQuery DetermineTrialBalanceQuery() {

      switch (_financialReportType.DataSource) {

        case FinancialReportDataSource.AnaliticoCuentas:
          return GetAnaliticoCuentasQuery();

        case FinancialReportDataSource.BalanzaEnColumnasPorMoneda:
          return GetBalanzaEnColumnasPorMonedaQuery();

        case FinancialReportDataSource.BalanzaTradicional:
          return GetBalanzaTradicionalQuery();

        default:
          throw Assertion.EnsureNoReachThisCode(
              $"Unrecognized balances source {_financialReportType.DataSource} for report type {_financialReportType.Name}.");
      }
    }


    private TrialBalanceDto InvokeGetBalancesUseCase() {
      TrialBalanceQuery query = DetermineTrialBalanceQuery();

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        return usecases.BuildTrialBalance(query);
      }
    }


    private EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> GetBalancesAsHashTable() {
      var balances = InvokeGetBalancesUseCase();

      var converted = new
                FixedList<ITrialBalanceEntryDto>(balances.Entries.FindAll(x =>
                                                 x.ItemType == TrialBalanceItemType.Entry ||
                                                 x.ItemType == TrialBalanceItemType.Summary));


      var distinctAccountNumbersList = converted.Select(x => x.AccountNumber)
                                                .Distinct()
                                                .ToList();

      var hashTable = new EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>>(distinctAccountNumbersList.Count);

      foreach (string accountNumber in distinctAccountNumbersList) {
        hashTable.Insert(accountNumber, converted.FindAll(x => x.AccountNumber == accountNumber));
      }

      return hashTable;
    }

    private TrialBalanceQuery GetAnaliticoCuentasQuery() {
      return new TrialBalanceQuery {
        AccountsChartUID = _buildQuery.AccountsChartUID,
        TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas,
        UseDefaultValuation = true,
        ShowCascadeBalances = false,
        WithSubledgerAccount = false,  // true
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        ConsolidateBalancesToTargetCurrency = false,
        InitialPeriod = new BalancesPeriod {
          FromDate = new DateTime(_buildQuery.ToDate.Year, _buildQuery.ToDate.Month, 1),
          ToDate = _buildQuery.ToDate,
          UseDefaultValuation = true
        }
      };
    }


    private TrialBalanceQuery GetBalanzaEnColumnasPorMonedaQuery() {
      return new TrialBalanceQuery {
        AccountsChartUID = _buildQuery.AccountsChartUID,
        TrialBalanceType = TrialBalanceType.BalanzaEnColumnasPorMoneda,
        UseDefaultValuation = true,
        ShowCascadeBalances = false,
        WithSubledgerAccount = false,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        ConsolidateBalancesToTargetCurrency = false,
        InitialPeriod = new BalancesPeriod {
          FromDate = new DateTime(_buildQuery.ToDate.Year, _buildQuery.ToDate.Month, 1),
          ToDate = _buildQuery.ToDate,
          UseDefaultValuation = true
        }
      };
    }


    private TrialBalanceQuery GetBalanzaTradicionalQuery() {
      return new TrialBalanceQuery {
        AccountsChartUID = _buildQuery.AccountsChartUID,
        TrialBalanceType = TrialBalanceType.Balanza,
        ShowCascadeBalances = false,
        WithSubledgerAccount = false,
        UseDefaultValuation = false,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        ConsolidateBalancesToTargetCurrency = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = new DateTime(_buildQuery.ToDate.Year, _buildQuery.ToDate.Month, 1),
          ToDate = _buildQuery.ToDate,
          ExchangeRateDate = _buildQuery.ToDate,
          ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID,
          ValuateToCurrrencyUID = Currency.MXN.UID,
          UseDefaultValuation = false
        }
      };
    }


    #endregion Helper methods

  } // class AccountBalancesProvider

} // namespace Empiria.FinancialAccounting.FinancialReports.Providers

