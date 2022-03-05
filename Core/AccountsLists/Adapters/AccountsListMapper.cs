/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Interface adapters                      *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Mapper class                            *
*  Type     : AccountsListMapper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map accounts lists.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Methods used to map accounts lists.</summary>
  static internal class AccountsListMapper {

    #region Public mappers

    static internal AccountsListDto Map(AccountsList list) {
      FixedList<AccountsListItem> items = list.GetItems();

      return new AccountsListDto {
        UID = list.UID,
        Name = list.Name,
        Items = Map(items)
      };
    }

    #endregion Public mappers

    #region Private methods

    static private FixedList<AccountsListItemDto> Map(FixedList<AccountsListItem> items) {
      var mapped = items.Select(x => MapAccountsListItem(x));

      return new FixedList<AccountsListItemDto>(mapped);
    }


    static private AccountsListItemDto MapAccountsListItem(AccountsListItem item) {
      return new AccountsListItemDto {

      };
    }

    #endregion Private methods

  } // class AccountsListMapper

} // Empiria.FinancialAccounting.Adapters
