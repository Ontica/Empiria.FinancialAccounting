/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Plain Objects                   *
*  Type     : SectorRule, SectorRuleComparer             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains information about an account's sector application rule.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting {

  /// <summary>Contains information about an account's sector application rule.</summary>
  public class SectorRule {

    protected SectorRule() {
      // Required by Empiria Framework.
    }


    public string UID {
      get {
        return $"{StandardAccountId}-{Sector.Id}-{StartDate.ToString("yyyyMMdd")}-{EndDate.ToString("yyyyMMdd")}";
      }
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    internal int StandardAccountId {
      get; private set;
    }


    [DataField("ID_SECTOR", ConvertFrom=typeof(long))]
    public Sector Sector {
      get; private set;
    }


    [DataField("ROL_SECTOR", Default = AccountRole.Detalle)]
    public AccountRole SectorRole {
      get; private set;
    } = AccountRole.Sumaria;


    [DataField("FECHA_INICIO")]
    public DateTime StartDate {
      get; private set;
    }


    [DataField("FECHA_FIN", Default = "ExecutionServer.DateMaxValue")]
    public DateTime EndDate {
      get; private set;
    }


    public bool AppliesOn(DateTime date) {
      return this.StartDate <= date && date <= this.EndDate;
    }

    public override bool Equals(object obj) => this.Equals(obj as SectorRule);

    public bool Equals(SectorRule obj) {
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

  }  // class SectorRule



  /// <summary>Comparer class used to get a list of distinct SectorRules for an account children.</summary>
  internal class SectorRuleComparer : IEqualityComparer<SectorRule> {

    public bool Equals(SectorRule x, SectorRule y) {
      return x.Sector.Equals(y.Sector) &&
             x.StartDate.Equals(y.StartDate) &&
             x.EndDate.Equals(y.EndDate);
    }

    public int GetHashCode(SectorRule rule) {
      return (rule.Sector.Id, rule.StartDate, rule.EndDate).GetHashCode();
    }

  }  // class SectorRuleComparer


}  // namespace Empiria.FinancialAccounting
