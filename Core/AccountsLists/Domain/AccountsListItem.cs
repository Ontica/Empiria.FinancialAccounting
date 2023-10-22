/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Domain Layer                            *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Empiria Object                          *
*  Type     : AccountsListItem                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a memeber of a financial accounts list.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Json;

namespace Empiria.FinancialAccounting {

  public interface IAccountListItem : IIdentifiable {

  }

  /// <summary>Describes a member of a financial accounts list.</summary>
  public class AccountsListItem : BaseObject, IAccountListItem {

    #region Constructors and parsers

    private AccountsListItem() {
      // Required by Empiria Framework.
    }


    protected AccountsListItem(AccountsList list) {
      this.List = list;
    }

    static public AccountsListItem Parse(int id) {
      return BaseObject.ParseId<AccountsListItem>(id);
    }

    static public AccountsListItem Parse(string uid) {
      return BaseObject.ParseKey<AccountsListItem>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    // [DataField("ID_LISTA")]
    public AccountsList List {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string AccountNumber {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_AUXILIAR")]
    public string SubledgerAccountNumber {
      get; private set;
    }


    [DataField("CLAVE_SECTOR")]
    public string SectorCode {
      get; private set;
    }


    [DataField("CLAVE_MONEDA")]
    public string CurrencyCode {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_ESTANDAR_2")]
    public string TargetAccountNumber {
      get; private set;
    }


    public string ProfitAccount {
      get {
        return ExtData.Get("additionalAccounts/profitAccount", string.Empty);
      }
    }

    public string LossAccount {
      get {
        return ExtData.Get("additionalAccounts/lossAccount", string.Empty);
      }
    }


    [DataField("ELEMENTO_EXT_DATA")]
    public JsonObject ExtData {
      get; private set;
    }

    #endregion Properties

  }  // class AccountsListItem

}  // namespace Empiria.FinancialAccounting
