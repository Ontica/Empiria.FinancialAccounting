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

    static public DepreciacionActivoFijoList Parse() {
      return (DepreciacionActivoFijoList) AccountsList.Parse("DepreciacionActivoFijo");
    }

    public FixedList<DepreciacionActivoFijoListItem> GetItems() {
      return AccountsListData.GetAccounts<DepreciacionActivoFijoListItem>(this);
    }

    public FixedList<DepreciacionActivoFijoListItem> GetItems(string keywords) {
      return AccountsListData.GetAccounts<DepreciacionActivoFijoListItem>(this, keywords);
    }


    internal DepreciacionActivoFijoListItem AddItem(DepreciacionActivoFijoListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      var items = GetItems();

      if (items.Contains(x => x.AuxiliarHistorico.Number == fields.AuxiliarHistorico)) {
        Assertion.RequireFail($"La lista ya contiene el auxiliar histórico {fields.AuxiliarHistorico}.");
      }

      return new DepreciacionActivoFijoListItem(this, fields);
    }


    internal DepreciacionActivoFijoListItem RemoveItem(DepreciacionActivoFijoListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      var items = GetItems();

      var itemToDelete = items.Find(x => x.UID == fields.UID &&
                                         x.AuxiliarHistorico.Number == fields.AuxiliarHistorico &&
                                         x.FechaAdquisicion == fields.FechaAdquisicion &&
                                         x.FechaInicioDepreciacion == fields.FechaInicioDepreciacion);

      if (itemToDelete == null) {
        Assertion.RequireFail($"La lista no contiene el auxiliar {fields.AuxiliarHistorico}.");
      } else {
        itemToDelete.Delete();
      }

      return itemToDelete;
    }


    internal DepreciacionActivoFijoListItem UpdateItem(DepreciacionActivoFijoListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      var items = GetItems();

      if (items.Contains(x => x.AuxiliarHistorico.Number == fields.AuxiliarHistorico && x.UID != fields.UID)) {
        Assertion.RequireFail($"La lista ya contiene el auxiliar {fields.AuxiliarHistorico}.");
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
