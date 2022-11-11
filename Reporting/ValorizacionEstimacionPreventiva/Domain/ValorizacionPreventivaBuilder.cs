/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : ValorizacionEstimacionPreventivaBuilder    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de valorizacion.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.Reporting.ValorizacionEstimacionPreventiva.Adaptars;

namespace Empiria.FinancialAccounting.Reporting.ValorizacionEstimacionPreventiva.Domain {

  /// <summary>Genera los datos para el reporte de valorizacion.</summary>
  internal class ValorizacionPreventivaBuilder : IReportBuilder {

    private TrialBalanceQuery _query;

    #region Public methods


    public ReportDataDto Build(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        
        _query = this.MapToTrialBalanceQuery(buildQuery);

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(_query);

        var entries = trialBalance.Entries.Select(a => (ValorizacionEntryDto) a);

        return ValorizacionPreventivaMapper.Map(buildQuery, trialBalance);
      }
    }



    #endregion Public methods


    #region Private methods



    private TrialBalanceQuery MapToTrialBalanceQuery(ReportBuilderQuery buildQuery) {
      return new TrialBalanceQuery {
        AccountsChartUID = buildQuery.AccountsChartUID,
        BalancesType = BalancesType.AllAccounts,
        InitialPeriod = {
         FromDate = buildQuery.FromDate,
         ToDate = buildQuery.ToDate
        },
        ShowCascadeBalances = false,
        TrialBalanceType = TrialBalanceType.ValorizacionEstimacionPreventiva,
        UseDefaultValuation = true,
        IsOperationalReport = true,
        FromAccount ="3.02.01",
        ToAccount = "3.02.01"
      };
    }

    #endregion Private methods

  } // class ValorizacionEstimacionPreventivaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
