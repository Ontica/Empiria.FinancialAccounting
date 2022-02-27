/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Use cases Layer                         *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Use case interactor class               *
*  Type     : AccountsListsUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for accounts lists.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for accounts lists.</summary>
  public class AccountsListsUseCases : UseCase {

    #region Constructors and parsers

    protected AccountsListsUseCases() {
      // no-op
    }


    static public AccountsListsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<AccountsListsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<NamedEntityDto> GetAccountsLists() {
      FixedList<AccountsList> list = AccountsList.GetList();

      return list.MapToNamedEntityList();
    }

    #endregion Use cases

  } // class AccountsListsUseCases

} // Empiria.FinancialAccounting.UseCases
