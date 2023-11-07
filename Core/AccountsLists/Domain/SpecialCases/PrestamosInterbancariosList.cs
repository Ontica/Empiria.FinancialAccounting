﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Domain Layer                            *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : List class                              *
*  Type     : PrestamosInterbancariosList                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Lista de préstamos interbancarios y de otros organismos.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

using Empiria.FinancialAccounting.AccountsLists.Adapters;
using Empiria.FinancialAccounting.AccountsLists.Data;

namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases {

  public class Prestamo {

    static public Prestamo Parse(JsonObject json) {
      return new Prestamo {
        UID = json.Get<string>("uid"),
        Name = json.Get<string>("name"),
        Bank = json.Get<string>("bank"),
        Number = json.Get<string>("number"),
        Classification = json.Get<string>("classification"),
        Order = json.Get<int>("order"),
      };
    }

    public string UID {
      get; private set;
    }

    public string Name {
      get; private set;
    }

    public string Bank {
      get; private set;
    }

    public string Number {
      get; private set;
    }

    public string Classification {
      get; private set;
    }

    public int Order {
      get; private set;
    }

  }  // class Prestamo

  /// <summary>Lista de préstamos interbancarios y de otros organismos.</summary>
  public class PrestamosInterbancariosList : AccountsList {

    static public PrestamosInterbancariosList Parse() {
      return (PrestamosInterbancariosList) AccountsList.Parse("PrestamosInterbancarios");
    }

    public FixedList<PrestamosInterbancariosListItem> GetItems() {
      return AccountsListData.GetAccounts<PrestamosInterbancariosListItem>(this);
    }

    public FixedList<PrestamosInterbancariosListItem> GetItems(string keywords) {
      return AccountsListData.GetAccounts<PrestamosInterbancariosListItem>(this, keywords);
    }

    public FixedList<Prestamo> GetPrestamos() {
      return ExtendedDataField.GetFixedList<Prestamo>("prestamos");
    }


    internal PrestamosInterbancariosListItem AddItem(PrestamosInterbancariosListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      var items = GetItems();

      if (items.Contains(x => x.SubledgerAccount.Number == fields.SubledgerAccountNumber &&
                              x.Currency.Code == fields.CurrencyCode &&
                              x.Sector.Code == fields.SectorCode)) {
        Assertion.RequireFail($"La lista ya contiene el auxiliar {fields.SubledgerAccountNumber} " +
                              $"para la moneda {fields.CurrencyCode} y el sector {fields.SectorCode}.");
      }

      return new PrestamosInterbancariosListItem(this, fields);
    }

    internal PrestamosInterbancariosListItem RemoveItem(PrestamosInterbancariosListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      var items = GetItems();

      var itemToDelete = items.Find(x => x.UID == fields.UID &&
                                         x.SubledgerAccount.Number == fields.SubledgerAccountNumber &&
                                         x.PrestamoUID == fields.PrestamoUID);

      if (itemToDelete == null) {
        Assertion.RequireFail($"La lista no contiene el auxiliar {fields.SubledgerAccountNumber}.");
      } else {
        itemToDelete.Delete();
      }

      return itemToDelete;
    }

    internal PrestamosInterbancariosListItem UpdateItem(PrestamosInterbancariosListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      var items = GetItems();

      if (items.Contains(x => x.SubledgerAccount.Number == fields.SubledgerAccountNumber &&
                              x.Currency.Code == fields.CurrencyCode && x.Sector.Code == fields.SectorCode &&
                              x.UID != fields.UID)) {
        Assertion.RequireFail($"La lista ya contiene el auxiliar {fields.SubledgerAccountNumber} " +
                              $"para la moneda {fields.CurrencyCode} y el sector {fields.SectorCode}.");
      }

      var itemToUpdate = items.Find(x => x.UID == fields.UID);

      if (itemToUpdate == null) {
        Assertion.RequireFail($"La lista no contiene un elemento con el identificador {fields.UID}.");
      } else {
        itemToUpdate.Update(fields);
      }

      return itemToUpdate;
    }

  }  // class PrestamosInterbancariosList

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases