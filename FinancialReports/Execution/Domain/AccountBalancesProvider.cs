/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : AccountBalancesProvider                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides accounts balances for their use in financial reports.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Provides accounts balances for their use in financial reports.</summary>
  internal class AccountBalancesProvider {

    private readonly FinancialReportQuery _buildQuery;

    #region Constructors and parsers

    internal AccountBalancesProvider(FinancialReportQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      _buildQuery = buildQuery;
    }

    #endregion Constructors and parsers

    #region Public methods

    internal EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> GetBalancesAsHashTable() {
      var balances = GetBalances();

      var converted = new
                FixedList<ITrialBalanceEntryDto>(balances.Entries.FindAll(x =>
                                                 x.ItemType == BalanceEngine.TrialBalanceItemType.Entry ||
                                                 x.ItemType == BalanceEngine.TrialBalanceItemType.Summary));


      var distinctAccountNumbersList = converted.Select(x => x.AccountNumber)
                                                .Distinct()
                                                .ToList();

      var hashTable = new EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>>(distinctAccountNumbersList.Count);

      foreach (string accountNumber in distinctAccountNumbersList) {
        hashTable.Insert(accountNumber, converted.FindAll(x => x.AccountNumber == accountNumber));
      }

      return hashTable;
    }

    #endregion Public methods

    #region Helper methods

    private TrialBalanceQuery DetermineTrialBalanceQuery() {
      FinancialReportType reportType = _buildQuery.GetFinancialReportType();

      switch (reportType.DataSource) {

        case FinancialReportDataSource.AnaliticoCuentas:
          return GetAnaliticoCuentasQuery();

        case FinancialReportDataSource.BalanzaEnColumnasPorMoneda:
          return GetBalanzaEnColumnasPorMonedaQuery();

        case FinancialReportDataSource.BalanzaTradicional:
          return GetBalanzaTradicionalQuery();

        default:
          throw Assertion.EnsureNoReachThisCode(
              $"Unrecognized balances source {reportType.DataSource} for report type {reportType.Name}.");
      }
    }


    private TrialBalanceDto GetBalances() {
      TrialBalanceQuery query = DetermineTrialBalanceQuery();

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        return usecases.BuildTrialBalance(query);
      }
    }


    private TrialBalanceQuery GetAnaliticoCuentasQuery() {
      return new TrialBalanceQuery {
        AccountsChartUID = _buildQuery.AccountsChartUID,
        TrialBalanceType = BalanceEngine.TrialBalanceType.AnaliticoDeCuentas,
        UseDefaultValuation = true,
        ShowCascadeBalances = false,
        WithSubledgerAccount = false,  // true
        BalancesType = BalanceEngine.BalancesType.WithCurrentBalanceOrMovements,
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
        TrialBalanceType = BalanceEngine.TrialBalanceType.BalanzaEnColumnasPorMoneda,
        UseDefaultValuation = true,
        ShowCascadeBalances = false,
        WithSubledgerAccount = false,
        BalancesType = BalanceEngine.BalancesType.WithCurrentBalanceOrMovements,
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
        TrialBalanceType = BalanceEngine.TrialBalanceType.Balanza,
        ShowCascadeBalances = false,
        WithSubledgerAccount = false,
        UseDefaultValuation = false,
        BalancesType = BalanceEngine.BalancesType.WithCurrentBalanceOrMovements,
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

} // namespace Empiria.FinancialAccounting.FinancialReports
