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
      this.ResetRules();
    }


    #endregion Constructors and parsers

    #region Properties

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

    public void CheckCurrencyRule(Currency currency, DateTime accountingDate) {
      Assertion.Require(
          AllCurrencyRules.Contains(x => x.Currency.Equals(currency) && x.AppliesOn(accountingDate)),
          $"La cuenta {this.Number} no permite movimientos en la moneda {currency.FullName}.");
    }


    public void CheckIsNotSummary() {
      Assertion.Require(this.Role != AccountRole.Sumaria,
          $"La cuenta {this.Number} es sumaria, por lo que no admite movimientos.");
    }


    public void CheckIsNotProtectedForEdition() {
      Assertion.Require(!this.Number.StartsWith("4.02.03.01") || ExecutionServer.CurrentPrincipal.IsInRole("administrador-operativo"),
                        $"La cuenta {this.Number} está protegida contra edición.");

      //Assertion.Require(!this.Number.StartsWith("9.01") || ExecutionServer.CurrentPrincipal.HasPermission("registro-manual-cuentas-protegidas"),
      //                  $"La cuenta {this.Number} está protegida contra edición.");
    }


    public void CheckNoEventTypeRule() {
      if (this.Number.StartsWith("13")) {
        Assertion.RequireFail($"La cuenta {this.Number} necesita un tipo de evento, sin embargo no se proporcionó.");
      }
    }


    public void CheckSectorRule(Sector sector, DateTime accountingDate) {
      Assertion.Require(this.Role == AccountRole.Sectorizada,
          $"La cuenta {this.Number} no requiere sector, sin embargo se proporcionó el sector {sector.FullName}.");

      Assertion.Require(
          AllSectorRules.Contains(x => x.Sector.Equals(sector) && x.AppliesOn(accountingDate)),
          $"El sector {sector.Code} no está definido para la cuenta {this.Number}.");
    }


    public void CheckNoSectorRule() {
      Assertion.Require(this.Role != AccountRole.Sectorizada,
                       $"La cuenta {this.Number} requiere un sector, sin embargo no se proporcionó.");
    }


    public void CheckSubledgerAccountRule() {
      Assertion.Require(this.Role == AccountRole.Control,
                      $"La cuenta {this.Number} no maneja auxiliares para el sector 00.");
    }


    public void CheckSubledgerAccountRule(Sector sector, DateTime accountingDate) {
      Assertion.Require(sector, "sector");

      SectorRule sectorRule = this.GetSectors(accountingDate).Find(x => x.Sector.Equals(sector));

      if (sectorRule == null) {
        Assertion.RequireFail($"La cuenta {this.Number} no maneja el sector {sector.FullName}.");

      } else {
        Assertion.Require(this.Role == AccountRole.Sectorizada && sectorRule.SectorRole == AccountRole.Control,
            $"La cuenta {this.Number} no requiere un auxiliar para el sector ({sector.Code}).");
      }
    }


    public void CheckNoSubledgerAccountRule() {
      Assertion.Require(this.Role == AccountRole.Detalle,
                       $"La cuenta {this.Number} requiere un auxiliar.");
    }


    public void CheckNoSubledgerAccountRule(Sector sector, DateTime accountingDate) {
      Assertion.Require(sector, "sector");

      SectorRule sectorRule = this.GetSectors(accountingDate).Find(x => x.Sector.Equals(sector));

      if (sectorRule == null) {
        Assertion.RequireFail($"La cuenta {this.Number} no maneja el sector {sector.FullName}.");

      } else {
        Assertion.Require(this.Role == AccountRole.Sectorizada && sectorRule.SectorRole == AccountRole.Detalle,
                         $"La cuenta {this.Number} maneja auxiliares para el sector ({sector.Code}).");
      }
    }


    internal FixedList<Account> GetChildren() {
      return this.AccountsChart.Accounts.FindAll(x => x.Number.StartsWith(this.Number) &&
                                                     !x.Number.Equals(this.Number));
    }


    public FixedList<CurrencyRule> GetCurrencies(DateTime date) {
      return _currencyRules.Value.FindAll(x => x.AppliesOn(date));
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

      var parentAccountNumber = this.AccountsChart.BuildParentAccountNumber(this.Number);

      return AccountsChart.GetAccount(parentAccountNumber);
    }


    public FixedList<SectorRule> GetSectors(DateTime date) {
      return _sectorRules.Value.FindAll(x => x.AppliesOn(date));
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
