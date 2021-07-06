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

    static internal FixedList<SubsidiaryAccount> GetSubsidiaryAccountsList(string keywords) {
      string sql = $"SELECT * FROM COF_CUENTA_AUXILIAR " +
                   $"WHERE NOMBRE_CUENTA_AUXILIAR LIKE '%{keywords}%' ";

      DataOperation operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<SubsidiaryAccount>(operation);
    }

  }  // class SubsidiaryLedgerData

}  // namespace Empiria.FinancialAccounting
