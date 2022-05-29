/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Structurer                              *
*  Type     : AccountsChartMasterData                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Structure that holds master data for an accounts chart.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Structure that holds master data for an accounts chart.</summary>
  public class AccountsChartMasterData {

    #region Constructors and parsers

    internal AccountsChartMasterData(AccountsChart accountsChart) {
      Assertion.Require(accountsChart, "accountsChart");

      this.AccountsChart = accountsChart;
    }


    internal AccountsChartMasterData(AccountsChart accountsChart, JsonObject fields) {
      Assertion.Require(accountsChart, "accountsChart");
      Assertion.Require(fields, "fields");

      this.AccountsChart = accountsChart;

      Load(fields);
    }


    #endregion Constructors and parsers

    #region Properties

    public AccountsChart AccountsChart {
      get;
    }


    public string AccountsPattern {
      get;
      private set;
    } = string.Empty;


    public char AccountNumberSeparator {
      get;
      private set;
    } = '-';


    public int MaxAccountLevel {
      get {
        return EmpiriaString.CountOccurences(AccountsPattern, AccountNumberSeparator) + 1;
      }
    }


    public DateTime StartDate {
      get;
      private set;
    } = ExecutionServer.DateMinValue;


    public DateTime EndDate {
      get;
      private set;
    } = ExecutionServer.DateMaxValue;


    public FixedList<AccountRole> AccountRoles {
      get;
      private set;
    } = new FixedList<AccountRole>();


    public FixedList<AccountType> AccountTypes {
      get;
      private set;
    } = new FixedList<AccountType>();


    public FixedList<Currency> Currencies {
      get;
      private set;
    } = new FixedList<Currency>();


    public FixedList<Sector> Sectors {
      get;
      private set;
    } = new FixedList<Sector>();


    public FixedList<Ledger> Ledgers {
      get;
      private set;
    } = new FixedList<Ledger>();


    internal Calendar Calendar {
      get;
      private set;
    }

    #endregion Properties

    #region Private methods

    private void Load(JsonObject fields) {
      this.AccountsPattern = fields.Get<string>("accountsPattern");
      this.AccountNumberSeparator = fields.Get<char>("accountNumberSeparator", '-');

      this.StartDate = fields.Get<DateTime>("startDate", this.StartDate);
      this.EndDate = fields.Get<DateTime>("endDate", this.EndDate);

      this.AccountRoles = GetAccountRoles(fields);

      this.AccountTypes = GetAccountTypes(fields);

      this.Currencies = GetCurrencies(fields);

      this.Sectors = GetSectors(fields);

      this.Ledgers = GetLedgers();

      this.Calendar = Calendar.Parse(fields.Get<int>("calendarId"));
    }


    static private FixedList<AccountRole> GetAccountRoles(JsonObject fields) {
      if (fields.Contains("accountRoles")) {
        return fields.GetFixedList<AccountRole>("accountRoles");
      } else {
        var minimalRoles = new[] { AccountRole.Sumaria, AccountRole.Detalle };

        return new FixedList<AccountRole>(minimalRoles);
      }
    }


    static private FixedList<AccountType> GetAccountTypes(JsonObject fields) {
      if (fields.Contains("accountTypes")) {
        return fields.GetFixedList<AccountType>("accountTypes");
      } else {
        return AccountType.GetList();
      }
    }


    static private FixedList<Currency> GetCurrencies(JsonObject fields) {
      if (fields.Contains("currencies")) {
        return fields.GetFixedList<Currency>("currencies");
      } else {
        return Currency.GetList();
      }
    }


    static private FixedList<Sector> GetSectors(JsonObject fields) {
      if (fields.Contains("sectors")) {
        return fields.GetFixedList<Sector>("sectors");
      } else {
        return Sector.GetList();
      }
    }

    private FixedList<Ledger> GetLedgers() {
      return AccountsChartData.GetAccountChartLedgers(this.AccountsChart);
    }


    #endregion Private methods

  }  // class AccountsChartMasterData

}  // namespace Empiria.FinancialAccounting
