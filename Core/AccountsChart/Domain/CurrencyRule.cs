/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Plain Objects                   *
*  Type     : CurrencyRule, CurrencyRuleComparer         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains information about an account's currency application rule.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting {

  /// <summary>Contains information about an account's currency application rule.</summary>
  public class CurrencyRule {

    protected CurrencyRule() {
      // Required by Empiria Framework.
    }

    public string UID {
      get {
        return $"{StandardAccountId}-{Currency.Id}-{StartDate.ToString("yyyyMMdd")}-{EndDate.ToString("yyyyMMdd")}";
      }
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    internal int StandardAccountId {
      get; private set;
    }


    [DataField("ID_MONEDA", ConvertFrom = typeof(long))]
    public Currency Currency {
      get; private set;
    }


    [DataField("FECHA_INICIO")]
    public DateTime StartDate {
      get; private set;
    }


    [DataField("FECHA_FIN", Default = "ExecutionServer.DateMaxValue")]
    public DateTime EndDate {
      get; private set;
    }


    internal bool AppliesOn(DateTime date) {
      return this.StartDate <= date && date <= this.EndDate;
    }

    public override bool Equals(object obj) => this.Equals(obj as CurrencyRule);

    public bool Equals(CurrencyRule obj) {
      if (obj == null) {
        return false;
      }
      if (Object.ReferenceEquals(this, obj)) {
        return true;
      }
      if (this.GetType() != obj.GetType()) {
        return false;
      }

      return this.UID == obj.UID;
    }

    public override int GetHashCode() {
      return UID.GetHashCode();
    }

  }  // class CurrencyRule



  /// <summary>Comparer class used to get a list of distinct CurrencyRules for an account children.</summary>
  internal class CurrencyRuleComparer : IEqualityComparer<CurrencyRule> {

    public bool Equals(CurrencyRule x, CurrencyRule y) {
      return x.Currency.Equals(y.Currency) &&
             x.StartDate.Equals(y.StartDate) &&
             x.EndDate.Equals(y.EndDate);
    }

    public int GetHashCode(CurrencyRule rule) {
      return (rule.Currency, rule.StartDate, rule.EndDate).GetHashCode();
    }

  }  // class CurrencyRuleComparer

}  // namespace Empiria.FinancialAccounting
