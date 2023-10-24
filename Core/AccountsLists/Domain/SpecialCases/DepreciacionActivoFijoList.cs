/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Use cases Layer                         *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Use case interactor class               *
*  Type     : ConciliacionDerivadosList                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Lista de Swaps de cobertura.                                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.AccountsLists.Adapters;
using Empiria.FinancialAccounting.AccountsLists.Data;

namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases {

  public class DepreciacionActivoFijoList : AccountsList {

    static internal DepreciacionActivoFijoList Parse() {
      return (DepreciacionActivoFijoList) AccountsList.Parse("DepreciacionActivoFijo");
    }


    public FixedList<DepreciacionActivoFijoListItem> GetItems(string keywords) {
      return AccountsListData.GetAccounts<DepreciacionActivoFijoListItem>(this, keywords);
    }


    internal DepreciacionActivoFijoListItem AddItem(DepreciacionActivoFijoListItemFields fields) {
      throw new NotImplementedException();
    }


    internal DepreciacionActivoFijoListItem RemoveItem(DepreciacionActivoFijoListItemFields fields) {
      throw new NotImplementedException();
    }


    internal DepreciacionActivoFijoListItem UpdateItem(DepreciacionActivoFijoListItemFields fields) {
      throw new NotImplementedException();
    }

  }  // class ConciliacionDerivadosList

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
