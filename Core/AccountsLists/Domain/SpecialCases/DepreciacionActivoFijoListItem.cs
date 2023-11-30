/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Domain Layer                            *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Empiria Object                          *
*  Type     : DepreciacionActivoFijoListItem             License   : Please read LICENSE.txt file            *
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
  public class DepreciacionActivoFijoListItem : BaseObject, IAccountListItem {

    #region Constructors and parsers

    private DepreciacionActivoFijoListItem() {
      // Required by Empiria Framework.
    }


    internal DepreciacionActivoFijoListItem(AccountsList list, DepreciacionActivoFijoListItemFields fields) {
      Assertion.Require(list, nameof(list));
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      this.List = list;

      this.Update(fields);
    }

    static public DepreciacionActivoFijoListItem Parse(int id) {
      return BaseObject.ParseId<DepreciacionActivoFijoListItem>(id);
    }

    static public DepreciacionActivoFijoListItem Parse(string uid) {
      return BaseObject.ParseKey<DepreciacionActivoFijoListItem>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_LISTA")]
    public AccountsList List {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_AUXILIAR")]
    private string SubledgerAccountNumber {
      get; set;
    }


    private SubledgerAccount _subledgerAccount;
    public SubledgerAccount AuxiliarHistorico {
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


    [DataField("ID_MAYOR")]
    public Ledger Ledger {
      get;
      private set;
    }


    public DateTime FechaAdquisicion {
      get {
        return this.ExtData.Get("fechaAdquisicion", ExecutionServer.DateMaxValue);
      }
      private set {
        this.ExtData.Set("fechaAdquisicion", value);
      }
    }


    public DateTime FechaInicioDepreciacion {
      get {
        return this.ExtData.Get("fechaInicioDepreciacion", ExecutionServer.DateMaxValue);
      }
      private set {
        this.ExtData.Set("fechaInicioDepreciacion", value);
      }
    }


    public int MesesDepreciacion {
      get {
        return this.ExtData.Get("mesesDepreciacion", 0);
      }
      private set {
        this.ExtData.Set("mesesDepreciacion", value);
      }
    }


    private SubledgerAccount _auxiliarRevaluacion;
    public SubledgerAccount AuxiliarRevaluacion {
      get {
        if (this.NumeroAuxiliarRevaluacion.Length == 0) {
          return SubledgerAccount.Empty;
        }
        if (_auxiliarRevaluacion == null) {
          _auxiliarRevaluacion = SubledgerAccount.TryParse(AccountsChart.IFRS, this.NumeroAuxiliarRevaluacion);
          if (_auxiliarRevaluacion == null) {
            return SubledgerAccount.Empty;
          }
        }
        return _auxiliarRevaluacion;
      }
    }


    private string NumeroAuxiliarRevaluacion {
      get {
        return this.ExtData.Get<string>("auxiliarRevaluacion", string.Empty);
      }
      set {
        this.ExtData.SetIfValue("auxiliarRevaluacion", value);
      }
    }


    public decimal MontoRevaluacion {
      get {
        return this.ExtData.Get<decimal>("montoRevaluacion", 0);
      }
      set {
        this.ExtData.SetIfValue("montoRevaluacion", value);
      }
    }


    public string NumeroInventario {
      get {
        return this.SubledgerAccountNumber.Substring(2).TrimStart('0');
      }
    }


    [DataField("FECHA_INICIO")]
    internal DateTime StartDate {
      get;
      private set;
    } = ExecutionServer.DateMinValue;


    [DataField("FECHA_FIN")]
    internal DateTime EndDate {
      get;
      private set;
    } = ExecutionServer.DateMaxValue;


    public string Keywords {
      get {
        var keywords = EmpiriaString.BuildKeywords(AuxiliarHistorico.Number, AuxiliarHistorico.Name, Ledger.Name);

        if (!AuxiliarRevaluacion.IsEmptyInstance) {
          keywords += EmpiriaString.BuildKeywords(AuxiliarRevaluacion.Number, AuxiliarRevaluacion.Name);
        }

        return keywords;
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

    internal void Update(DepreciacionActivoFijoListItemFields fields) {
      SubledgerAccountNumber = fields.AuxiliarHistorico;
      this.Ledger = Ledger.Parse(fields.DelegacionUID);
      this.SubledgerAccountNumber = fields.AuxiliarHistorico;
      this.FechaAdquisicion = fields.FechaAdquisicion;
      this.FechaInicioDepreciacion = fields.FechaInicioDepreciacion;
      this.MesesDepreciacion = fields.MesesDepreciacion;
      this.NumeroAuxiliarRevaluacion = fields.AuxiliarRevaluacion;
      this.MontoRevaluacion = fields.MontoRevaluacion;
    }

    #endregion Methods

  }  // class DepreciacionActivoFijoListItem

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
