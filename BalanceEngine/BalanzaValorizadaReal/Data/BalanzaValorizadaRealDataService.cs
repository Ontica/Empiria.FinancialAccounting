/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Services                           *
*  Type     : BalanzaValorizadaRealDataService           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data access for Balanza Valorizada Real data.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Provides data access for Balanza Valorizada Real data.</summary>
  static internal class BalanzaValorizadaRealDataService {

    static internal FixedList<BalanzaValorizadaReal> GetBalances(DateTime fromDate, DateTime toDate) {

      var op = DataOperation.Parse("qry_cof_balanza_real", fromDate, toDate);

      return DataReader.GetPlainObjectFixedList<BalanzaValorizadaReal>(op);
    }


  }  // class BalanzaValorizadaRealDataService

}  // namespace Empiria.FinancialAccounting.BalanceEngine.Data
