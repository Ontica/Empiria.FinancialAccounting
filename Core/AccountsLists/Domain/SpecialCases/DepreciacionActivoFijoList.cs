/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Use cases Layer                         *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Use case interactor class               *
*  Type     : DepreciacionActivoFijoList                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Lista de depreciación de activos fijos.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

using Empiria.FinancialAccounting.AccountsLists.Adapters;
using Empiria.FinancialAccounting.AccountsLists.Data;

namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases {


  public class TipoActivoFijo : INamedEntity {

    private TipoActivoFijo() {
      // no-op
    }

    static internal TipoActivoFijo Parse(JsonObject instance) {
      return new TipoActivoFijo {
        UID = instance.Get<string>("uid"),
        Name = instance.Get<string>("name"),
        ValorHistoricoAccount = instance.Get<string>("valorHistoricoAccount"),
        RevaluacionAccount = instance.Get<string>("revaluacionAccount"),
        DepreciacionAcumuladaAccount = instance.Get<string>("depreciacionAcumuladaAccount")
      };
    }

    static public TipoActivoFijo Empty => new TipoActivoFijo {
      UID = "Empty",
      Name = "No determinado"
    };

    public string UID {
      get;
      private set;
    }

    public string Name {
      get;
      private set;
    }

    public string ValorHistoricoAccount {
      get;
      private set;
    } = string.Empty;

    public string RevaluacionAccount {
      get;
      private set;
    } = string.Empty;

    public string DepreciacionAcumuladaAccount {
      get;
      private set;
    } = string.Empty;

  }

  /// <summary>Lista de depreciación de activos fijos.</summary>
  public class DepreciacionActivoFijoList : AccountsList {

    static public DepreciacionActivoFijoList Parse() {
      return (DepreciacionActivoFijoList) AccountsList.Parse("DepreciacionActivoFijo");
    }

    static public new DepreciacionActivoFijoList Empty => BaseObject.ParseEmpty<DepreciacionActivoFijoList>();

    public FixedList<DepreciacionActivoFijoListItem> GetItems() {
      return AccountsListData.GetAccounts<DepreciacionActivoFijoListItem>(this);
    }

    public FixedList<DepreciacionActivoFijoListItem> GetItems(string keywords) {
      return AccountsListData.GetAccounts<DepreciacionActivoFijoListItem>(this, keywords);
    }


    public FixedList<TipoActivoFijo> TiposActivoFijo {
      get {
        return base.ExtendedDataField.GetFixedList<TipoActivoFijo>("tiposActivoFijo");
      }
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

  }  // class DepreciacionActivoFijoList

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
