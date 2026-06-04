/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Data Layer                              *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Data Services                           *
*  Type     : ReclassifiedBalancesDataService            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data access for reclassified balances data.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Reclassification.Data {

  /// <summary>Provides data access for Balanza Valorizada Real data.</summary>
  static internal class ReclassifiedBalancesDataService {

    static internal FixedList<AccountReclassifiedBalances> GetBalances(DateTime fromDate, DateTime toDate) {

      var op = DataOperation.Parse("qry_balanza_reclasificada", fromDate, toDate.AddDays(1));

      return DataReader.GetPlainObjectFixedList<AccountReclassifiedBalances>(op);
    }


  }  // class ReclassifiedBalancesDataService

}  // namespace Empiria.FinancialAccounting.Reclassification.Data
