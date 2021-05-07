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
    } = new FixedList<AccountRole>(new[] { AccountRole.Summary, AccountRole.Posting });


    public FixedList<AccountType> AccountTypes {
      get;
      private set;
    }


    public FixedList<Currency> Currencies {
      get;
      private set;
    }


    #endregion Properties

    #region Methods

    private void Load(JsonObject fields) {
      this.AccountsPattern = fields.Get<string>("accountsPattern");
      this.StartDate = fields.Get<DateTime>("startDate", this.StartDate);
      this.EndDate = fields.Get<DateTime>("endDate", this.EndDate);

      if (fields.Contains("accountRoles")) {
        this.AccountRoles = fields.GetList<AccountRole>("accountRoles")
                                  .ToFixedList();
      }

      if (fields.Contains("accountTypes")) {
        this.AccountTypes = fields.GetList<AccountType>("accountTypes")
                                  .ToFixedList();
      } else {
        this.AccountTypes = AccountType.GetList();
      }

      if (fields.Contains("currencies")) {
        this.Currencies = fields.GetList<Currency>("currencies")
                                .ToFixedList();
      } else {
        this.Currencies = Currency.GetList();
      }

    }

    #endregion Methods

  }  // class AccountsChartMasterData

}  // namespace Empiria.FinancialAccounting
