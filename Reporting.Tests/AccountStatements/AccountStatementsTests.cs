/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : AccountStatementsTests                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for Account statements.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Reporting;
using Empiria.FinancialAccounting.Reporting.AccountStatements;
using Empiria.FinancialAccounting.Reporting.AccountStatements.Adapters;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.Reporting.AccountStatements {

  /// <summary>Test cases for Account statements.</summary>
  public class AccountStatementsTests {


    [Fact]
    public void ShouldBuildAccountStatements() {

      AccountStatementQuery query = GetAccountStatementQuery();

      using (var useCase = AccountStatementUseCases.UseCaseInteractor()) {
        AccountStatementDto sut = useCase.BuildAccountStatement(query);

        Assert.NotNull(sut);
        Assert.NotEmpty(sut.Entries);
      }
    }


    #region Helpers


    private AccountStatementQuery GetAccountStatementQuery() {
      return new AccountStatementQuery {
        BalancesQuery = {
          AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
          FromAccount = "1.05.01.01.05.01",
          Ledgers = new string[]{ "81816c16-3306-98b0-66bf-a69021e31171" },
          InitialPeriod = {
            FromDate = new DateTime(2023, 06, 30),
            ToDate = new DateTime(2023, 06, 30),
            //ExchangeRateDate =new DateTime(2023, 06, 30),
            //ExchangeRateTypeUID = "96c617f6-8ed9-47f3-8d2d-f1240e446e1d",
            //ValuateToCurrrencyUID = "01"
          },
          TrialBalanceType = BalanceEngine.TrialBalanceType.BalanzaConContabilidadesEnCascada,
          UseDefaultValuation = true,
          WithSubledgerAccount = false,
          SubledgerAccount ="",
          
        },

        Entry = {
          AccountNumberForBalances = "1.05.01.01.05.01", //
          CurrencyCode = "02", //
          InitialBalance = 20042110138.42M,
          CurrentBalanceForBalances = 19887224722.07M,
          DebtorCreditor="Deudora",
          ItemType = BalanceEngine.TrialBalanceItemType.Entry,
          LastChangeDateForBalances = new DateTime(2023, 06, 30),
          LedgerNumber="09",
          LedgerUID="81816c16-3306-98b0-66bf-a69021e31171",
          SectorCode = "31",
          SubledgerAccountNumber ="0"
        },

        OrderBy = {
          SortType = AccountStatementOrder.AccountingDate,
          OrderType = AccountStatementOrderType.Ascending
        }
      };
    }

    #endregion Helpers

  } // class AccountStatementsTests

} // namespace Empiria.FinancialAccounting.Tests.Reporting.AccountStatements
