/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Domain Layer                            *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Empiria Object                          *
*  Type     : ConciliacionDerivadosListItem              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a memeber of a financial accounts list.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases {

  /// <summary>Describes a member of a financial accounts list.</summary>
  public class ConciliacionDerivadosListItem : BaseObject, IAccountListItem {

    #region Constructors and parsers

    private ConciliacionDerivadosListItem() {
      // Required by Empiria Framework.
    }


    protected ConciliacionDerivadosListItem(AccountsList list, string accountNumber) {
      Assertion.Require(list, nameof(list));
      Assertion.Require(accountNumber, nameof(accountNumber));

      this.List = list;
      this.AccountNumber = accountNumber;
    }

    static public ConciliacionDerivadosListItem Parse(int id) {
      return BaseObject.ParseId<ConciliacionDerivadosListItem>(id);
    }

    static public ConciliacionDerivadosListItem Parse(string uid) {
      return BaseObject.ParseKey<ConciliacionDerivadosListItem>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_LISTA")]
    public AccountsList List {
      get; private set;
    }


    public Account Account {
      get {
        return AccountsChart.IFRS.GetAccount(this.AccountNumber);
      }
    }


    [DataField("NUMERO_CUENTA_ESTANDAR")]
    private string AccountNumber {
      get; set;
    }

    #endregion Properties

  }  // class ConciliacionDerivadosListItem

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
