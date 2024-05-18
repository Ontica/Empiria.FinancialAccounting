/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Data Layer                              *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Data Service                            *
*  Type     : AccountStatementDataService                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data services that gets information used to build account statements.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;
using Empiria.FinancialAccounting.Reporting.AccountStatements.Domain;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements {

  /// <summary>Data services that gets information used to build account statements. </summary>
  static internal class AccountStatementDataService {

    static internal FixedList<AccountStatementEntry> GetVouchersWithAccounts(AccountStatementSqlClauses sqlClauses) {

      var operation = DataOperation.Parse("@qryVouchersByAccount",
                                          sqlClauses.AccountsChartId,
                                          DataCommonMethods.FormatSqlDbDate(sqlClauses.FromDate),
                                          DataCommonMethods.FormatSqlDbDate(sqlClauses.ToDate),
                                          sqlClauses.Filters,
                                          sqlClauses.Fields,
                                          sqlClauses.Grouping);

      return DataReader.GetPlainObjectFixedList<AccountStatementEntry>(operation);
    }

  } // class AccountStatementDataService

} // namespace Empiria.FinancialAccounting.Reporting.AccountStatements
