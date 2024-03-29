﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : ValorizacionEstimacionPreventivaBuilder    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de valorizacion.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.Reporting.ValorizacionEstimacionPreventiva.Adapters;

namespace Empiria.FinancialAccounting.Reporting.ValorizacionEstimacionPreventiva.Domain {

  /// <summary>Genera los datos para el reporte de valorizacion.</summary>
  internal class ValorizacionPreventivaBuilder : IReportBuilder {

    #region Public methods

    public ReportDataDto Build(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceQuery _query = this.MapToTrialBalanceQuery(buildQuery);

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(_query);

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
        IsOperationalReport = true
      };
    }

    #endregion Private methods

  } // class ValorizacionEstimacionPreventivaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
