/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : AccountComparerBuilder                        License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides the services that is used to generate account comparer.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Reporting.AccountsComparer.Domain {

  /// <summary>Provides the services that is used to generate account comparer.</summary>
  internal class AccountComparerBuilder : IReportBuilder {

    #region Public methods


    public ReportDataDto Build(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        TrialBalanceQuery trialBalanceQuery = MapToTrialBalanceQuery(buildQuery);

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(trialBalanceQuery);

        return MapToReportDataDto(buildQuery, trialBalance);
      }
    }


    #endregion Public methods

    #region Private methods


    private ReportDataDto MapToReportDataDto(ReportBuilderQuery buildQuery, TrialBalanceDto trialBalance) {
      throw new NotImplementedException();
    }


    private TrialBalanceQuery MapToTrialBalanceQuery(ReportBuilderQuery buildQuery) {
      throw new NotImplementedException();
    }


    #endregion Private methods

  } // class AccountComparerBuilder

} // namespace Empiria.FinancialAccounting.Reporting.AccountsComparer.Domain
