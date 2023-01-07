/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Domain Layer                          *
*  Assembly : FinancialAccounting.BalanceEngine.dll        Pattern   : Empiria General Object                *
*  Type     : StoredBalanceSet                             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Describes a stored accounts balance set.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Describes a stored accounts balance set.</summary>
  internal class StoredBalanceSet : GeneralObject {

    static private readonly Lazy<List<StoredBalanceSet>> _list =
                                    new Lazy<List<StoredBalanceSet>>(() => LoadList());

    private Lazy<FixedList<StoredBalance>> _balances;

    #region Constructors and parsers

    private StoredBalanceSet() {
      // Required by Empiria Framework.
    }

    private StoredBalanceSet(AccountsChart accountsChart, DateTime balancesDate) {
      this.AccountsChart = accountsChart;
      this.BalancesDate = balancesDate.Date;
    }


    static public StoredBalanceSet Parse(int id) {
      return BaseObject.ParseId<StoredBalanceSet>(id);
    }


    static public StoredBalanceSet Parse(string uid) {
      return BaseObject.ParseKey<StoredBalanceSet>(uid);
    }


    static public StoredBalanceSet Empty {
      get {
        return BaseObject.ParseEmpty<StoredBalanceSet>();
      }
    }


    static public FixedList<StoredBalanceSet> GetList(AccountsChart accountsChart) {
      return _list.Value.FindAll(x => x.AccountsChart.Equals(accountsChart))
                        .ToFixedList();
    }


    static internal StoredBalanceSet CreateOrGetBalanceSet(AccountsChart accountsChart,
                                                           DateTime balancesDate) {

      var existing = GetList(accountsChart).Find(x => x.BalancesDate == balancesDate.Date);

      if (existing != null) {
        return existing;
      }

      return new StoredBalanceSet(accountsChart, balancesDate);
    }


    static internal StoredBalanceSet GetBestBalanceSet(AccountsChart accountsChart,
                                                       DateTime fromDate) {

      var bestBalanceSet = GetList(accountsChart).FindLast(x => x.BalancesDate < fromDate && x.Calculated);

      Assertion.Require(bestBalanceSet,
          $"No hay ningún conjunto de saldos definidos para el catálogo {accountsChart.Name} " +
          $"para la fecha {fromDate.ToString("dd/MMM/yyyy")}.");

      return bestBalanceSet;
    }


    protected override void OnInitialize() {
      ResetBalances();
    }


    #endregion Constructors and parsers

    #region Properties

    public AccountsChart AccountsChart {
      get {
        return this.ExtendedDataField.Get<AccountsChart>("accountsChartId");
      }
      set {
        this.ExtendedDataField.Set("accountsChartId", value.Id);
      }
    }


    public DateTime BalancesDate {
      get {
        return this.ExtendedDataField.Get<DateTime>("balancesDate");
      }
      set {
        this.ExtendedDataField.Set("balancesDate", value);
      }
    }


    public bool Calculated {
      get {
        return this.ExtendedDataField.Get<bool>("calculated", false);
      }
      set {
        this.ExtendedDataField.Set("calculated", value);
      }
    }


    public DateTime CalculationTime {
      get {
        return this.ExtendedDataField.Get<DateTime>("calculationTime", ExecutionServer.DateMinValue);
      }
      set {
        this.ExtendedDataField.Set("calculationTime", value);
      }
    }


    public bool Protected {
      get {
        return this.ExtendedDataField.Get<bool>("protected", false);
      }
      set {
        this.ExtendedDataField.Set("protected", value);
      }
    }


    public FixedList<StoredBalance> Balances {
      get {
        return _balances.Value;
      }
    }


    public bool Unprotected {
      get {
        return !this.Protected;
      }
    }

    #endregion Properties

    #region Methods

    internal void Calculate() {
      Assertion.Require(this.Unprotected,
        "This balance set is protected. It can not be recalculated.");

      this.Calculated = false;
      this.Save();

      var query = new TrialBalanceQuery {
        AccountsChartUID = this.AccountsChart.UID,
        TrialBalanceType = TrialBalanceType.GeneracionDeSaldos,
        WithSubledgerAccount = true,
        BalancesType = BalancesType.AllAccounts,
        ShowCascadeBalances = true,
        UseCache = false,
        InitialPeriod = {
          FromDate = this.BalancesDate,
          ToDate = this.BalancesDate
        }
      };

      var balanceEngine = new TrialBalanceEngine(query);

      TrialBalance trialBalance = balanceEngine.BuildTrialBalance();

      StoredBalanceDataService.DeleteBalances(this);

      var entries = trialBalance.Entries.Select(x => (TrialBalanceEntry) x);

      foreach (var entry in entries) {
        var storedBalance = new StoredBalance(this, entry);

        storedBalance.Save();
      }

      this.CalculationTime = DateTime.Now;

      this.Calculated = true;

      this.Save();

      this.ResetBalances();
    }


    protected override void OnSave() {
      base.Name = $"Saldos acumulados al {this.BalancesDate.ToLongDateString()}";
      base.Keywords = EmpiriaString.BuildKeywords(base.Name);

      if (IsNew) {
        _list.Value.Add(this);
        _list.Value.Sort((x, y) => x.BalancesDate.CompareTo(y.BalancesDate));
      }

      base.OnSave();
    }


    private FixedList<StoredBalance> LoadBalances() {
      return StoredBalanceDataService.GetBalances(this);
    }

    static private List<StoredBalanceSet> LoadList() {
      var list = BaseObject.GetList<StoredBalanceSet>("ObjectStatus <> 'X'", string.Empty);

      list.Sort((x, y) => x.BalancesDate.CompareTo(y.BalancesDate));

      return list;
    }

    private void ResetBalances() {
      _balances = new Lazy<FixedList<StoredBalance>>(() => LoadBalances());
    }

    #endregion Methods

  }  // class StoredBalanceSet

}  // namespace Empiria.FinancialAccounting.BalanceEngine
