/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : SubledgerUseCases                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for subledgers and subledger accounts management.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for subledgers and subledger accounts management.</summary>
  public class SubledgerUseCases : UseCase {

    #region Constructors and parsers

    protected SubledgerUseCases() {
      // no-op
    }

    static public SubledgerUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<SubledgerUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public SubledgerAccountDto CreateSubledgerAccount(string subledgerUID,
                                                      SubledgerAccountFields fields) {
      Assertion.AssertObject(subledgerUID, "subledgerUID");
      Assertion.AssertObject(fields, "fields");

      var subledger = Subledger.Parse(subledgerUID);

      SubledgerAccount subledgerAccount = subledger.CreateAccount(fields);

      subledgerAccount.Save();

      return SubledgerMapper.MapAccount(subledgerAccount);
    }


    public SubledgerDto GetSubledger(string subledgerUID) {
      Assertion.AssertObject(subledgerUID, "subledgerUID");

      var subledger = Subledger.Parse(subledgerUID);

      return SubledgerMapper.Map(subledger);
    }


    public SubledgerAccountDto GetSubledgerAccount(string subledgerUID,
                                                   int subledgerAccountId) {
      Assertion.AssertObject(subledgerUID, "subledgerUID");
      Assertion.Assert(subledgerAccountId > 0, "subledgerAccountId");

      var subledger = Subledger.Parse(subledgerUID);

      SubledgerAccount subledgerAccount = subledger.GetAccountWithId(subledgerAccountId);

      return SubledgerMapper.MapAccount(subledgerAccount);
    }


    public FixedList<SubledgerAccountDto> SearchSubledgerAccounts(string accountsChartUID,
                                                                  SearchSubledgerAccountCommand command) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");
      Assertion.AssertObject(command, "command");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      FixedList<SubledgerAccount> subledgerAccounts = SubledgerAccount.GetList(accountsChart,
                                                                               command.Keywords);

      return SubledgerMapper.Map(subledgerAccounts);
    }

    #endregion Use cases

  }  // class SubledgerUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
