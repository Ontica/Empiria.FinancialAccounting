/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Query payload                           *
*  Type     : BalancesPeriod                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes balances' date periods.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Describes balances' date periods.</summary>
  public class BalancesPeriod {

    public DateTime FromDate {
      get; set;
    }

    public DateTime ToDate {
      get; set;
    }

    public DateTime ExchangeRateDate {
      get;  set;
    } = DateTime.Today;


    public string ExchangeRateTypeUID {
      get; set;
    } = string.Empty;


    public string ValuateToCurrrencyUID {
      get; set;
    } = string.Empty;


    public bool UseDefaultValuation {
      get; set;
    } = false;


    public bool IsSecondPeriod {
      get; set;
    } = false;


    public override bool Equals(object obj) => this.Equals(obj as BalancesPeriod);

    public bool Equals(BalancesPeriod period) {
      if (period == null) {
        return false;
      }
      if (Object.ReferenceEquals(this, period)) {
        return true;
      }
      if (this.GetType() != period.GetType()) {
        return false;
      }

      return period.GetHashCode() == this.GetHashCode();
    }


    public override int GetHashCode() {
      var json = JsonObject.Parse(this);

      return json.GetHashCode();
    }

  }  // BalancesPeriod

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
