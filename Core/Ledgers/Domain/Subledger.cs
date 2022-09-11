/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Aggregate root                          *
*  Type     : Subledger                                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about a subledger book.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about a subledger book.</summary>
  public class Subledger : BaseObject, INamedEntity {

    #region Constructors and parsers

    protected Subledger() {
      // Required by Empiria Framework.
    }


    static public Subledger Parse(int id) {
      return BaseObject.ParseId<Subledger>(id);
    }

    static public Subledger Parse(string uid) {
      return BaseObject.ParseKey<Subledger>(uid);
    }

    static public void Preload() {
      BaseObject.GetList<Subledger>();
    }

    static public Subledger Empty => BaseObject.ParseEmpty<Subledger>();


    #endregion Constructors and parsers

    #region Public properties


    [DataField("ID_MAYOR", ConvertFrom = typeof(long))]
    public Ledger BaseLedger {
      get; private set;
    }


    [DataField("ID_MAYOR_ADICIONAL", ConvertFrom = typeof(long))]
    public Ledger AdditionalLedger {
      get; private set;
    }


    [DataField("ID_TIPO_MAYOR_AUXILIAR", ConvertFrom = typeof(long))]
    public SubledgerType SubledgerType {
      get; private set;
    }


    [DataField("NOMBRE_MAYOR_AUXILIAR")]
    public string Name {
      get; private set;
    } = string.Empty;


    [DataField("DESCRIPCION")]
    public string Description {
      get; private set;
    } = string.Empty;


    [DataField("PREFIJO_CUENTAS_AUXILIARES")]
    public string AccountsPrefix {
      get; private set;
    } = string.Empty;


    [DataField("ELIMINADO", ConvertFrom = typeof(int))]
    public bool Deleted {
      get; private set;
    }


    #endregion Public properties

    #region Public methods

    public bool BelongsTo(Ledger ledger) {
      return ledger.Equals(this.BaseLedger) || ledger.Equals(this.AdditionalLedger);
    }


    internal SubledgerAccount CreateAccount(string subledgerAccountNo) {
      Assertion.Require(subledgerAccountNo, "subledgerAccountNo");

      return this.CreateAccount(subledgerAccountNo, "Sin nombre. Pendiente de clasificar.");
    }


    internal SubledgerAccount CreateAccount(string subledgerAccountNo, string subledgerAccountName) {
      Assertion.Require(subledgerAccountNo, "subledgerAccountNo");
      Assertion.Require(subledgerAccountName, "subledgerAccountName");

      return new SubledgerAccount(this, subledgerAccountNo, subledgerAccountName);
    }

    internal string FormatSubledgerAccount(string number) {
      Assertion.Require(number, "number");

      return this.BaseLedger.FormatSubledgerAccount(number);
    }


    public SubledgerAccount GetAccountWithId(int subledgerAccountId) {
      var subledgerAccount = SubledgerAccount.Parse(subledgerAccountId);

      Assertion.Require(subledgerAccount.Subledger.Equals(this),
          $"The subledger account with id {subledgerAccountId} does not " +
          $"belong to subledger '{this.Name}'.");

      return subledgerAccount;
    }


    #endregion Public methods

  }  // class Subledger

}  // namespace Empiria.FinancialAccounting
