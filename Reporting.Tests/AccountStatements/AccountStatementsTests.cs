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
using Empiria.FinancialAccounting.Reporting.AccountStatements.Exporters;
using Empiria.Storage;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.Reporting.AccountStatements {

  /// <summary>Test cases for Account statements.</summary>
  public class AccountStatementsTests {


    [Fact]
    public void ShouldBuildAccountStatementsTest() {

      AccountStatementQuery query = GetAccountStatementQuery();

      using (var useCase = AccountStatementUseCases.UseCaseInteractor()) {
        AccountStatementDto sut = useCase.BuildAccountStatement(query);

        Assert.NotNull(sut);
        Assert.NotEmpty(sut.Entries);
      }
    }


    [Fact]
    public void AccountStatementsExcelExporterTest() {

      AccountStatementQuery query = GetAccountStatementQuery();

      using (var usecases = AccountStatementUseCases.UseCaseInteractor()) {

        AccountStatementDto accountStatement = usecases.BuildAccountStatement(query);

        var excelExporter = new AccountStatementExcelExporterService();

        FileDto excelFileDto = excelExporter.Export(accountStatement);

        Assert.NotNull(excelFileDto);
      }
    }


    #region Helpers


    private AccountStatementQuery GetAccountStatementQuery() {
      return new AccountStatementQuery {
        BalancesQuery = {
          AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
          FromAccount = "1.04.02.02.01.01.01.01",
          InitialPeriod = {
            FromDate = new DateTime(2024, 09, 01),
            ToDate = new DateTime(2024, 09, 18),
            ExchangeRateDate = new DateTime(2024, 09, 18),
            ExchangeRateTypeUID = "5923136d-8533-4975-81b9-c8ec3bf18dea",
            ValuateToCurrrencyUID ="01"
          },
          TrialBalanceType = BalanceEngine.TrialBalanceType.Balanza,
          UseDefaultValuation = false,
          WithSubledgerAccount = false,
        },

        Entry = {
          AccountNumberForBalances = "1.04.02.02.01.01.01.01", //
          CurrencyCode = "02", //
          InitialBalance = 1360669249.38M,
          CurrentBalanceForBalances = 1356773554.94M,
          DebtorCreditor="Deudora",
          ItemType = BalanceEngine.TrialBalanceItemType.Entry,
          LastChangeDateForBalances = new DateTime(2024, 09, 09),
          SectorCode = "00",
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
