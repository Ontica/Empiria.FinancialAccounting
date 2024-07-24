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
          FromAccount = "1.01.01.02",
          InitialPeriod = {
            FromDate = new DateTime(2023, 06, 01),
            ToDate = new DateTime(2023, 06, 30),
          },
          TrialBalanceType = BalanceEngine.TrialBalanceType.Balanza,
          WithSubledgerAccount = false,
          SubledgerAccount ="",
          
        },

        Entry = {
          AccountNumberForBalances = "1.01.01", //
          CurrencyCode = "01", //
          InitialBalance = 24400,
          CurrentBalanceForBalances = 23400,
          DebtorCreditor="Deudora",
          ItemType = BalanceEngine.TrialBalanceItemType.Summary,
          LastChangeDateForBalances = new DateTime(2023, 06, 07),
          LedgerNumber="",
          //LedgerUID="",
          SectorCode = "00",
          SubledgerAccountNumber ="0"
        },

        OrderBy = {
          SortType = AccountStatementOrder.VoucherNumber,
          OrderType = AccountStatementOrderType.Descending
        }
      };
    }

    #endregion Helpers

  } // class AccountStatementsTests

} // namespace Empiria.FinancialAccounting.Tests.Reporting.AccountStatements
