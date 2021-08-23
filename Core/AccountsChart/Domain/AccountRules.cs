/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Plain Objects                   *
*  Type     : AreaRules, CurrencyRule,                   License   : Please read LICENSE.txt file            *
*             LedgerRule, SectorRule                                                                         *
*                                                                                                            *
*  Summary  : Types that holds information about an account's responsibility area, sector, currency          *
*             and leger application rules.                                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

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

  }  // class AreaRule



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

  }  // class CurrencyRule



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
    } = new DateTime(2000, 12, 29);


    public DateTime EndDate {
      get; private set;
    } = new DateTime(2049, 12, 31);


    internal bool AppliesOn(DateTime date) {
      return this.StartDate <= date && date <= this.EndDate;
    }

  }  // class LedgerRule



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

  }  // class SectorRule

}  // namespace Empiria.FinancialAccounting
