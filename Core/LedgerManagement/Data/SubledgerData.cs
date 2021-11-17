/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : SubledgerData                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data service layer for subledger accounts.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Data {

  /// <summary>Data service layer for subledger accounts.</summary>
  static internal class SubledgerData {

    static internal FixedList<SubledgerAccount> GetSubledgerAccountsList(AccountsChart accountsChart,
                                                                         string keywords) {

      string sqlKeywords = SearchExpression.ParseAndLikeKeywords("KEYWORDS_CUENTA_AUXILIAR", keywords);

      DataOperation operation = DataOperation.Parse("@qry_cof_busca_auxiliares",
                                                    accountsChart.Id,
                                                    sqlKeywords);

      return DataReader.GetFixedList<SubledgerAccount>(operation);
    }


    static internal long NextSubledgerAccountId() {
      return CommonMethods.GetNextObjectId("SEC_ID_CUENTA_AUXILIAR");
    }


    static internal void WriteSubledgerAccount(SubledgerAccount o) {
      var op = DataOperation.Parse("write_cof_cuenta_auxiliar",
                                    o.Id, o.Subledger.Id,
                                    o.Number, o.Name, o.Description,
                                    o.Deleted ? 1: 0);

      DataWriter.Execute(op);
    }


  }  // class SubledgerData

}  // namespace Empiria.FinancialAccounting.Data
