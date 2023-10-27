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

    public FixedList<ConciliacionDerivadosListItem> GetItems() {
      return AccountsListData.GetAccounts<ConciliacionDerivadosListItem>(this);
    }

    public FixedList<ConciliacionDerivadosListItem> GetItems(string keywords) {
      return AccountsListData.GetAccounts<ConciliacionDerivadosListItem>(this, keywords);
    }

    internal ConciliacionDerivadosListItem AddItem(ConciliacionDerivadosListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      var items = GetItems();

      if (items.Contains(x => x.Account.Number == fields.AccountNumber)) {
        Assertion.RequireFail($"La lista ya contiene la cuenta {fields.AccountNumber}.");
      }

      return new ConciliacionDerivadosListItem(this, fields);
    }

    internal ConciliacionDerivadosListItem RemoveItem(ConciliacionDerivadosListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      var items = GetItems();

      var itemToDelete = items.Find(x => x.UID == fields.UID &&
                                         x.Account.Number == fields.AccountNumber &&
                                         x.StartDate == fields.StartDate &&
                                         x.EndDate == fields.EndDate);

      if (itemToDelete == null) {
        Assertion.RequireFail($"La lista no contiene la cuenta {fields.AccountNumber}.");
      } else {
        itemToDelete.Delete();
      }

      return itemToDelete;
    }

    internal ConciliacionDerivadosListItem UpdateItem(ConciliacionDerivadosListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      var items = GetItems();

      if (items.Contains(x => x.Account.Number == fields.AccountNumber && x.UID != fields.UID)) {
        Assertion.RequireFail($"La lista ya contiene la cuenta {fields.AccountNumber}.");
      }

      var itemToUpdate = items.Find(x => x.UID == fields.UID);

      if (itemToUpdate == null) {
        Assertion.RequireFail($"La lista no contiene un elemento con el identificador {fields.UID}.");
      } else {
        itemToUpdate.Update(fields);
      }

      return itemToUpdate;
    }

  }  // class ConciliacionDerivadosList

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
