/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Domain Layer                            *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Empiria Object                          *
*  Type     : PrestamosInterbancariosListItem            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a memeber of a financial accounts list.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.AccountsLists.Adapters;
using Empiria.FinancialAccounting.AccountsLists.Data;

namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases {

  /// <summary>Describes a member of a financial accounts list.</summary>
  public class PrestamosInterbancariosListItem : BaseObject, IAccountListItem {

    #region Constructors and parsers

    private PrestamosInterbancariosListItem() {
      // Required by Empiria Framework.
    }


    public PrestamosInterbancariosListItem(AccountsList list, PrestamosInterbancariosListItemFields fields) {
      Assertion.Require(list, nameof(list));
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      this.List = list;
      this.Update(fields);
    }

    static public PrestamosInterbancariosListItem Parse(int id) {
      return BaseObject.ParseId<PrestamosInterbancariosListItem>(id);
    }

    static public PrestamosInterbancariosListItem Parse(string uid) {
      return BaseObject.ParseKey<PrestamosInterbancariosListItem>(uid);
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


    [DataField("ELEMENTO_EXT_DATA")]
    internal JsonObject ExtData {
      get; private set;
    }


    [DataField("FECHA_INICIO")]
    internal DateTime StartDate {
      get;
      private set;
    } = AccountsChart.IFRS.MasterData.StartDate;


    [DataField("FECHA_FIN")]
    internal DateTime EndDate {
      get;
      private set;
    } = AccountsChart.IFRS.MasterData.EndDate;


    public string Keywords {
      get {
        return EmpiriaString.BuildKeywords(SubledgerAccount.Number, SubledgerAccount.Name,
                                           Prestamo.Name, Prestamo.Number, Prestamo.Bank);
      }
    }


    [DataField("STATUS_ELEMENTO_LISTA", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get;
      private set;
    } = EntityStatus.Active;


    [DataField("CLAVE_SECTOR")]
    public string ClaveSector {
      get; private set;
    }

    public Sector Sector {
      get {
        return Sector.Parse(ClaveSector);
      }
    }

    [DataField("CLAVE_MONEDA")]
    public string ClaveMoneda {
      get; private set;
    }

    public Currency Currency {
      get {
        return Currency.Parse(ClaveMoneda);
      }
    }

    public string PrestamoUID {
      get {
        return ExtData.Get<string>("prestamo");
      }
      private set {
        ExtData.Set("prestamo", value);
      }
    }


    public Prestamo Prestamo {
      get {
        return PrestamosInterbancariosList.Parse()
                                          .GetPrestamos()
                                          .Find(x => x.UID == PrestamoUID);
      }
    }

    public DateTime Vencimiento {
      get {
        return ExtData.Get<DateTime>("vencimiento");
      }
      private set {
        ExtData.Set("vencimiento", value);
      }
    }

    #endregion Properties

    #region Methods

    internal void Delete() {
      Status = EntityStatus.Deleted;
    }

    protected override void OnSave() {
      AccountsListData.Write(this);
    }

    internal void Update(PrestamosInterbancariosListItemFields fields) {
      SubledgerAccountNumber = fields.SubledgerAccountNumber;
      ClaveMoneda = fields.CurrencyCode;
      ClaveSector = fields.SectorCode;
      PrestamoUID = fields.PrestamoUID;
      Vencimiento = fields.Vencimiento;
    }

    #endregion Methods

  }  // class PrestamosInterbancariosListItem

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
