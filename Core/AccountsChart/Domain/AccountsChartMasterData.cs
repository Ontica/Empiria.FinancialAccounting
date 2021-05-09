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

namespace Empiria.FinancialAccounting {

  /// <summary>Structure that holds master data for an accounts chart.</summary>
  public class AccountsChartMasterData {

    #region Constructors and parsers

    internal AccountsChartMasterData(AccountsChart accountsChart) {
      Assertion.AssertObject(accountsChart, "accountsChart");

      this.AccountsChart = accountsChart;
    }


    internal AccountsChartMasterData(AccountsChart accountsChart, JsonObject fields) {
      Assertion.AssertObject(accountsChart, "accountsChart");
      Assertion.AssertObject(fields, "fields");

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


    #endregion Properties

    #region Methods

    private void Load(JsonObject fields) {
      this.AccountsPattern = fields.Get<string>("accountsPattern");

      this.AccountNumberSeparator = fields.Get<char>("accountNumberSeparator", '-');

      this.StartDate = fields.Get<DateTime>("startDate", this.StartDate);
      this.EndDate = fields.Get<DateTime>("endDate", this.EndDate);


      if (fields.Contains("accountRoles")) {
        this.AccountRoles = fields.GetFixedList<AccountRole>("accountRoles");
      } else {
        var minimalRoles = new[] { AccountRole.Sumaria, AccountRole.Detalle };

        this.AccountRoles = new FixedList<AccountRole>(minimalRoles);
      }


      if (fields.Contains("accountTypes")) {
        this.AccountTypes = fields.GetFixedList<AccountType>("accountTypes");
      } else {
        this.AccountTypes = AccountType.GetList();
      }


      if (fields.Contains("currencies")) {
        this.Currencies = fields.GetFixedList<Currency>("currencies");
      } else {
        this.Currencies = Currency.GetList();
      }


      if (fields.Contains("sectors")) {
        this.Sectors = fields.GetFixedList<Sector>("sectors");
      } else {
        this.Sectors = Sector.GetList();
      }

    }

    #endregion Methods

  }  // class AccountsChartMasterData

}  // namespace Empiria.FinancialAccounting
