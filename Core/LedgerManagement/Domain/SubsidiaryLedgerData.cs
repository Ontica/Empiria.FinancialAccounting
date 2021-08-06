/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : SubsidiaryAccount                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about a subsidiary ledger account.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting {

  static internal class SubsidiaryLedgerData {

    static internal FixedList<SubsidiaryAccount> GetSubsidiaryAccountsList(AccountsChart accountsChart,
                                                                           string keywords) {

      string sqlKeywords = SearchExpression.ParseAndLikeKeywords("KEYWORDS_CUENTA_AUXILIAR", keywords);

      DataOperation operation = DataOperation.Parse("@qry_cof_busca_auxiliares",
                                                    accountsChart.Id,
                                                    sqlKeywords);

      return DataReader.GetFixedList<SubsidiaryAccount>(operation);
    }

  }  // class SubsidiaryLedgerData

}  // namespace Empiria.FinancialAccounting
