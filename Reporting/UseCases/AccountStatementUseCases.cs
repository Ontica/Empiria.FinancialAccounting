/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Helper methods                          *
*  Type     : VouchersByAccountUseCases                       License   : Please read LICENSE.txt file       *
*                                                                                                            *
*  Summary  : Use cases used to build vouchers by account.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.Services;

namespace Empiria.FinancialAccounting.Reporting.UseCases {

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


    public AccountStatementDto BuilAccountStatement(AccountStatementCommand accountStatementCommand) {

      Assertion.AssertObject(accountStatementCommand, "accountStatementCommand");

      var vouchersConstructor = new AccountStatementConstructor(accountStatementCommand);

      AccountStatement vouchers = vouchersConstructor.Build();

      return AccountStatementMapper.Map(vouchers);
    }


    #endregion

  } // class VouchersByAccountUseCases

} // namespace Empiria.FinancialAccounting.Reporting.UseCases
