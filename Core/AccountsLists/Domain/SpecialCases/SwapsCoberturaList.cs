/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Domain Layer                            *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : List class                              *
*  Type     : SwapsCoberturaList                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Lista de Swaps de cobertura.                                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.AccountsLists.Adapters;
using Empiria.FinancialAccounting.AccountsLists.Data;

namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases {

  /// <summary>Lista de Swaps de cobertura.</summary>
  public class SwapsCoberturaList : AccountsList {

    static internal SwapsCoberturaList Parse() {
      return (SwapsCoberturaList) AccountsList.Parse("SwapsCobertura");
    }

    public FixedList<SwapsCoberturaListItem> GetItems(string keywords) {
      return AccountsListData.GetAccounts<SwapsCoberturaListItem>(this, keywords);
    }

    internal SwapsCoberturaListItem AddItem(SwapsCoberturaListItemFields fields) {
      throw new NotImplementedException();
    }

    internal SwapsCoberturaListItem RemoveItem(SwapsCoberturaListItemFields fields) {
      throw new NotImplementedException();
    }

    internal SwapsCoberturaListItem UpdateItem(SwapsCoberturaListItemFields fields) {
      throw new NotImplementedException();
    }

  }  // class SwapsCoberturaList

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
