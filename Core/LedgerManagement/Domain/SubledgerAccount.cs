/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : SubledgerAccount                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about a subledger account (cuenta auxiliar).                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about a subledger account (cuenta auxiliar).</summary>
  public class SubledgerAccount : BaseObject {

    #region Constructors and parsers

    protected SubledgerAccount() {
      // Required by Empiria Framework.
    }


    internal SubledgerAccount(Subledger subledger, string number, string name) {
      Assertion.AssertObject(subledger, "subledger");
      Assertion.AssertObject(number, "number");
      Assertion.AssertObject(name, "name");

      this.Subledger = subledger;
      this.Number = number;
      this.Name = name;
    }


    static public SubledgerAccount Parse(int id) {
      if (id == 0) {
        id = -1;
      }
      return BaseObject.ParseId<SubledgerAccount>(id);
    }


    static internal FixedList<SubledgerAccount> Search(AccountsChart accountsChart, string filter) {
      Assertion.AssertObject(accountsChart, "accountsChart");
      Assertion.AssertObject(filter, "filter");

      return SubledgerData.GetSubledgerAccountsList(accountsChart, filter);
    }


    static public void Preload() {
      BaseObject.GetList<SubledgerAccount>();
    }


    static public SubledgerAccount Empty => BaseObject.ParseEmpty<SubledgerAccount>();


    #endregion Constructors and parsers

    #region Public properties

    public Ledger Ledger {
      get {
        return this.Subledger.BaseLedger;
      }
    }


    public Ledger AdditionalLedger {
      get {
        return this.Subledger.AdditionalLedger;
      }
    }


    [DataField("ID_MAYOR_AUXILIAR", ConvertFrom = typeof(long))]
    public Subledger Subledger {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_AUXILIAR")]
    public string Number {
      get; private set;
    }


    [DataField("NOMBRE_CUENTA_AUXILIAR")]
    public string Name {
      get; private set;
    }


    [DataField("DESCRIPCION")]
    public string Description {
      get; private set;
    }


    [DataField("ELIMINADA", ConvertFrom = typeof(int))]
    public bool Suspended {
      get; private set;
    }

    #endregion Public properties

    #region Methods


    protected override void OnSave() {
      SubledgerData.WriteSubledgerAccount(this);
    }


    internal void Update(string number, string name) {
      Assertion.AssertObject(number, "number");
      Assertion.AssertObject(name, "name");

      this.Number = number;
      this.Name = name;
    }


    #endregion Methods

  }  // class SubledgerAccount

}  // namespace Empiria.FinancialAccounting
