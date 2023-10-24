/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Domain Layer                            *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Empiria Object                          *
*  Type     : SwapsCoberturaListItem                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a memeber of a financial accounts list.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases {

  /// <summary>Describes a member of a financial accounts list.</summary>
  public class SwapsCoberturaListItem : BaseObject, IAccountListItem {

    #region Constructors and parsers

    private SwapsCoberturaListItem() {
      // Required by Empiria Framework.
    }


    protected SwapsCoberturaListItem(AccountsList list) {
      this.List = list;
    }

    static public SwapsCoberturaListItem Parse(int id) {
      return BaseObject.ParseId<SwapsCoberturaListItem>(id);
    }

    static public SwapsCoberturaListItem Parse(string uid) {
      return BaseObject.ParseKey<SwapsCoberturaListItem>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_LISTA")]
    public AccountsList List {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_AUXILIAR")]
    public string SubledgerAccountNumber {
      get; private set;
    }


    private SubledgerAccount _subledgerAccount;
    public SubledgerAccount SubledgerAccount {
      get {
        if (_subledgerAccount == null) {
          _subledgerAccount = SubledgerAccount.TryParse(AccountsChart.IFRS, this.SubledgerAccountNumber);
          if (_subledgerAccount == null) {
            return SubledgerAccount.Empty;
          }
        }
        return _subledgerAccount;
      }
    }

    public string Classification {
      get {
        return ExtData.Get("classification", string.Empty);
      }
    }


    [DataField("ELEMENTO_EXT_DATA")]
    internal JsonObject ExtData {
      get; private set;
    }

    #endregion Properties

  }  // class SwapsCoberturaListItem

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
