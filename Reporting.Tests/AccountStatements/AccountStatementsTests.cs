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

      AccountStatementQuery query = new AccountStatementQuery {
        BalancesQuery = {
          AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
          FromAccount = "1.09.05.02.02.02",
          InitialPeriod = {
            FromDate = new DateTime(2022, 01, 01),
            ToDate = new DateTime(2022, 01, 31),
          },
          TrialBalanceType = BalanceEngine.TrialBalanceType.SaldosPorCuenta,
          WithSubledgerAccount = true,
          SubledgerAccount =""
        },
        Entry = {
          AccountNumberForBalances = "1.09.05.02.02.02", //
          CurrencyCode = "01", //
          InitialBalance = 0,
          CurrentBalanceForBalances = 3879.50M,
          DebtorCreditor="",
          ItemType = BalanceEngine.TrialBalanceItemType.Entry,
          LastChangeDateForBalances = new DateTime(2022, 01, 14),
          LedgerNumber="",
          //LedgerUID="",
          SectorCode = "00",
          SubledgerAccountNumber ="0"
        }
      };

      using (var useCase = AccountStatementUseCases.UseCaseInteractor()) {
        AccountStatementDto sut = useCase.BuildAccountStatement(query);

        Assert.NotNull(sut);
        Assert.NotEmpty(sut.Entries);
      }

    }

  } // class AccountStatementsTests

} // namespace Empiria.FinancialAccounting.Tests.Reporting.AccountStatements
