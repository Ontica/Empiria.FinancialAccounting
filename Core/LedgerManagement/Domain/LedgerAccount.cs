/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : LedgerAccount                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about a ledger account.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about a ledger account.</summary>
  public class LedgerAccount : BaseObject {

    #region Constructors and parsers

    private LedgerAccount() {
      // Required by Empiria Framework.
    }


    static internal LedgerAccount Parse(int id) {
      return BaseObject.ParseId<LedgerAccount>(id);
    }

    static public LedgerAccount Empty => BaseObject.ParseEmpty<LedgerAccount>();


    #endregion Constructors and parsers

    #region Public properties

    [DataField("ID_MAYOR", ConvertFrom = typeof(long))]
    public Ledger Ledger {
      get; private set;
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    private Account MasterAccount {
      get; set;
    }


    public string Number => this.MasterAccount.Number;

    public string Name => this.MasterAccount.Name;

    public string Description => this.MasterAccount.Description;

    public AccountRole Role => this.MasterAccount.Role;

    public string AccountType => this.MasterAccount.AccountType;

    public DebtorCreditorType DebtorCreditor => this.MasterAccount.DebtorCreditor;

    public int Level => this.MasterAccount.Level;


    #endregion Public properties

  }  // class LedgerAccount

}  // namespace Empiria.FinancialAccounting
