/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Core                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : LedgerAccount                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains information about a ledger account.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Contains information about a ledger account.</summary>
  public class LedgerAccount : BaseObject {

    #region Constructors and parsers

    private LedgerAccount() {
      // Required by Empiria Framework.
    }


    static internal LedgerAccount Parse(int id) {
      return BaseObject.ParseId<LedgerAccount>(id);
    }

    static public LedgerAccount Parse(string accountNumber) {
      Assertion.AssertObject(accountNumber, "accountNumber");

      return BaseObject.ParseKey<LedgerAccount>(accountNumber);
    }

    static public LedgerAccount Empty => BaseObject.ParseEmpty<LedgerAccount>();


    #endregion Constructors and parsers

    #region Public properties

    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string AccountNumber {
      get; private set;
    } = string.Empty;


    [DataField("NOMBRE_CUENTA_ESTANDAR")]
    public string Name {
      get; private set;
    } = string.Empty;


    public string Description {
      get; private set;
    } = string.Empty;


    public string Role {
      get; private set;
    } = string.Empty;


    public string AccountType {
      get; private set;
    } = string.Empty;


    public string Naturaleza {
      get; private set;
    } = string.Empty;


    #endregion Public properties

  }  // class LedgerAccount

}  // namespace Empiria.FinancialAccounting
