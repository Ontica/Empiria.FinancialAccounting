/* Empiria Financial *****************************************************************************************
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

  public enum PrestamoBaseClasificacion {

    OtrosOrganismos = 1,

    AgenteFinanciero = 2,

    GobiernoFederal = 3,

    BancaComercial = 4,

    RegistroBackOffice = 5,

    None = 99,

    Total = 100,
  }

  static public class PrestamoBaseClasificacionMethods {

    static public string DisplayName(this PrestamoBaseClasificacion classification) {
      switch (classification) {
        case PrestamoBaseClasificacion.OtrosOrganismos:
          return "Préstamos de Otros Organismos";

        case PrestamoBaseClasificacion.AgenteFinanciero:
          return "Agente Financiero del Gobierno Federal";

        case PrestamoBaseClasificacion.GobiernoFederal:
          return "Gobierno Federal";

        case PrestamoBaseClasificacion.BancaComercial:
          return "Banca comercial";

        case PrestamoBaseClasificacion.None:
          return "Préstamos sin clasificación";

        case PrestamoBaseClasificacion.Total:
          return "TOTAL PRESTAMOS INTERBANCARIOS Y DE OTROS ORGANISMOS";

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled classification {classification}");
      }
    }

  }



  public class PrestamoBase {

    static public PrestamoBase Empty {
      get {
        return new PrestamoBase() {
          UID = "Empty",
          Name = string.Empty,
          Bank = string.Empty,
          Number = "",
          Classification = PrestamoBaseClasificacion.None,
          Order = 99,
        };
      }
    }

    static public PrestamoBase Unknown {
      get {
        return new PrestamoBase() {
          UID = "Unknown",
          Name = "No asignado",
          Bank = "No asignado",
          Number = "",
          Classification = PrestamoBaseClasificacion.None,
          Order = 98,
        };
      }
    }

    static public PrestamoBase Parse(JsonObject json) {
      return new PrestamoBase {
        UID = json.Get<string>("uid"),
        Name = json.Get<string>("name"),
        Bank = json.Get<string>("bank"),
        Number = json.Get<string>("number"),
        Classification = json.Get<PrestamoBaseClasificacion>("classification"),
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

    public PrestamoBaseClasificacion Classification {
      get; private set;
    }


    public int Order {
      get; private set;
    }

  }  // class PrestamoBase

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

    public FixedList<PrestamoBase> GetPrestamosBase() {
      return ExtendedDataField.GetFixedList<PrestamoBase>("prestamos");
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
