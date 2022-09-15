/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Helper methods                          *
*  Type     : AccountStatementUseCases                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to build vouchers by account.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Reporting.AccountStatements.Adapters;
using Empiria.FinancialAccounting.Reporting.AccountStatements.Domain;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements {

  /// <summary>Use cases used to build vouchers by account.</summary>
  public class AccountStatementUseCases : UseCase {

    #region Constructors and parsers

    protected AccountStatementUseCases() {
      // no-op
    }

    static public AccountStatementUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<AccountStatementUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public AccountStatementDto BuildAccountStatement(AccountStatementQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      var builder = new AccountStatementBuilder(buildQuery);

      AccountStatement accountStatement = builder.Build();

      return AccountStatementMapper.Map(accountStatement);
    }

    #endregion Use cases

  } // class AccountStatementUseCases

} // namespace Empiria.FinancialAccounting.Reporting.UseCases
