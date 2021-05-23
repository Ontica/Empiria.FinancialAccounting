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
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting {

  /// <summary>Contains information about an account.</summary>
  public class Account : BaseObject {

    #region Fields

    static public DateTime MAX_END_DATE = ConfigurationData.Get<DateTime>("MAX_ACCOUNT_END_DATE",
                                                                           new DateTime(2049, 12, 31));

    private Lazy<FixedList<AreaRule>> _areaRules;

    private Lazy<FixedList<CurrencyRule>> _currencyRules;

    private Lazy<FixedList<LedgerRule>> _ledgerRules;

    private Lazy<FixedList<SectorRule>> _sectorRules;

    #endregion Fields

    #region Constructors and parsers

    private Account() {
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

    [DataField("ID_TIPO_CUENTAS_STD", ConvertFrom=typeof(long))]
    internal int AccountsChartId {
      get; private set;
    } = -1;


    public AccountsChart AccountsChart {
      get {
        return AccountsChart.Parse(this.AccountsChartId);
      }
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


    public int Level {
      get {
        return EmpiriaString.CountOccurences(Number, this.AccountsChart.MasterData.AccountNumberSeparator) + 1;
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

    internal FixedList<Account> GetHistory() {
      return this.AccountsChart.GetAccountHistory(this.Number);
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
