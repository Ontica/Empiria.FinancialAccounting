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

  public class ConciliacionDerivadosList : AccountsList {

    static internal ConciliacionDerivadosList Parse() {
      return BaseObject.ParseKey<ConciliacionDerivadosList>("ConciliacionDerivados");
    }

    public FixedList<ConciliacionDerivadosListItem> GetItems(string keywords) {
      return AccountsListData.GetAccounts<ConciliacionDerivadosListItem>(this, keywords);
    }

    internal ConciliacionDerivadosListItem AddItem(ConciliacionDerivadosListItemFields fields) {
      throw new NotImplementedException();
    }

    internal ConciliacionDerivadosListItem RemoveItem(ConciliacionDerivadosListItemFields fields) {
      throw new NotImplementedException();
    }

    internal ConciliacionDerivadosListItem UpdateItem(ConciliacionDerivadosListItemFields fields) {
      throw new NotImplementedException();
    }

  }  // class ConciliacionDerivadosList

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
