/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : SubsidiaryLedgerData                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data service layer for subsidiary accounts.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Data {

  /// <summary>Data service layer for subsidiary accounts.</summary>
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

}  // namespace Empiria.FinancialAccounting.Data
