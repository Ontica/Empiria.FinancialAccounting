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

namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases {

  /// <summary>Describes a member of a financial accounts list.</summary>
  public class DepreciacionActivoFijoListItem : BaseObject, IAccountListItem {

    #region Constructors and parsers

    private DepreciacionActivoFijoListItem() {
      // Required by Empiria Framework.
    }


    protected DepreciacionActivoFijoListItem(AccountsList list) {
      this.List = list;
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


    public DateTime FechaDepreciacion {
      get {
        return this.ExtData.Get("fechaDepreciacion", ExecutionServer.DateMaxValue);
      }
      private set {
        this.ExtData.Set("fechaDepreciacion", value);
      }
    }


    public DateTime InicioDepreciacion {
      get {
        return this.ExtData.Get("inicioDepreciacion", ExecutionServer.DateMaxValue);
      }
      private set {
        this.ExtData.Set("inicioDepreciacion", value);
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


    public SubledgerAccount AuxiliarRevaluacion {
      get {
        return this.ExtData.Get<SubledgerAccount>("auxiliarRevaluacion", SubledgerAccount.Empty);
      }
      private set {
        this.ExtData.Set("auxiliarRevaluacion", value.Number);
      }
    }


    public string NumeroInventario {
      get {
        return this.SubledgerAccountNumber.Substring(2).TrimStart('0');
      }
    }

    #endregion Properties

  }  // class DepreciacionActivoFijoListItem

}  // namespace Empiria.FinancialAccounting.AccountsLists.SpecialCases
