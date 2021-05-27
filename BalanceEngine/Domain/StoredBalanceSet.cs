/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria General Object                  *
*  Type     : StoredBalanceSet                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a stored balance set.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  public enum StoredBalanceSetType {
    TrialBalance
  }


  /// <summary>Describes a stored balance set.</summary>
  internal class StoredBalanceSet : GeneralObject {

    #region Constructors and parsers

    private StoredBalanceSet() {
      // Required by Empiria Framework.
    }

    public StoredBalanceSet(AccountsChart accountsChart,
                        DateTime balancesDate) {
      this.AcountsChart = accountsChart;
      this.BalancesDate = balancesDate;
    }


    static public StoredBalanceSet Parse(int id) {
      return BaseObject.ParseId<StoredBalanceSet>(id);
    }


    static public FixedList<StoredBalanceSet> GetList() {
      var list = BaseObject.GetList<StoredBalanceSet>("ObjectStatus <> 'X'", string.Empty);

      list.Sort((x, y) => x.BalancesDate.CompareTo(y.BalancesDate));

      return list.ToFixedList();
    }


    static internal StoredBalanceSet GetBestSet(StoredBalanceSetType storedBalanceSetType,
                                                DateTime fromDate) {
      return StoredBalanceSet.Parse(2001);
    }

    #endregion Constructors and parsers

    #region Properties

    public AccountsChart AcountsChart {
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


    public DateTime CaculationTime {
      get {
        return this.ExtendedDataField.Get<DateTime>("calculationTime");
      }
      set {
        this.ExtendedDataField.Set("calculationTime", value);
      }
    }

    #endregion Properties


    #region Methods

    protected override void OnSave() {
      base.Name = $"Saldos acumulados al {this.BalancesDate.ToLongDateString()}";
      this.CaculationTime = DateTime.Now;
      base.Keywords = EmpiriaString.BuildKeywords(base.Name);

      base.OnSave();
    }

    #endregion Methods

  }  // class StoredBalanceSet

}  // namespace Empiria.FinancialAccounting.BalanceEngine
