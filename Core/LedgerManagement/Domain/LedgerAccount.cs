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

    #region Properties

    [DataField("ID_MAYOR", ConvertFrom = typeof(long))]
    public Ledger Ledger {
      get; private set;
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    internal int StandardAccountId {
      get; private set;
    }

    private Account _standardAccount;

    private Account StandardAccount {
      get {
        if (_standardAccount == null) {
          _standardAccount = GetStandardAccount();
        }
        return _standardAccount;
      }
    }


    public string Number => this.StandardAccount.Number;

    public string Name => this.StandardAccount.Name;

    public string Description => this.StandardAccount.Description;

    public AccountRole Role => this.StandardAccount.Role;

    public string AccountType => this.StandardAccount.AccountType;

    public DebtorCreditorType DebtorCreditor => this.StandardAccount.DebtorCreditor;

    public int Level => this.StandardAccount.Level;

    public FixedList<AreaRule> AreaRules => this.StandardAccount.AreaRules;

    public FixedList<CurrencyRule> CurrencyRules => this.StandardAccount.CurrencyRules;

    public FixedList<SectorRule> SectorRules => this.StandardAccount.SectorRules;

    #endregion Properties

    #region Methods

    private Account GetStandardAccount() {
      return Data.AccountsChartData.GetCurrentAccountWithStandardAccountId(this.StandardAccountId);
    }

    #endregion Methods

  }  // class LedgerAccount

}  // namespace Empiria.FinancialAccounting
