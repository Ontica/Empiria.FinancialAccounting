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

using Empiria.Json;

using Empiria.FinancialAccounting.AccountsLists.Adapters;
using Empiria.FinancialAccounting.AccountsLists.Data;

namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases {

  public class SwapsCoberturaListConfigItem {

    static public SwapsCoberturaListConfigItem Parse(JsonObject json) {
      return new SwapsCoberturaListConfigItem {
        Value = json.Get<string>("value"),
        Group = json.Get<string>("group", string.Empty),
        IsTotalRow = json.Get<bool>("isTotalRow", false),
        Row = json.Get<int>("row")
      };
    }

    public string Value {
      get; internal set;
    }

    public string Group {
      get; internal set;
    }

    public bool IsTotalRow {
      get; internal set;
    }

    public int Row {
      get; internal set;
    }

  }

  /// <summary>Lista de Swaps de cobertura.</summary>
  public class SwapsCoberturaList : AccountsList {

    static public SwapsCoberturaList Parse() {
      return (SwapsCoberturaList) AccountsList.Parse("SwapsCobertura");
    }

    public FixedList<SwapsCoberturaListItem> GetItems() {
      return AccountsListData.GetAccounts<SwapsCoberturaListItem>(this);
    }

    public FixedList<SwapsCoberturaListItem> GetItems(string keywords) {
      return AccountsListData.GetAccounts<SwapsCoberturaListItem>(this, keywords);
    }

    public FixedList<SwapsCoberturaListConfigItem> GetConfiguration() {
      return ExtendedDataField.GetFixedList<SwapsCoberturaListConfigItem>("configuration");
    }

    public FixedList<string> GetClassificationValues() {
      return GetConfiguration().FindAll(x => !x.IsTotalRow)
                               .Select(x => x.Value)
                               .ToFixedList();
    }

    internal SwapsCoberturaListItem AddItem(SwapsCoberturaListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      var items = GetItems();

      if (items.Contains(x => x.SubledgerAccount.Number == fields.SubledgerAccountNumber)) {
        Assertion.RequireFail($"La lista ya contiene el auxiliar {fields.SubledgerAccountNumber}.");
      }

      return new SwapsCoberturaListItem(this, fields);
    }

    internal SwapsCoberturaListItem RemoveItem(SwapsCoberturaListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      var items = GetItems();

      var itemToDelete = items.Find(x => x.UID == fields.UID &&
                                         x.SubledgerAccount.Number == fields.SubledgerAccountNumber &&
                                         x.Classification == fields.Classification &&
                                         x.StartDate == fields.StartDate &&
                                         x.EndDate == fields.EndDate);

      if (itemToDelete == null) {
        Assertion.RequireFail($"La lista no contiene el auxiliar {fields.SubledgerAccountNumber}.");
      } else {
        itemToDelete.Delete();
      }

      return itemToDelete;
    }

    internal SwapsCoberturaListItem UpdateItem(SwapsCoberturaListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      var items = GetItems();

      if (items.Contains(x => x.SubledgerAccount.Number == fields.SubledgerAccountNumber && x.UID != fields.UID)) {
        Assertion.RequireFail($"La lista ya contiene el auxiliar {fields.SubledgerAccountNumber}.");
      }

      var itemToUpdate = items.Find(x => x.UID == fields.UID);

      if (itemToUpdate == null) {
        Assertion.RequireFail($"La lista no contiene un elemento con el identificador {fields.UID}.");
      } else {
        itemToUpdate.Update(fields);
      }

      return itemToUpdate;
    }

  }  // class SwapsCoberturaList

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
