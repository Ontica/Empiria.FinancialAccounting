/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : Account                                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains information about an account.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Contains information about an account.</summary>
  public class Account : BaseObject {

    #region Fields

    static readonly public DateTime MAX_END_DATE = ConfigurationData.Get<DateTime>("MAX_ACCOUNT_END_DATE",
                                                                                   new DateTime(2049, 12, 31));

    private Lazy<FixedList<AreaRule>> _areaRules;

    private Lazy<FixedList<CurrencyRule>> _currencyRules;

    private Lazy<FixedList<LedgerRule>> _ledgerRules;

    private Lazy<FixedList<SectorRule>> _sectorRules;

    #endregion Fields

    #region Constructors and parsers

    protected Account() {
      // Required by Empiria Framework.
    }


    static public Account Parse(int id) {
      return BaseObject.ParseId<Account>(id);
    }


    static public Account Parse(string uid) {
      return BaseObject.ParseKey<Account>(uid);
    }


    static public Account Empty => BaseObject.ParseEmpty<Account>();


    protected override void OnLoad() {
      base.OnLoad();

      this.ResetAreaRules();
      this.ResetCurrencyRules();
      this.ResetLedgerRules();
      this.ResetSectorRules();
    }


    #endregion Constructors and parsers

    #region Public properties


    [DataField("ID_TIPO_CUENTAS_STD", ConvertFrom = typeof(long))]
    public AccountsChart AccountsChart {
      get; private set;
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    public int StandardAccountId {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string Number {
      get; private set;
    } = string.Empty;


    [DataField("NOMBRE_CUENTA_ESTANDAR")]
    public string Name {
      get; private set;
    } = string.Empty;


    [DataField("DESCRIPCION")]
    public string Description {
      get; private set;
    } = string.Empty;


    [DataField("ROL_CUENTA", Default = AccountRole.Sumaria)]
    public AccountRole Role {
      get; private set;
    } = AccountRole.Sumaria;


    [DataField("ID_TIPO_CUENTA")]
    private AccountType _accountType = FinancialAccounting.AccountType.Empty;

    public string AccountType {
      get {
        return _accountType.Name;
      }
    }


    [DataField("NATURALEZA", Default = DebtorCreditorType.Deudora)]
    public DebtorCreditorType DebtorCreditor {
      get; private set;
    } = DebtorCreditorType.Deudora;


    [DataField("FECHA_INICIO")]
    public DateTime StartDate {
      get; private set;
    }


    [DataField("FECHA_FIN")]
    public DateTime EndDate {
      get; private set;
    }


    public bool HasParent {
      get {
        return (this.Level > 1);
      }
    }


    public bool NotHasParent {
      get {
        return !this.HasParent;
      }
    }


    public int Level {
      get {
        var accountNumberSeparator = this.AccountsChart.MasterData.AccountNumberSeparator;

        return EmpiriaString.CountOccurences(Number, accountNumberSeparator) + 1;
      }
    }


    public bool IsSummaryWithNotChildren {
      get {
        if (this.Role != AccountRole.Sumaria) {
          return false;
        }
        return !this.AccountsChart.HasChildren(this);
      }
    }

    public FixedList<AreaRule> AreaRules {
      get {
        return _areaRules.Value;
      }
    }


    public FixedList<CurrencyRule> CurrencyRules {
      get {
        return _currencyRules.Value;
      }
    }

    public FixedList<LedgerRule> LedgerRules {
      get {
        return _ledgerRules.Value;
      }
    }


    public FixedList<SectorRule> SectorRules {
      get {
        return _sectorRules.Value;
      }
    }

    #endregion Public properties


    #region Public methods

    public void CheckIsNotSummary(DateTime accountingDate) {
      Account account = this.GetHistory(accountingDate);

      Assertion.Assert(account.Role != AccountRole.Sumaria,
          $"La cuenta {account.Number} es sumaria, por lo que no admite movimientos.");
    }


    public void CheckCurrencyRule(Currency currency, DateTime accountingDate) {
      Assertion.Assert(
          CurrencyRules.Contains(x => x.Currency.Equals(currency) && x.AppliesOn(accountingDate)),
          $"La moneda {currency.Name} no está definida para la cuenta {this.Number}.");
    }


    public void CheckNoEventTypeRule(DateTime accountingDate) {
      if (this.Number.StartsWith("13")) {
        Assertion.AssertFail($"La cuenta {this.Number} necesita un tipo de evento, sin embargo no se proporcionó.");
      }
    }


    public void CheckSectorRule(Sector sector, DateTime accountingDate) {
      Account account = this.GetHistory(accountingDate);

      Assertion.Assert(account.Role == AccountRole.Sectorizada,
          $"La cuenta {account.Number} no maneja sectores, sin embargo se proporcionó el sector {sector.FullName}.");

      Assertion.Assert(
          SectorRules.Contains(x => x.Sector.Equals(sector) && x.AppliesOn(accountingDate)),
          $"El sector {sector.Code} no está definido para la cuenta {account.Number}.");
    }


    public void CheckNoSectorRule(DateTime accountingDate) {
      Account account = this.GetHistory(accountingDate);

      Assertion.Assert(account.Role != AccountRole.Sectorizada,
                       $"La cuenta {account.Number} maneja sectores, sin embargo no se proporcionó.");
    }


    public void CheckSubledgerAccountRule(DateTime accountingDate) {
      Account account = this.GetHistory(accountingDate);

      Assertion.Assert(account.Role == AccountRole.Control,
                      $"La cuenta {account.Number} no maneja auxiliares.");
    }


    public void CheckSubledgerAccountRule(Sector sector, DateTime accountingDate) {
      Assertion.AssertObject(sector, "sector");

      Account account = this.GetHistory(accountingDate);

      SectorRule sectorRule = account.GetSectors(accountingDate).Find(x => x.Sector.Equals(sector));

      if (sectorRule == null) {
        Assertion.AssertFail($"La cuenta {account.Number} no maneja el sector {sector.FullName}.");

      } else {
        Assertion.Assert(account.Role == AccountRole.Sectorizada && sectorRule.SectorRole == AccountRole.Control,
            $"La cuenta {account.Number} no maneja auxiliares para el sector ({sector.Code}).");
      }
    }


    public void CheckNoSubledgerAccountRule(DateTime accountingDate) {
      Account account = this.GetHistory(accountingDate);

      Assertion.Assert(account.Role == AccountRole.Detalle,
                       $"La cuenta {account.Number} maneja auxiliares.");
    }


    public void CheckNoSubledgerAccountRule(Sector sector, DateTime accountingDate) {
      Assertion.AssertObject(sector, "sector");

      Account account = this.GetHistory(accountingDate);

      SectorRule sectorRule = account.GetSectors(accountingDate).Find(x => x.Sector.Equals(sector));

      if (sectorRule == null) {
        Assertion.AssertFail($"La cuenta {account.Number} no maneja el sector {sector.FullName}.");

      } else {
        Assertion.Assert(account.Role == AccountRole.Sectorizada && sectorRule.SectorRole == AccountRole.Detalle,
                         $"La cuenta {account.Number} maneja auxiliares para el sector ({sector.Code}).");
      }
    }


    internal FixedList<Account> GetChildren() {
      return this.AccountsChart.Accounts.FindAll(x => x.Number.StartsWith(this.Number) &&
                                                     !x.Number.Equals(this.Number));
    }


    internal FixedList<Account> GetHistory() {
      return this.AccountsChart.GetAccountHistory(this.Number);
    }


    internal Account GetHistory(DateTime date) {
      return this.AccountsChart.GetAccountHistory(this.Number, date);
    }


    public Account GetParent() {
      if (this.Level == 1) {
        return Account.Empty;
      }

      var accountNumberSeparator = this.AccountsChart.MasterData.AccountNumberSeparator;

      var parentAccountNumber = this.Number.Substring(0, this.Number.LastIndexOf(accountNumberSeparator));

      return AccountsChart.GetAccount(parentAccountNumber);
    }

    internal FixedList<SectorRule> GetSectors() {
      return GetSectors(DateTime.Today);
    }


    internal FixedList<SectorRule> GetSectors(DateTime date) {
      return _sectorRules.Value.FindAll(x => x.StartDate <= date.Date && date.Date <= x.EndDate);
    }


    internal bool HasChildrenWithSectors() {
      FixedList<Account> children = this.GetChildren();

      return children.Contains(x => x.Role == AccountRole.Sectorizada);
    }

    #endregion Public methods

    #region Private methods

    private void ResetAreaRules() {
      _areaRules = new Lazy<FixedList<AreaRule>>(() => AreasRulesChart.GetAccountAreasRules(this));
    }

    private void ResetCurrencyRules() {
      _currencyRules = new Lazy<FixedList<CurrencyRule>>(() => CurrenciesRulesChart.GetAccountCurrenciesRules(this));
    }

    private void ResetLedgerRules() {
      _ledgerRules = new Lazy<FixedList<LedgerRule>>(() => LedgersRulesChart.GetAccountLedgerRules(this));
    }

    private void ResetSectorRules() {
      _sectorRules = new Lazy<FixedList<SectorRule>>(() => SectorRulesChart.GetAccountSectorRules(this));
    }

    #endregion Private methods

  }  // class Account

}  // namespace Empiria.FinancialAccounting
