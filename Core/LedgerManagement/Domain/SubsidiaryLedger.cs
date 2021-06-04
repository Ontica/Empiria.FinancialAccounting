/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Aggregate root                          *
*  Type     : SubsidiaryLedger                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about a subsidiary ledger book.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about a subsidiary ledger book.</summary>
  public class SubsidiaryLedger : BaseObject, INamedEntity {

    #region Constructors and parsers

    private SubsidiaryLedger() {
      // Required by Empiria Framework.
    }


    static public SubsidiaryLedger Parse(int id) {
      return BaseObject.ParseId<SubsidiaryLedger>(id);
    }

    static public SubsidiaryLedger Parse(string uid) {
      return BaseObject.ParseKey<SubsidiaryLedger>(uid);
    }

    static public void Preload() {
      BaseObject.GetList<SubsidiaryLedger>();
    }

    static public SubsidiaryLedger Empty => BaseObject.ParseEmpty<SubsidiaryLedger>();


    #endregion Constructors and parsers

    #region Public properties

    [DataField("ID_MAYOR", ConvertFrom=typeof(long))]
    public Ledger BaseLedger {
      get; private set;
    }


    [DataField("ID_TIPO_MAYOR_AUXILIAR", ConvertFrom = typeof(long))]
    public SubsidiaryLedgerType SubsidiaryLedgerType {
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

    public SubsidiaryAccount GetAccountWithId(int subsidaryAccountId) {
      var subsidaryAccount = SubsidiaryAccount.Parse(subsidaryAccountId);

      Assertion.Assert(subsidaryAccount.SubsidaryLedger.Equals(this),
          $"The subsidiary ledger account with id {subsidaryAccountId} does not " +
          $"belong to subsidiary ledger '{this.Name}'.");

      return subsidaryAccount;
    }

    #endregion Public methods

  }  // class SubsidiaryLedger

}  // namespace Empiria.FinancialAccounting
