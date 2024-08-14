/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Plain Objects                   *
*  Type     : LedgerRule, LedgerRuleComparer            License   : Please read LICENSE.txt file             *
*                                                                                                            *
*  Summary  : Contains information about an account's ledger application rule.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting {

  /// <summary>Contains information about an account's ledger application rule.</summary>
  public class LedgerRule {

    protected LedgerRule() {
      // Required by Empiria Framework.
    }

    [DataField("ID_CUENTA", ConvertFrom = typeof(long))]
    public string UID {
      get;
      private set;
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    internal int StandardAccountId {
      get; private set;
    }


    [DataField("ID_MAYOR", ConvertFrom = typeof(long))]
    public Ledger Ledger {
      get; private set;
    }


    public DateTime StartDate {
      get; private set;
    } = new DateTime(2021, 12, 31);


    public DateTime EndDate {
      get; private set;
    } = new DateTime(2049, 12, 31);


    internal bool AppliesOn(DateTime date) {
      return this.StartDate <= date && date <= this.EndDate;
    }

    public override bool Equals(object obj) => this.Equals(obj as LedgerRule);

    public bool Equals(LedgerRule obj) {
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

  }  // class LedgerRule



  /// <summary>Comparer class used to get a list of distinct LedgerRules for an account children.</summary>
  internal class LedgerRuleComparer : IEqualityComparer<LedgerRule> {

    public bool Equals(LedgerRule x, LedgerRule y) {
      return x.Ledger.Equals(y.Ledger) &&
             x.StartDate.Equals(y.StartDate) &&
             x.EndDate.Equals(y.EndDate);
    }

    public int GetHashCode(LedgerRule rule) {
      return (rule.Ledger, rule.StartDate, rule.EndDate).GetHashCode();
    }

  }  // class LedgerRuleComparer

}  // namespace Empiria.FinancialAccounting
