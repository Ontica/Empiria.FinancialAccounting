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
using Empiria.FinancialAccounting.AccountsLists.Adapters;
using Empiria.FinancialAccounting.AccountsLists.Data;
using Empiria.Json;
using Empiria.StateEnums;

namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases {

  /// <summary>Describes a member of a financial accounts list.</summary>
  public class SwapsCoberturaListItem : BaseObject, IAccountListItem {

    #region Constructors and parsers

    private SwapsCoberturaListItem() {
      // Required by Empiria Framework.
    }


    public SwapsCoberturaListItem(AccountsList list, SwapsCoberturaListItemFields fields) {
      Assertion.Require(list, nameof(list));
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      this.List = list;
      this.SubledgerAccountNumber = fields.SubledgerAccountNumber;
      this.Classification = fields.Classification;
      this.StartDate = fields.StartDate;
      this.EndDate = fields.EndDate;
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
      private set {
        ExtData.Set("classification", value);
      }
    }


    [DataField("ELEMENTO_EXT_DATA")]
    internal JsonObject ExtData {
      get; private set;
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
        return EmpiriaString.BuildKeywords(SubledgerAccount.Number, SubledgerAccount.Name, Classification);
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

    internal void Update(SwapsCoberturaListItemFields fields) {
      SubledgerAccountNumber = fields.SubledgerAccountNumber;
      Classification = fields.Classification;
      StartDate = fields.StartDate;
      EndDate = fields.EndDate;
    }

    #endregion Methods

  }  // class SwapsCoberturaListItem

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
