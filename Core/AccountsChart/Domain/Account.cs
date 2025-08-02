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
using System.Linq;

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
      this.ResetRules();
    }


    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_TIPO_CUENTAS_STD", ConvertFrom = typeof(long))]
    public AccountsChart AccountsChart {
      get; private set;
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    public StandardAccount StandardAccount {
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


    public string FullName {
      get {
        if (HasParent) {
          return $"{Name} - {GetParent().FullName}";
        } else if (!this.IsEmptyInstance) {
          return Name;
        } else {
          return string.Empty;
        }
      }
    }

    [DataField("DESCRIPCION")]
    public string Description {
      get; private set;
    } = string.Empty;


    [DataField("ROL_CUENTA", Default = AccountRole.Sumaria)]
    public AccountRole Role {
      get; private set;
    } = AccountRole.Sumaria;


    [DataField("ID_TIPO_CUENTA")]
    public AccountType AccountType {
      get;
      private set;
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
        if (this.IsEmptyInstance) {
          return 0;
        }
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


    public FixedList<CurrencyRule> AllCurrencyRules {
      get {
        return _currencyRules.Value;
      }
    }


    public FixedList<SectorRule> AllSectorRules {
      get {
        return _sectorRules.Value;
      }
    }


    public FixedList<AreaRule> AreaRules {
      get {
        return _areaRules.Value;
      }
    }


    public FixedList<LedgerRule> LedgerRules {
      get {
        return _ledgerRules.Value;
      }
    }

    #endregion Properties

    #region Public methods

    internal FixedList<AreaRule> GetCascadeAreas(DateTime date) {
      return this.AreaRules;
    }


    internal FixedList<CurrencyRule> GetCascadeCurrencies(DateTime date) {
      return GetCascadeCurrencies(date, date);
    }


    internal FixedList<CurrencyRule> GetCascadeCurrencies(DateTime fromDate,
                                                          DateTime toDate) {
      Assertion.Require(fromDate <= toDate, "fromDate must be less or equal than toDate.");

      if (this.Role != AccountRole.Sumaria) {
        return GetCurrencies(fromDate, toDate);
      }

      FixedList<Account> children = GetChildren(fromDate, toDate).FindAll(x => x.Role != AccountRole.Sumaria);

      return children.SelectMany(x => x.GetCurrencies(fromDate, toDate))
                     .Distinct(new CurrencyRuleComparer())
                     .OrderBy(x => x.Currency.Code)
                     .ToFixedList();
    }


    internal FixedList<SectorRule> GetCascadeSectors(DateTime date) {
      return GetCascadeSectors(date, date);
    }


    internal FixedList<SectorRule> GetCascadeSectors(DateTime fromDate, DateTime toDate) {
      if (this.Role == AccountRole.Sectorizada) {
        return GetSectors(fromDate, toDate);
      }

      FixedList<Account> children = GetChildren(fromDate, toDate).FindAll(x => x.Role == AccountRole.Sectorizada);

      return children.SelectMany(x => x.GetSectors(fromDate, toDate))
                     .Distinct(new SectorRuleComparer())
                     .OrderBy(x => x.Sector.Code)
                     .ToFixedList();
    }


    internal FixedList<LedgerRule> GetCascadeLedgers(DateTime date) {
      if (this.Role != AccountRole.Sumaria) {
        return GetLedgers(date);
      }

      FixedList<Account> children = GetChildren(date).FindAll(x => x.Role != AccountRole.Sumaria);

      return children.SelectMany(x => x.GetLedgers(date))
                     .Distinct(new LedgerRuleComparer())
                     .OrderBy(x => x.Ledger.Number)
                     .ToFixedList();
    }


    internal FixedList<Account> GetChildren() {
      return this.AccountsChart.Accounts.FindAll(x => x.Number.StartsWith(this.Number) &&
                                                     !x.Number.Equals(this.Number));
    }


    private FixedList<Account> GetChildren(DateTime date) {
      return this.AccountsChart.Accounts.FindAll(x => x.Number.StartsWith(this.Number) &&
                                                     !x.Number.Equals(this.Number) &&
                                                     (x.StartDate <= date && date <= x.EndDate));
    }


    private FixedList<Account> GetChildren(DateTime fromDate, DateTime toDate) {
      Assertion.Require(fromDate <= toDate, "fromDate must be less or equal than toDate.");

      return this.AccountsChart.Accounts.FindAll(x => x.Number.StartsWith(this.Number) &&
                                                     !x.Number.Equals(this.Number) &&
                                                     !(toDate < x.StartDate || fromDate > x.EndDate));
    }

    public FixedList<CurrencyRule> GetCurrencies(DateTime date) {
      return _currencyRules.Value.FindAll(x => x.AppliesOn(date));
    }

    public FixedList<CurrencyRule> GetCurrencies(DateTime fromDate, DateTime toDate) {
      return _currencyRules.Value.FindAll(x => x.AppliesOn(fromDate, toDate));
    }


    private FixedList<LedgerRule> GetLedgers(DateTime date) {
      return _ledgerRules.Value.FindAll(x => x.AppliesOn(date));
    }


    internal FixedList<Account> GetHistory() {
      return this.AccountsChart.GetAccountHistory(this.Number);
    }


    internal Account GetHistory(DateTime date) {
      return this.AccountsChart.GetAccountHistory(this.Number, date);
    }


    private Account _parent;
    public Account GetParent() {
      if (_parent != null) {
        return _parent;
      }

      if (this.Level == 1) {
        _parent = Account.Empty;
        return _parent;
      }

      var parentAccountNumber = this.AccountsChart.BuildParentAccountNumber(this.Number);

      _parent = AccountsChart.GetAccount(parentAccountNumber);

      return _parent;
    }


    public FixedList<SectorRule> GetSectors(DateTime date) {
      return _sectorRules.Value.FindAll(x => x.AppliesOn(date));
    }


    public FixedList<SectorRule> GetSectors(DateTime fromDate, DateTime toDate) {
      return _sectorRules.Value.FindAll(x => x.AppliesOn(fromDate, toDate));
    }

    internal bool HasChildrenWithSectors() {
      FixedList<Account> children = this.GetChildren();

      return children.Contains(x => x.Role == AccountRole.Sectorizada);
    }


    public Account TryGetHistory(DateTime date) {
      FixedList<Account> history = this.AccountsChart.GetAccountHistory(this.Number);

      return history.Find(x => x.StartDate <= date && date <= x.EndDate);
    }

    #endregion Public methods

    #region Private methods

    private void ResetRules() {
      AccountsChartRules rules = this.AccountsChart.Rules;

      _areaRules = new Lazy<FixedList<AreaRule>>(() => rules.GetAccountAreasRules(this));
      _currencyRules = new Lazy<FixedList<CurrencyRule>>(() => rules.GetAccountCurrenciesRules(this));
      _ledgerRules = new Lazy<FixedList<LedgerRule>>(() => rules.GetAccountLedgerRules(this));
      _sectorRules = new Lazy<FixedList<SectorRule>>(() => rules.GetAccountSectorRules(this));
    }

    #endregion Private methods

  }  // class Account

}  // namespace Empiria.FinancialAccounting
