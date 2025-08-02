/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : LedgerAccount                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about a ledger account.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about a ledger account.</summary>
  public class LedgerAccount : BaseObject {

    #region Constructors and parsers

    protected LedgerAccount() {
      // Required by Empiria Framework.
    }


    static public LedgerAccount Parse(int id) {
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
    public StandardAccount StandardAccount {
      get; private set;
    }


    private Account _account;

    private Account Account {
      get {
        if (_account == null) {
          _account = GetAccount();
        }
        return _account;
      }
    }

    public string Number => this.Account.Number;

    public string Name => this.Account.Name;

    public string ParentFullName => this.Account.GetParent().FullName;

    public string Description => this.Account.Description;

    public AccountRole Role => this.Account.Role;

    public AccountType AccountType => this.Account.AccountType;

    public DebtorCreditorType DebtorCreditor => this.Account.DebtorCreditor;

    public int Level => this.Account.Level;

    public FixedList<AreaRule> AreaRules => this.Account.AreaRules;

    public FixedList<CurrencyRule> CurrencyRules => this.Account.AllCurrencyRules;

    public FixedList<SectorRule> SectorRules => this.Account.AllSectorRules;

    #endregion Properties

    #region Methods

    internal FixedList<CurrencyRule> CurrencyRulesOn(DateTime date) {
      return this.CurrencyRules.FindAll(x => x.AppliesOn(date));
    }


    private Account GetAccount() {
      return AccountsChartData.GetCurrentAccountWithStandardAccountId(this.StandardAccount.Id);
    }


    public Account GetHistoric(DateTime accountingDate) {
      return this.Account.GetHistory(accountingDate);
    }


    public ShortFlexibleEntityDto MapToShortFlexibleEntity() {
      return new ShortFlexibleEntityDto {
        UID = this.UID,
        Number = this.Number,
        Name = this.Name,
        FullName = $"{this.Number} - {this.Name}"
      };
    }


    internal FixedList<SectorRule> SectorRulesOn(DateTime date) {
      return this.SectorRules.FindAll(x => x.AppliesOn(date));
    }

    #endregion Methods

  }  // class LedgerAccount

}  // namespace Empiria.FinancialAccounting
