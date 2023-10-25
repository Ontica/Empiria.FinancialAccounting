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

using Empiria.StateEnums;

using Empiria.FinancialAccounting.AccountsLists.Adapters;
using Empiria.FinancialAccounting.AccountsLists.Data;

namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases {

  /// <summary>Describes a member of a financial accounts list.</summary>
  public class ConciliacionDerivadosListItem : BaseObject, IAccountListItem {

    #region Constructors and parsers

    private ConciliacionDerivadosListItem() {
      // Required by Empiria Framework.
    }


    internal protected ConciliacionDerivadosListItem(AccountsList list,
                                                     ConciliacionDerivadosListItemFields fields) {
      Assertion.Require(list, nameof(list));
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      this.List = list;
      this.AccountNumber = fields.AccountNumber;
      this.StartDate = fields.StartDate;
      this.EndDate = fields.EndDate;
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


    [DataField("FECHA_INICIO")]
    public DateTime StartDate {
      get;
      private set;
    }


    [DataField("FECHA_FIN")]
    public DateTime EndDate {
      get;
      private set;
    }


    public string Keywords {
      get {
        return EmpiriaString.BuildKeywords(AccountNumber, Account.Name);
      }
    }


    [DataField("STATUS_ELEMENTO_LISTA", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get;
      private set;
    } = EntityStatus.Active;


    #endregion Properties

    #region Methods

    internal void Delete() {
      Status = EntityStatus.Deleted;
    }


    protected override void OnSave() {
      AccountsListData.Write(this);
    }

    internal void Update(ConciliacionDerivadosListItemFields fields) {
      AccountNumber = fields.AccountNumber;
      StartDate = fields.StartDate;
      EndDate = fields.EndDate;
    }

    #endregion Methods

  }  // class ConciliacionDerivadosListItem

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
