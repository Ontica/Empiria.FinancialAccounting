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

using Empiria.FinancialAccounting.Adapters;
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

    public NumberedNamedEntityDto MapToNumberedNamedEntity() {
      return new NumberedNamedEntityDto {
        UID = this.UID,
        Number = this.Number,
        Name = this.Name,
        FullName = $"{this.Number} - {this.Name}"
      };
    }

    public string AccountType => this.StandardAccount.AccountType;

    public DebtorCreditorType DebtorCreditor => this.StandardAccount.DebtorCreditor;

    public int Level => this.StandardAccount.Level;

    public FixedList<AreaRule> AreaRules => this.StandardAccount.AreaRules;

    public FixedList<CurrencyRule> CurrencyRules => this.StandardAccount.CurrencyRules;

    public FixedList<SectorRule> SectorRules => this.StandardAccount.SectorRules;

    #endregion Properties

    #region Methods

    public void CheckIsNotSummary(DateTime accountingDate) {
      Account account = this.StandardAccount.GetHistory(accountingDate);

      Assertion.Assert(account.Role != AccountRole.Sumaria,
          $"La cuenta {account.Number} es sumaria, por lo que no admite movimientos.");
    }


    public void CheckCurrencyRule(Currency currency, DateTime accountingDate) {
      Assertion.Assert(
          CurrencyRules.Contains(x => x.Currency.Equals(currency) && x.AppliesOn(accountingDate)),
          $"La moneda {currency.Name} no está definida para la cuenta {this.StandardAccount.Number}.");
    }


    public void CheckNoEventTypeRule(DateTime accountingDate) {
      if (this.Number.StartsWith("13")) {
        Assertion.AssertFail($"La cuenta {this.StandardAccount.Number} necesita un tipo de evento, sin embargo no se proporcionó.");
      }
    }


    public void CheckSectorRule(Sector sector, DateTime accountingDate) {
      Account account = this.StandardAccount.GetHistory(accountingDate);

      Assertion.Assert(account.Role == AccountRole.Sectorizada,
          $"La cuenta {account.Number} no maneja sectores, sin embargo se proporcionó el sector {sector.FullName}.");

      Assertion.Assert(
          SectorRules.Contains(x => x.Sector.Equals(sector) && x.AppliesOn(accountingDate)),
          $"El sector {sector.Code} no está definido para la cuenta {account.Number}.");
    }


    public void CheckNoSectorRule(DateTime accountingDate) {
      Account account = this.StandardAccount.GetHistory(accountingDate);

      Assertion.Assert(account.Role != AccountRole.Sectorizada,
                       $"La cuenta {account.Number} maneja sectores, sin embargo no se proporcionó.");
    }


    public void CheckSubledgerAccountRule(SubsidiaryAccount subledgerAccount, DateTime accountingDate) {
      Assertion.AssertObject(subledgerAccount, "subledgerAccount");

      Account account = this.StandardAccount.GetHistory(accountingDate);

      Assertion.Assert(account.Role == AccountRole.Control,
          $"La cuenta {account.Number} no maneja auxiliares.");

      Assertion.Assert(subledgerAccount.SubsidaryLedger.BaseLedger.Equals(this.Ledger),
          $"El auxiliar {subledgerAccount.Number} no pertenece a la contabilidad {this.Ledger.FullName}.");
    }


    public void CheckSubledgerAccountRule(Sector sector, SubsidiaryAccount subledgerAccount,
                                          DateTime accountingDate) {
      Assertion.AssertObject(sector, "sector");
      Assertion.AssertObject(subledgerAccount, "subledgerAccount");

      Account account = this.StandardAccount.GetHistory(accountingDate);

      SectorRule sectorRule = account.GetSectors(accountingDate).Find(x => x.Sector.Equals(sector));

      if (sectorRule == null) {
        Assertion.AssertFail($"La cuenta {account.Number} no maneja el sector {sector.FullName}.");
      } else {
        Assertion.Assert(account.Role == AccountRole.Sectorizada && sectorRule.SectorRole == AccountRole.Control,
            $"La cuenta {account.Number} no maneja auxiliares para el sector ({sector.Code}).");
      }

      Assertion.Assert(subledgerAccount.SubsidaryLedger.BaseLedger.Equals(this.Ledger),
          $"El auxiliar {subledgerAccount.Number} no pertenece a la contabilidad {this.Ledger.FullName}.");
    }


    public void CheckNoSubledgerAccountRule(DateTime accountingDate) {
      Account account = this.StandardAccount.GetHistory(accountingDate);

      Assertion.Assert(account.Role == AccountRole.Detalle,
          $"La cuenta {account.Number} maneja auxiliares.");
    }


    public void CheckNoSubledgerAccountRule(Sector sector, DateTime accountingDate) {
      Assertion.AssertObject(sector, "sector");

      Account account = this.StandardAccount.GetHistory(accountingDate);

      SectorRule sectorRule = account.GetSectors(accountingDate).Find(x => x.Sector.Equals(sector));

      if (sectorRule == null) {
        Assertion.AssertFail($"La cuenta {account.Number} no maneja el sector {sector.FullName}.");
      } else {
        Assertion.Assert(account.Role == AccountRole.Sectorizada && sectorRule.SectorRole == AccountRole.Detalle,
             $"La cuenta {account.Number} maneja auxiliares para el sector ({sector.Code}).");
      }
    }


    internal FixedList<CurrencyRule> CurrencyRulesOn(DateTime date) {
      return this.CurrencyRules.FindAll(x => x.AppliesOn(date));
    }


    private Account GetStandardAccount() {
      return AccountsChartData.GetCurrentAccountWithStandardAccountId(this.StandardAccountId);
    }


    internal FixedList<SectorRule> SectorRulesOn(DateTime date) {
      return this.SectorRules.FindAll(x => x.AppliesOn(date));
    }


    #endregion Methods

  }  // class LedgerAccount

}  // namespace Empiria.FinancialAccounting
