/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Plain Objects                   *
*  Type     : AreaRule, AreaRuleComparer                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains information about an account's responsibility area application rule.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting {

  /// <summary>Contains information about an account's responsibility area application rule.</summary>
  public class AreaRule {

    protected AreaRule() {
      // Required by Empiria Framework.
    }

    public string UID {
      get {
        return $"{StandardAccountId}-{AreaCodePattern}-{StartDate.ToString("yyyyMMdd")}-{EndDate.ToString("yyyyMMdd")}";
      }
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    internal int StandardAccountId {
      get; private set;
    }


    [DataField("PATRON_AREA")]
    public string AreaCodePattern {
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

    public override bool Equals(object obj) => this.Equals(obj as AreaRule);

    public bool Equals(AreaRule obj) {
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

  }  // class AreaRule



  /// <summary>Comparer class used to get a list of distinct AreaRules for an account children.</summary>
  internal class AreaRuleComparer : IEqualityComparer<AreaRule> {

    public bool Equals(AreaRule x, AreaRule y) {
      return x.AreaCodePattern.Equals(y.AreaCodePattern) &&
             x.StartDate.Equals(y.StartDate) &&
             x.EndDate.Equals(y.EndDate);
    }

    public int GetHashCode(AreaRule rule) {
      return (rule.AreaCodePattern, rule.StartDate, rule.EndDate).GetHashCode();
    }

  }  // class AreaRuleComparer

}  // namespace Empiria.FinancialAccounting
